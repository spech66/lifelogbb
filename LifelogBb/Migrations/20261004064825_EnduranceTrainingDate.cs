using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifelogBb.Migrations
{
    /// <inheritdoc />
    public partial class EnduranceTrainingDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "EnduranceTrainings",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // Existing workouts were logged on the day they happened; keep only the day part so they
            // compare equal to new entries, which store midnight.
            migrationBuilder.Sql("UPDATE EnduranceTrainings SET Date = strftime('%Y-%m-%d 00:00:00', CreatedAt)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "EnduranceTrainings");
        }
    }
}
