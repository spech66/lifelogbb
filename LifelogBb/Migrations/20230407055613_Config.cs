using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifelogBb.Migrations
{
    /// <inheritdoc />
    public partial class Config : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartOfWeek = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: DayOfWeek.Monday),
                    UnitsType = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    BucketListPageSize = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 12),
                    WeightPageSize = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 20),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configs");
        }
    }
}
