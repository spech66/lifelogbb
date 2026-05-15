using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifelogBb.Migrations
{
    /// <inheritdoc />
    public partial class JournalDateUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Normalize existing values to midnight timestamps to match EF's DateTime storage format.
            migrationBuilder.Sql("UPDATE Journals SET Date = datetime(date(Date));");

            migrationBuilder.CreateIndex(
                name: "IX_Journals_Date",
                table: "Journals",
                column: "Date",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Journals_Date",
                table: "Journals");
        }
    }
}
