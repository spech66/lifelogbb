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
            // Pre-check: fail early with a clear message if duplicate calendar-day dates exist.
            // RAISE() is only valid inside SQLite triggers, so we use a temp table + BEFORE INSERT trigger.
            migrationBuilder.Sql("CREATE TEMP TABLE IF NOT EXISTS _journal_date_dedup (d TEXT NOT NULL);");
            migrationBuilder.Sql(
                "CREATE TEMP TRIGGER IF NOT EXISTS _journal_date_dedup_check BEFORE INSERT ON _journal_date_dedup " +
                "BEGIN " +
                "SELECT RAISE(ABORT, 'Cannot apply migration: duplicate journal entries exist for the same calendar day. " +
                "Please manually resolve duplicate journal entries before upgrading.') " +
                "WHERE EXISTS (SELECT 1 FROM _journal_date_dedup WHERE d = NEW.d); " +
                "END;");
            migrationBuilder.Sql("INSERT INTO _journal_date_dedup SELECT date(Date) FROM Journals;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS _journal_date_dedup_check;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS _journal_date_dedup;");

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
