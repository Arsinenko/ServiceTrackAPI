using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthApp.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EquipmentRefactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_Equipment_ParentEquipmentId",
                table: "Equipment");

            migrationBuilder.RenameColumn(
                name: "ParentEquipmentId",
                table: "Equipment",
                newName: "ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Equipment_ParentEquipmentId",
                table: "Equipment",
                newName: "IX_Equipment_ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_Equipment_ParentId",
                table: "Equipment",
                column: "ParentId",
                principalTable: "Equipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_Equipment_ParentId",
                table: "Equipment");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "Equipment",
                newName: "ParentEquipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Equipment_ParentId",
                table: "Equipment",
                newName: "IX_Equipment_ParentEquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_Equipment_ParentEquipmentId",
                table: "Equipment",
                column: "ParentEquipmentId",
                principalTable: "Equipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
