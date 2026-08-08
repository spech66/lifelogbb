using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifelogBb.Migrations
{
    /// <inheritdoc />
    public partial class IcalAlarms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alarms",
                table: "Todos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Alarms",
                table: "Habits",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Alarms",
                table: "Goals",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alarms",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "Alarms",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "Alarms",
                table: "Goals");
        }
    }
}
