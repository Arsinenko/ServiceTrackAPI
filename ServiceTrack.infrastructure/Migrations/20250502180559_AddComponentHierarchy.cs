using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthApp.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddComponentHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentComponentId",
                table: "EquipmentComponents",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentComponents_ParentComponentId",
                table: "EquipmentComponents",
                column: "ParentComponentId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentComponents_EquipmentComponents_ParentComponentId",
                table: "EquipmentComponents",
                column: "ParentComponentId",
                principalTable: "EquipmentComponents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentComponents_EquipmentComponents_ParentComponentId",
                table: "EquipmentComponents");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentComponents_ParentComponentId",
                table: "EquipmentComponents");

            migrationBuilder.DropColumn(
                name: "ParentComponentId",
                table: "EquipmentComponents");
        }
    }
}
