using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifelogBb.Migrations
{
    /// <inheritdoc />
    public partial class TrainingSetDurationAndOptionalWeight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "TrainingPlanSets",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AddColumn<int>(
                name: "DurationSeconds",
                table: "TrainingPlanSets",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "StrengthTrainings",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AddColumn<int>(
                name: "DurationSeconds",
                table: "StrengthTrainings",
                type: "INTEGER",
                nullable: true);

            // Before this migration a set without a weight had to be stored as 0, so every bodyweight,
            // band and mobility set looks like an explicit 0 kg. Nothing is lost by reading those back
            // as "no weight applies": a 0 kg set has no volume either way, and null keeps it out of the
            // volume statistics instead of dragging them down.
            migrationBuilder.Sql("UPDATE StrengthTrainings SET Weight = NULL WHERE Weight = 0");
            migrationBuilder.Sql("UPDATE TrainingPlanSets SET Weight = NULL WHERE Weight = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // The columns go back to NOT NULL below, and SQLite rebuilds the table to do it, so the
            // nulls have to become 0 again first or the copy fails.
            migrationBuilder.Sql("UPDATE StrengthTrainings SET Weight = 0 WHERE Weight IS NULL");
            migrationBuilder.Sql("UPDATE TrainingPlanSets SET Weight = 0 WHERE Weight IS NULL");

            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "TrainingPlanSets");

            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "StrengthTrainings");

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "TrainingPlanSets",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "StrengthTrainings",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldNullable: true);
        }
    }
}
