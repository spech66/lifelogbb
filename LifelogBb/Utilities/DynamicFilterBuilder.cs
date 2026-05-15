using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using LifelogBb.Models.Filtering;

namespace LifelogBb.Utilities
{
    public static class DynamicFilterBuilder
    {
        private const int MaxDepth = 5;
        private const int MaxConditions = 20;

        /// <summary>
        /// Builds an Expression&lt;Func&lt;T, bool&gt;&gt; from a FilterGroup tree.
        /// Returns null if the filter group is empty (no conditions or sub-groups).
        /// </summary>
        public static Expression<Func<T, bool>>? BuildExpression<T>(FilterGroup group)
        {
            EnsureGroupCollectionsAreNotNull(group);
            var allowedProperties = GetAllowedProperties<T>();
            ValidateLimits(group, MaxDepth, MaxConditions);

            var parameter = Expression.Parameter(typeof(T), "x");
            var body = BuildGroupExpression<T>(group, parameter, allowedProperties, 0);

            if (body == null)
                return null;

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private static Expression? BuildGroupExpression<T>(
            FilterGroup group,
            ParameterExpression parameter,
            HashSet<string> allowedProperties,
            int depth)
        {
            if (depth > MaxDepth)
                throw new ArgumentException($"Filter tree exceeds maximum depth of {MaxDepth}.");
            EnsureGroupCollectionsAreNotNull(group);

            var expressions = new List<Expression>();

            // Build condition expressions
            foreach (var condition in group.Conditions)
            {
                var expr = BuildConditionExpression<T>(condition, parameter, allowedProperties);
                if (expr != null)
                    expressions.Add(expr);
            }

            // Recursively build sub-group expressions
            foreach (var subGroup in group.Groups)
            {
                var expr = BuildGroupExpression<T>(subGroup, parameter, allowedProperties, depth + 1);
                if (expr != null)
                    expressions.Add(expr);
            }

            if (expressions.Count == 0)
                return null;

            // Combine with AND or OR
            var combined = expressions[0];
            for (int i = 1; i < expressions.Count; i++)
            {
                combined = group.Operator == LogicalOperator.And
                    ? Expression.AndAlso(combined, expressions[i])
                    : Expression.OrElse(combined, expressions[i]);
            }

            return combined;
        }

        private static Expression? BuildConditionExpression<T>(
            FilterCondition condition,
            ParameterExpression parameter,
            HashSet<string> allowedProperties)
        {
            if (string.IsNullOrWhiteSpace(condition.Field))
                return null;

            if (condition.Value == null)
                throw new ArgumentException($"Condition value for field '{condition.Field}' cannot be null.");

            if (!allowedProperties.Contains(condition.Field))
                throw new ArgumentException($"Field '{condition.Field}' is not a valid filterable property.");

            var property = typeof(T).GetProperty(condition.Field, BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
                return null;

            var memberAccess = Expression.Property(parameter, property);
            var propertyType = property.PropertyType;
            var underlyingType = Nullable.GetUnderlyingType(propertyType);
            var isNullable = underlyingType != null;
            var targetType = underlyingType ?? propertyType;

            // For In/NotIn operators, handle list of values
            if (condition.Operator == ComparisonOperator.In || condition.Operator == ComparisonOperator.NotIn)
            {
                return BuildInExpression(condition, memberAccess, propertyType, targetType, isNullable);
            }

            // For Contains/NotContains, only valid for string properties
            if (condition.Operator == ComparisonOperator.Contains || condition.Operator == ComparisonOperator.NotContains)
            {
                if (targetType != typeof(string))
                    throw new ArgumentException($"Operator '{condition.Operator}' is only valid for string properties. Field '{condition.Field}' is {targetType.Name}.");

                return BuildStringContainsExpression(condition, memberAccess);
            }

            // Parse the value to the target type
            object parsedValue = ParseValue(condition.Value, targetType);

            var constant = Expression.Constant(parsedValue, targetType);

            // For nullable properties, access .Value for comparison
            Expression compareLeft = isNullable
                ? Expression.Property(memberAccess, "Value")
                : memberAccess;

            if (IsComparisonOperator(condition.Operator) && !SupportsComparisonOperators(targetType))
                throw new ArgumentException($"Operator '{condition.Operator}' is not valid for field '{condition.Field}' of type '{targetType.Name}'.");

            Expression comparison = condition.Operator switch
            {
                ComparisonOperator.Equal => Expression.Equal(compareLeft, constant),
                ComparisonOperator.NotEqual => Expression.NotEqual(compareLeft, constant),
                ComparisonOperator.GreaterThan => Expression.GreaterThan(compareLeft, constant),
                ComparisonOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(compareLeft, constant),
                ComparisonOperator.LessThan => Expression.LessThan(compareLeft, constant),
                ComparisonOperator.LessThanOrEqual => Expression.LessThanOrEqual(compareLeft, constant),
                _ => throw new ArgumentException($"Unsupported operator: {condition.Operator}")
            };

            // For nullable properties, add HasValue check
            if (isNullable)
            {
                var hasValue = Expression.Property(memberAccess, "HasValue");
                comparison = Expression.AndAlso(hasValue, comparison);
            }

            return comparison;
        }

        private static Expression BuildStringContainsExpression(
            FilterCondition condition,
            MemberExpression memberAccess)
        {
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
            var valueConstant = Expression.Constant(condition.Value, typeof(string));

            // Always add a null check since strings are nullable reference types
            var nullCheck = Expression.NotEqual(memberAccess, Expression.Constant(null, typeof(string)));
            var containsCall = Expression.Call(memberAccess, containsMethod, valueConstant);
            Expression result = Expression.AndAlso(nullCheck, containsCall);

            if (condition.Operator == ComparisonOperator.NotContains)
            {
                result = Expression.AndAlso(nullCheck, Expression.Not(containsCall));
            }

            return result;
        }

        private static Expression? BuildInExpression(
            FilterCondition condition,
            MemberExpression memberAccess,
            Type propertyType,
            Type targetType,
            bool isNullable)
        {
            // Parse comma-separated values
            if (string.IsNullOrWhiteSpace(condition.Value))
                throw new ArgumentException($"Condition value for field '{condition.Field ?? "<unknown>"}' cannot be null or empty for {condition.Operator}.");

            var rawValues = condition.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (rawValues.Length == 0)
                throw new ArgumentException($"Condition value for field '{condition.Field ?? "<unknown>"}' must contain at least one value for {condition.Operator}.");

            var parsedValues = new List<object>();
            foreach (var raw in rawValues)
            {
                parsedValues.Add(ParseValue(raw, targetType));
            }

            // Create a typed list and use Contains
            var listType = typeof(List<>).MakeGenericType(targetType);
            var list = Activator.CreateInstance(listType)!;
            var addMethod = listType.GetMethod("Add")!;
            foreach (var val in parsedValues)
                addMethod.Invoke(list, new[] { val });

            var listConstant = Expression.Constant(list, listType);
            var containsMethod = listType.GetMethod("Contains", new[] { targetType })!;

            Expression valueExpr = isNullable
                ? Expression.Property(memberAccess, "Value")
                : memberAccess;

            Expression containsCall = Expression.Call(listConstant, containsMethod, valueExpr);

            if (isNullable)
            {
                var hasValue = Expression.Property(memberAccess, "HasValue");
                if (condition.Operator == ComparisonOperator.NotIn)
                {
                    // (HasValue) AND NOT list.Contains(Value) — nulls do not match
                    containsCall = Expression.AndAlso(hasValue, Expression.Not(containsCall));
                }
                else
                {
                    containsCall = Expression.AndAlso(hasValue, containsCall);
                }
            }
            else if (condition.Operator == ComparisonOperator.NotIn)
            {
                containsCall = Expression.Not(containsCall);
            }

            return containsCall;
        }

        private static object ParseValue(string value, Type targetType)
        {
            try
            {
                if (targetType == typeof(string))
                    return value;

                if (targetType == typeof(int))
                    return int.Parse(value, CultureInfo.InvariantCulture);

                if (targetType == typeof(long))
                    return long.Parse(value, CultureInfo.InvariantCulture);

                if (targetType == typeof(double))
                    return double.Parse(value, CultureInfo.InvariantCulture);

                if (targetType == typeof(float))
                    return float.Parse(value, CultureInfo.InvariantCulture);

                if (targetType == typeof(decimal))
                    return decimal.Parse(value, CultureInfo.InvariantCulture);

                if (targetType == typeof(bool))
                    return bool.Parse(value);

                if (targetType == typeof(DateTime))
                    return DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                if (targetType == typeof(DateTimeOffset))
                    return DateTimeOffset.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                if (targetType == typeof(TimeSpan))
                    return TimeSpan.Parse(value, CultureInfo.InvariantCulture);

                if (targetType == typeof(Guid))
                    return Guid.Parse(value);

                if (targetType.IsEnum)
                    return Enum.Parse(targetType, value, ignoreCase: true);

                // Fallback: try Convert
                return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture)!;
            }
            catch (Exception ex) when (ex is not ArgumentException)
            {
                throw new ArgumentException($"Cannot parse value '{value}' as type '{targetType.Name}'.", ex);
            }
        }

        private static bool IsComparisonOperator(ComparisonOperator op)
        {
            return op == ComparisonOperator.GreaterThan
                || op == ComparisonOperator.GreaterThanOrEqual
                || op == ComparisonOperator.LessThan
                || op == ComparisonOperator.LessThanOrEqual;
        }

        private static bool SupportsComparisonOperators(Type type)
        {
            var t = Nullable.GetUnderlyingType(type) ?? type;
            return t == typeof(byte)
                || t == typeof(sbyte)
                || t == typeof(short)
                || t == typeof(ushort)
                || t == typeof(int)
                || t == typeof(uint)
                || t == typeof(long)
                || t == typeof(ulong)
                || t == typeof(float)
                || t == typeof(double)
                || t == typeof(decimal)
                || t == typeof(DateTime)
                || t == typeof(DateTimeOffset)
                || t == typeof(TimeSpan);
        }

        private static HashSet<string> GetAllowedProperties<T>()
        {
            return typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => IsSimpleType(p.PropertyType))
                .Select(p => p.Name)
                .ToHashSet();
        }

        private static bool IsSimpleType(Type type)
        {
            var t = Nullable.GetUnderlyingType(type) ?? type;
            return t.IsPrimitive
                || t.IsEnum
                || t == typeof(string)
                || t == typeof(decimal)
                || t == typeof(DateTime)
                || t == typeof(DateTimeOffset)
                || t == typeof(TimeSpan)
                || t == typeof(Guid);
        }

        private static void ValidateLimits(FilterGroup group, int maxDepth, int maxConditions, int currentDepth = 0, int[]? conditionCount = null)
        {
            conditionCount ??= new[] { 0 };
            EnsureGroupCollectionsAreNotNull(group);

            if (currentDepth > maxDepth)
                throw new ArgumentException($"Filter tree exceeds maximum depth of {maxDepth}.");

            conditionCount[0] += group.Conditions.Count;
            if (conditionCount[0] > maxConditions)
                throw new ArgumentException($"Filter tree exceeds maximum of {maxConditions} conditions.");

            foreach (var subGroup in group.Groups)
            {
                ValidateLimits(subGroup, maxDepth, maxConditions, currentDepth + 1, conditionCount);
            }
        }

        private static void EnsureGroupCollectionsAreNotNull(FilterGroup group)
        {
            if (group.Conditions == null || group.Groups == null)
                throw new ArgumentException("Filter group contains null collections.");
        }
    }
}
