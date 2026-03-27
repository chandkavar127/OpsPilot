using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpsPilot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ManagerAdminApprovalWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AdminActionDate",
                table: "Requests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminApprovedById",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Requests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ManagerActionDate",
                table: "Requests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerApprovedById",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManagerProfileId",
                table: "EmployeeProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Status_EmployeeProfileId",
                table: "Requests",
                columns: new[] { "Status", "EmployeeProfileId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProfiles_ManagerProfileId",
                table: "EmployeeProfiles",
                column: "ManagerProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProfiles_EmployeeProfiles_ManagerProfileId",
                table: "EmployeeProfiles",
                column: "ManagerProfileId",
                principalTable: "EmployeeProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProfiles_EmployeeProfiles_ManagerProfileId",
                table: "EmployeeProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Requests_Status_EmployeeProfileId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeProfiles_ManagerProfileId",
                table: "EmployeeProfiles");

            migrationBuilder.DropColumn(
                name: "AdminActionDate",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "AdminApprovedById",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ManagerActionDate",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ManagerApprovedById",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ManagerProfileId",
                table: "EmployeeProfiles");
        }
    }
}
