using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifelogBb.Migrations
{
    /// <inheritdoc />
    public partial class OAuthServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "McpOAuthEnabled",
                table: "Configs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "OAuthClients",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    ClientName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    RedirectUris = table.Column<string>(type: "TEXT", nullable: false),
                    Scope = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthClients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OAuthGrants",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GrantType = table.Column<int>(type: "INTEGER", nullable: false),
                    OAuthClientId = table.Column<long>(type: "INTEGER", nullable: false),
                    TokenHash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    SessionId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    RedirectUri = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CodeChallenge = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Scope = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Resource = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ConsumedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthGrants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OAuthGrants_OAuthClients_OAuthClientId",
                        column: x => x.OAuthClientId,
                        principalTable: "OAuthClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OAuthClients_ClientId",
                table: "OAuthClients",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OAuthGrants_ExpiresAt",
                table: "OAuthGrants",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_OAuthGrants_OAuthClientId",
                table: "OAuthGrants",
                column: "OAuthClientId");

            migrationBuilder.CreateIndex(
                name: "IX_OAuthGrants_SessionId",
                table: "OAuthGrants",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_OAuthGrants_TokenHash",
                table: "OAuthGrants",
                column: "TokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OAuthGrants");

            migrationBuilder.DropTable(
                name: "OAuthClients");

            migrationBuilder.DropColumn(
                name: "McpOAuthEnabled",
                table: "Configs");
        }
    }
}
