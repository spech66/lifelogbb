using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifelogBb.Migrations
{
    /// <inheritdoc />
    public partial class BucketListImageToFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "BucketLists");

            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "BucketLists",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "BucketLists");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "BucketLists",
                type: "BLOB",
                nullable: true);
        }
    }
}
