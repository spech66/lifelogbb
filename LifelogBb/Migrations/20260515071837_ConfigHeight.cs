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
                SET Height = COALESCE(
                    (SELECT Height FROM Weights WHERE Height > 0 ORDER BY CreatedAt DESC LIMIT 1),
                    CASE WHEN UnitsType = 1 THEN 67 ELSE 170 END
                )
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
