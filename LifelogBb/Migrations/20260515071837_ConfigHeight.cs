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

            // Backfill Height from the most recent Weight entry; fall back to unit-appropriate default
            migrationBuilder.Sql(@"
                UPDATE Configs
                SET Height = CASE
                    WHEN UnitsType = 1 THEN COALESCE(
                        (SELECT Height FROM Weights WHERE Height BETWEEN 16 AND 98 ORDER BY CreatedAt DESC LIMIT 1),
                        67
                    )
                    ELSE COALESCE(
                        (SELECT Height FROM Weights WHERE Height BETWEEN 40 AND 250 ORDER BY CreatedAt DESC LIMIT 1),
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
