using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthApp.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEquipmentCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_Equipment_ParentId",
                table: "Equipment");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_Equipment_ParentId",
                table: "Equipment",
                column: "ParentId",
                principalTable: "Equipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_Equipment_ParentId",
                table: "Equipment");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_Equipment_ParentId",
                table: "Equipment",
                column: "ParentId",
                principalTable: "Equipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
