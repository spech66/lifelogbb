using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifelogBb.Migrations
{
    /// <inheritdoc />
    public partial class TrainingPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "StrengthTrainings",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.Sql("UPDATE StrengthTrainings SET Date = CreatedAt");

            migrationBuilder.AddColumn<long>(
                name: "TrainingPlanId",
                table: "StrengthTrainings",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TrainingPlanSetId",
                table: "StrengthTrainings",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrainingPlanPageSize",
                table: "Configs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 20);

            migrationBuilder.CreateTable(
                name: "TrainingPlans",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrainingPlanSets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TrainingPlanId = table.Column<long>(type: "INTEGER", nullable: false),
                    Exercise = table.Column<string>(type: "TEXT", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Reps = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<double>(type: "REAL", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPlanSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingPlanSets_TrainingPlans_TrainingPlanId",
                        column: x => x.TrainingPlanId,
                        principalTable: "TrainingPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StrengthTrainings_TrainingPlanId",
                table: "StrengthTrainings",
                column: "TrainingPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_StrengthTrainings_TrainingPlanSetId",
                table: "StrengthTrainings",
                column: "TrainingPlanSetId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlanSets_TrainingPlanId",
                table: "TrainingPlanSets",
                column: "TrainingPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_StrengthTrainings_TrainingPlanSets_TrainingPlanSetId",
                table: "StrengthTrainings",
                column: "TrainingPlanSetId",
                principalTable: "TrainingPlanSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StrengthTrainings_TrainingPlans_TrainingPlanId",
                table: "StrengthTrainings",
                column: "TrainingPlanId",
                principalTable: "TrainingPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StrengthTrainings_TrainingPlanSets_TrainingPlanSetId",
                table: "StrengthTrainings");

            migrationBuilder.DropForeignKey(
                name: "FK_StrengthTrainings_TrainingPlans_TrainingPlanId",
                table: "StrengthTrainings");

            migrationBuilder.DropTable(
                name: "TrainingPlanSets");

            migrationBuilder.DropTable(
                name: "TrainingPlans");

            migrationBuilder.DropIndex(
                name: "IX_StrengthTrainings_TrainingPlanId",
                table: "StrengthTrainings");

            migrationBuilder.DropIndex(
                name: "IX_StrengthTrainings_TrainingPlanSetId",
                table: "StrengthTrainings");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "StrengthTrainings");

            migrationBuilder.DropColumn(
                name: "TrainingPlanId",
                table: "StrengthTrainings");

            migrationBuilder.DropColumn(
                name: "TrainingPlanSetId",
                table: "StrengthTrainings");

            migrationBuilder.DropColumn(
                name: "TrainingPlanPageSize",
                table: "Configs");
        }
    }
}
