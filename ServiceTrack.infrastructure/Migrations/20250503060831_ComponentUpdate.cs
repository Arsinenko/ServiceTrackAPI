using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthApp.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ComponentUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Equipment",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentEquipmentId",
                table: "Equipment",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_ParentEquipmentId",
                table: "Equipment",
                column: "ParentEquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_Equipment_ParentEquipmentId",
                table: "Equipment",
                column: "ParentEquipmentId",
                principalTable: "Equipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_Equipment_ParentEquipmentId",
                table: "Equipment");

            migrationBuilder.DropIndex(
                name: "IX_Equipment_ParentEquipmentId",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "ParentEquipmentId",
                table: "Equipment");
        }
    }
}
