using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AuthApp.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ServiceRequestManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Users_AssignedUserId",
                table: "ServiceRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceRequests",
                table: "ServiceRequests");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_AssignedUserId",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "AssignedUserId",
                table: "ServiceRequests");

            migrationBuilder.AlterColumn<int>(
                name: "ContractId",
                table: "ServiceRequests",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ServiceRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceRequests",
                table: "ServiceRequests",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserServiceRequests",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceRequestId = table.Column<int>(type: "integer", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPrimaryAssignee = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserServiceRequests", x => new { x.UserId, x.ServiceRequestId });
                    table.ForeignKey(
                        name: "FK_UserServiceRequests_ServiceRequests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalTable: "ServiceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserServiceRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_ContractId",
                table: "ServiceRequests",
                column: "ContractId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserServiceRequests_ServiceRequestId",
                table: "UserServiceRequests",
                column: "ServiceRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserServiceRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceRequests",
                table: "ServiceRequests");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_ContractId",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ServiceRequests");

            migrationBuilder.AlterColumn<int>(
                name: "ContractId",
                table: "ServiceRequests",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedUserId",
                table: "ServiceRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceRequests",
                table: "ServiceRequests",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_AssignedUserId",
                table: "ServiceRequests",
                column: "AssignedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Users_AssignedUserId",
                table: "ServiceRequests",
                column: "AssignedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
