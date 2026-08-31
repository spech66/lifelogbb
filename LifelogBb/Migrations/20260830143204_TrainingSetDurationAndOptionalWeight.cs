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

            // The 0 -> null backfill for existing rows lives in the BackfillOptionalSetWeight migration.
            // SQLite cannot alter a column in place, so the AlterColumn calls above are applied as a
            // deferred table rebuild at the end of this migration. Raw SQL does not force that rebuild,
            // so an UPDATE writing null here would still hit the old NOT NULL table and fail.
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
