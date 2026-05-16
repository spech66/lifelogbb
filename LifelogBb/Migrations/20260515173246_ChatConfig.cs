using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifelogBb.Migrations
{
    /// <inheritdoc />
    public partial class ChatConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChatApiKey",
                table: "Configs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ChatEndpoint",
                table: "Configs",
                type: "TEXT",
                nullable: false,
                defaultValue: "https://api.openai.com/v1/chat/completions");

            migrationBuilder.AddColumn<int>(
                name: "ChatMaxToolRoundtrips",
                table: "Configs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 10);

            migrationBuilder.AddColumn<string>(
                name: "ChatModel",
                table: "Configs",
                type: "TEXT",
                nullable: false,
                defaultValue: "gpt-4o");

            migrationBuilder.AddColumn<string>(
                name: "ChatSystemPrompt",
                table: "Configs",
                type: "TEXT",
                nullable: false,
                defaultValue: "You are a helpful life-tracking assistant for LifelogBB. You can query the user's data (weights, journals, todos, goals, habits, quotes, strength trainings, endurance trainings) using the available tools. Summarize and analyze the data to help the user understand their progress and habits.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatApiKey",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "ChatEndpoint",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "ChatMaxToolRoundtrips",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "ChatModel",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "ChatSystemPrompt",
                table: "Configs");
        }
    }
}
