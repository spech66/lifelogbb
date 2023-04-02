using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifelogBb.Migrations
{
    /// <inheritdoc />
    public partial class Tags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Todos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Todos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Quotes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Quotes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Journals",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Journals",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Habits",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Habits",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Goals",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Goals",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "BucketLists",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Journals");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Journals");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "BucketLists");
        }
    }
}
