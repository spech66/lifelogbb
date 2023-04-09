using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifelogBb.Migrations
{
    /// <inheritdoc />
    public partial class Ical : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FrequencyUnit",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "Habits");

            migrationBuilder.RenameColumn(
                name: "ExtraRules",
                table: "Habits",
                newName: "RecurrenceRules");

            migrationBuilder.AddColumn<DateTime>(
                name: "Completed",
                table: "Todos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Progress",
                table: "Todos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Todos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeedTimeZone",
                table: "Configs",
                type: "TEXT",
                nullable: false,
                defaultValue: "Europe/Berlin");

            migrationBuilder.AddColumn<string>(
                name: "FeedToken",
                table: "Configs",
                type: "TEXT",
                nullable: false,
                defaultValue: "ChangeMeInTheConfig");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "Progress",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "FeedTimeZone",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "FeedToken",
                table: "Configs");

            migrationBuilder.RenameColumn(
                name: "RecurrenceRules",
                table: "Habits",
                newName: "FrequencyUnit");

            migrationBuilder.AddColumn<string>(
                name: "ExtraRules",
                table: "Habits",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "Habits",
                type: "INTEGER",
                nullable: true);
        }
    }
}
