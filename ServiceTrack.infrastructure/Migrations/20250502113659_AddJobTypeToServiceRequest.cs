using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthApp.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobTypeToServiceRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "JobTypeId",
                table: "ServiceRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_JobTypeId",
                table: "ServiceRequests",
                column: "JobTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_JobTypes_JobTypeId",
                table: "ServiceRequests",
                column: "JobTypeId",
                principalTable: "JobTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_JobTypes_JobTypeId",
                table: "ServiceRequests");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_JobTypeId",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "JobTypeId",
                table: "ServiceRequests");
        }
    }
}
