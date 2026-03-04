using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DAL.EF.Migrations
{
    /// <inheritdoc />
    public partial class SupportSystemWorkflows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupportImpersonationSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupportUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImpersonatedAppUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportImpersonationSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportImpersonationSessions_AspNetUsers_ImpersonatedAppUse~",
                        column: x => x.ImpersonatedAppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupportImpersonationSessions_AspNetUsers_SupportUserId",
                        column: x => x.SupportUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupportImpersonationSessions_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupportTicketStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTicketStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemAnalyticsSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CapturedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActiveCompanies = table.Column<int>(type: "integer", nullable: false),
                    ActiveSubscribers = table.Column<int>(type: "integer", nullable: false),
                    WeeklyDeliveries = table.Column<int>(type: "integer", nullable: false),
                    OpenSupportTickets = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAnalyticsSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    ValueType = table.Column<string>(type: "text", nullable: false),
                    IsSensitive = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedByAppUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemSettings_AspNetUsers_UpdatedByAppUserId",
                        column: x => x.UpdatedByAppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TenantSupportAccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupportUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GrantedByAppUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantSupportAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantSupportAccesses_AspNetUsers_GrantedByAppUserId",
                        column: x => x.GrantedByAppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantSupportAccesses_AspNetUsers_SupportUserId",
                        column: x => x.SupportUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantSupportAccesses_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupportTickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByAppUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedToAppUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupportTicketStatusId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportTickets_AspNetUsers_AssignedToAppUserId",
                        column: x => x.AssignedToAppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupportTickets_AspNetUsers_CreatedByAppUserId",
                        column: x => x.CreatedByAppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupportTickets_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupportTickets_SupportTicketStatuses_SupportTicketStatusId",
                        column: x => x.SupportTicketStatusId,
                        principalTable: "SupportTicketStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupportImpersonationSessions_CompanyId",
                table: "SupportImpersonationSessions",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportImpersonationSessions_ImpersonatedAppUserId",
                table: "SupportImpersonationSessions",
                column: "ImpersonatedAppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportImpersonationSessions_SupportUserId",
                table: "SupportImpersonationSessions",
                column: "SupportUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_AssignedToAppUserId",
                table: "SupportTickets",
                column: "AssignedToAppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_CompanyId",
                table: "SupportTickets",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_CreatedByAppUserId",
                table: "SupportTickets",
                column: "CreatedByAppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_SupportTicketStatusId",
                table: "SupportTickets",
                column: "SupportTicketStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTicketStatuses_Code",
                table: "SupportTicketStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Category_Key",
                table: "SystemSettings",
                columns: new[] { "Category", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_UpdatedByAppUserId",
                table: "SystemSettings",
                column: "UpdatedByAppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantSupportAccesses_CompanyId_SupportUserId_RevokedAt",
                table: "TenantSupportAccesses",
                columns: new[] { "CompanyId", "SupportUserId", "RevokedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantSupportAccesses_GrantedByAppUserId",
                table: "TenantSupportAccesses",
                column: "GrantedByAppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantSupportAccesses_SupportUserId",
                table: "TenantSupportAccesses",
                column: "SupportUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupportImpersonationSessions");

            migrationBuilder.DropTable(
                name: "SupportTickets");

            migrationBuilder.DropTable(
                name: "SystemAnalyticsSnapshots");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "TenantSupportAccesses");

            migrationBuilder.DropTable(
                name: "SupportTicketStatuses");
        }
    }
}
