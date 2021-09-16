using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    VoteId = table.Column<Guid>(nullable: false),
                    VoteName = table.Column<string>(maxLength: 150, nullable: true),
                    VoteDescription = table.Column<string>(nullable: true),
                    Question = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.VoteId);
                    table.ForeignKey(
                        name: "FK_Votes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VoteAnswers",
                columns: table => new
                {
                    VoteAnswerId = table.Column<Guid>(nullable: false),
                    Option = table.Column<string>(nullable: true),
                    OptionDescription = table.Column<string>(nullable: true),
                    VoteId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteAnswers", x => x.VoteAnswerId);
                    table.ForeignKey(
                        name: "FK_VoteAnswers_Votes_VoteId",
                        column: x => x.VoteId,
                        principalTable: "Votes",
                        principalColumn: "VoteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserVoteAnswers",
                columns: table => new
                {
                    UserVoteAnswerId = table.Column<Guid>(nullable: false),
                    SubmitTime = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    VoteId = table.Column<Guid>(nullable: false),
                    VoteOptionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVoteAnswers", x => x.UserVoteAnswerId);
                    table.ForeignKey(
                        name: "FK_UserVoteAnswers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserVoteAnswers_Votes_VoteId",
                        column: x => x.VoteId,
                        principalTable: "Votes",
                        principalColumn: "VoteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserVoteAnswers_VoteAnswers_VoteOptionId",
                        column: x => x.VoteOptionId,
                        principalTable: "VoteAnswers",
                        principalColumn: "VoteAnswerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserVoteAnswers_UserId",
                table: "UserVoteAnswers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVoteAnswers_VoteId",
                table: "UserVoteAnswers",
                column: "VoteId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVoteAnswers_VoteOptionId",
                table: "UserVoteAnswers",
                column: "VoteOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_VoteAnswers_VoteId",
                table: "VoteAnswers",
                column: "VoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserId",
                table: "Votes",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserVoteAnswers");

            migrationBuilder.DropTable(
                name: "VoteAnswers");

            migrationBuilder.DropTable(
                name: "Votes");
        }
    }
}
