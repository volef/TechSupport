using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "SupportRequests",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Head = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    DoneTime = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    SupportId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportRequests", x => x.Id);
                    table.ForeignKey(
                        "FK_SupportRequests_AspNetUsers_SupportId",
                        x => x.SupportId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_SupportRequests_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "SupportIdentities",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<string>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    CurrentRequestId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportIdentities", x => x.Id);
                    table.ForeignKey(
                        "FK_SupportIdentities_SupportRequests_CurrentRequestId",
                        x => x.CurrentRequestId,
                        "SupportRequests",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_SupportIdentities_AspNetUsers_OwnerId",
                        x => x.OwnerId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_SupportIdentities_CurrentRequestId",
                "SupportIdentities",
                "CurrentRequestId");

            migrationBuilder.CreateIndex(
                "IX_SupportIdentities_OwnerId",
                "SupportIdentities",
                "OwnerId");

            migrationBuilder.CreateIndex(
                "IX_SupportRequests_SupportId",
                "SupportRequests",
                "SupportId");

            migrationBuilder.CreateIndex(
                "IX_SupportRequests_UserId",
                "SupportRequests",
                "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "SupportIdentities");

            migrationBuilder.DropTable(
                "SupportRequests");
        }
    }
}