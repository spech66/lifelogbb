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
                DELETE FROM Journals
                WHERE Id IN (
                    SELECT older.Id
                    FROM Journals older
                    JOIN Journals newer
                        ON date(older.Date) = date(newer.Date)
                        AND (
                            newer.UpdatedAt > older.UpdatedAt
                            OR (newer.UpdatedAt = older.UpdatedAt AND newer.Id > older.Id)
                        )
                );
            ");

            migrationBuilder.Sql("UPDATE Journals SET Date = date(Date);");

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
