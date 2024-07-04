using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifelogBb.Migrations
{
    /// <inheritdoc />
    public partial class WeightWarning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "WeightWarning",
                table: "Configs",
                type: "REAL",
                nullable: false,
                defaultValue: 1.0);

            migrationBuilder.AddColumn<string>(
                name: "WeightWarningText",
                table: "Configs",
                type: "TEXT",
                nullable: false,
                defaultValue: "You are gaining weight!");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeightWarning",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "WeightWarningText",
                table: "Configs");
        }
    }
}
