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
