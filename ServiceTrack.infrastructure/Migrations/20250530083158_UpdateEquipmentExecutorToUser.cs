using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthApp.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEquipmentExecutorToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Executor",
                table: "Equipment");

            migrationBuilder.AddColumn<Guid>(
                name: "ExecutorId",
                table: "Equipment",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_ExecutorId",
                table: "Equipment",
                column: "ExecutorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_Users_ExecutorId",
                table: "Equipment",
                column: "ExecutorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_Users_ExecutorId",
                table: "Equipment");

            migrationBuilder.DropIndex(
                name: "IX_Equipment_ExecutorId",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "ExecutorId",
                table: "Equipment");

            migrationBuilder.AddColumn<string>(
                name: "Executor",
                table: "Equipment",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
