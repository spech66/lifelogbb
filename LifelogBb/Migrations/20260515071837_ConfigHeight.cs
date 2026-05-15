using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifelogBb.Migrations
{
    /// <inheritdoc />
    public partial class ConfigHeight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "Configs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 170);

            // Backfill Height from the most recent Weight entry only when the stored value is
            // unambiguous for the current unit system. The valid ranges overlap from 40-98, so
            // those values cannot be safely inferred as metric or imperial during migration.
            // Range values align with EditConfigViewModel constants: metric 40-250, imperial 16-98.
            migrationBuilder.Sql(@"
                UPDATE Configs
                SET Height = CASE
                    WHEN UnitsType = 1 THEN COALESCE(
                        (SELECT Height FROM Weights WHERE Height BETWEEN 16 AND 39 ORDER BY CreatedAt DESC LIMIT 1),
                        67
                    )
                    ELSE COALESCE(
                        (SELECT Height FROM Weights WHERE Height BETWEEN 99 AND 250 ORDER BY CreatedAt DESC LIMIT 1),
                        170
                    )
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "Configs");
        }
    }
}
