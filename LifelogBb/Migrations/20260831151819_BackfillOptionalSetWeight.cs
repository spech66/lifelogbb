using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifelogBb.Migrations
{
    /// <inheritdoc />
    public partial class BackfillOptionalSetWeight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Before TrainingSetDurationAndOptionalWeight a set without a weight had to be stored as 0,
            // so every bodyweight, band and mobility set looks like an explicit 0 kg. Nothing is lost by
            // reading those back as "no weight applies": a 0 kg set has no volume either way, and null
            // keeps it out of the volume statistics instead of dragging them down.
            //
            // This runs in its own migration because SQLite applies the nullable change as a deferred
            // table rebuild, which is only flushed once the migration that requests it has finished.
            migrationBuilder.Sql("UPDATE StrengthTrainings SET Weight = NULL WHERE Weight = 0");
            migrationBuilder.Sql("UPDATE TrainingPlanSets SET Weight = NULL WHERE Weight = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE StrengthTrainings SET Weight = 0 WHERE Weight IS NULL");
            migrationBuilder.Sql("UPDATE TrainingPlanSets SET Weight = 0 WHERE Weight IS NULL");
        }
    }
}
