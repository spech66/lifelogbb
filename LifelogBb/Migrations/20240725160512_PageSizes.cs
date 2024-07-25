using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifelogBb.Migrations
{
    /// <inheritdoc />
    public partial class PageSizes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WeightWarningText",
                table: "Configs",
                type: "TEXT",
                nullable: false,
                defaultValue: "You are gaining weight!",
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<double>(
                name: "WeightWarning",
                table: "Configs",
                type: "REAL",
                nullable: false,
                defaultValue: 1.0,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AddColumn<int>(
                name: "EnduranceTrainingPageSize",
                table: "Configs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 20);

            migrationBuilder.AddColumn<int>(
                name: "GoalPageSize",
                table: "Configs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 20);

            migrationBuilder.AddColumn<int>(
                name: "HabitPageSize",
                table: "Configs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 20);

            migrationBuilder.AddColumn<int>(
                name: "JournalPageSize",
                table: "Configs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 20);

            migrationBuilder.AddColumn<int>(
                name: "QuotePageSize",
                table: "Configs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 20);

            migrationBuilder.AddColumn<int>(
                name: "StrengthTrainingPageSize",
                table: "Configs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 20);

            migrationBuilder.AddColumn<int>(
                name: "TodoPageSize",
                table: "Configs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 20);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnduranceTrainingPageSize",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "GoalPageSize",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "HabitPageSize",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "JournalPageSize",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "QuotePageSize",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "StrengthTrainingPageSize",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "TodoPageSize",
                table: "Configs");

            migrationBuilder.AlterColumn<string>(
                name: "WeightWarningText",
                table: "Configs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldDefaultValue: "You are gaining weight!");

            migrationBuilder.AlterColumn<double>(
                name: "WeightWarning",
                table: "Configs",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldDefaultValue: 1.0);
        }
    }
}
