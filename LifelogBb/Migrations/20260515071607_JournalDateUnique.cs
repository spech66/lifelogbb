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
            migrationBuilder.Sql(@"
                SELECT CASE
                    WHEN EXISTS (
                        SELECT 1
                        FROM Journals
                        GROUP BY date(Date)
                        HAVING COUNT(*) > 1
                    )
                    THEN RAISE(ABORT, 'Cannot apply JournalDateUnique migration: duplicate journal dates exist. Resolve duplicates before applying this migration.')
                END;
            ");

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
