using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Owner = table.Column<int>(type: "INTEGER", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Checklists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Statistics_CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    User_MoodRating = table.Column<byte>(type: "INTEGER", nullable: true),
                    User_MotivationRating = table.Column<byte>(type: "INTEGER", nullable: true),
                    User_EffortRating = table.Column<byte>(type: "INTEGER", nullable: true),
                    User_ProductivityRating = table.Column<byte>(type: "INTEGER", nullable: true),
                    User_FocusRating = table.Column<byte>(type: "INTEGER", nullable: true),
                    User_StressRating = table.Column<byte>(type: "INTEGER", nullable: true),
                    LLM_MoodRating = table.Column<byte>(type: "INTEGER", nullable: true),
                    LLM_MotivationRating = table.Column<byte>(type: "INTEGER", nullable: true),
                    LLM_EffortRating = table.Column<byte>(type: "INTEGER", nullable: true),
                    LLM_ProductivityRating = table.Column<byte>(type: "INTEGER", nullable: true),
                    LLM_FocusRating = table.Column<byte>(type: "INTEGER", nullable: true),
                    LLM_StressRating = table.Column<byte>(type: "INTEGER", nullable: true),
                    Owner = table.Column<int>(type: "INTEGER", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checklists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checklists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    TaskType_Name = table.Column<string>(type: "TEXT", nullable: false),
                    TaskType_Category = table.Column<int>(type: "INTEGER", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    PlannedSchedule_Start = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    PlannedSchedule_End = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ActualSchedule_Start = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ActualSchedule_End = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ChecklistId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChecklistStateId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskEntity_Checklists_ChecklistId",
                        column: x => x.ChecklistId,
                        principalTable: "Checklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskEntity_Checklists_ChecklistStateId",
                        column: x => x.ChecklistStateId,
                        principalTable: "Checklists",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_UserId",
                table: "Checklists",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskEntity_ChecklistId",
                table: "TaskEntity",
                column: "ChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskEntity_ChecklistStateId",
                table: "TaskEntity",
                column: "ChecklistStateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskEntity");

            migrationBuilder.DropTable(
                name: "Checklists");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
