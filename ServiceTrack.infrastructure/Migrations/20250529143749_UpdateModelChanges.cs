using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthApp.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAttachments_Equipment_EquipmentID",
                table: "EquipmentAttachments");

            migrationBuilder.RenameColumn(
                name: "EquipmentID",
                table: "EquipmentAttachments",
                newName: "EquipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAttachments_EquipmentID",
                table: "EquipmentAttachments",
                newName: "IX_EquipmentAttachments_EquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAttachments_Equipment_EquipmentId",
                table: "EquipmentAttachments",
                column: "EquipmentId",
                principalTable: "Equipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAttachments_Equipment_EquipmentId",
                table: "EquipmentAttachments");

            migrationBuilder.RenameColumn(
                name: "EquipmentId",
                table: "EquipmentAttachments",
                newName: "EquipmentID");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentAttachments_EquipmentId",
                table: "EquipmentAttachments",
                newName: "IX_EquipmentAttachments_EquipmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAttachments_Equipment_EquipmentID",
                table: "EquipmentAttachments",
                column: "EquipmentID",
                principalTable: "Equipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
