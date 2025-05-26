using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AuthApp.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentAttachmentsAndRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Equipment",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Executor",
                table: "Equipment",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SZZ",
                table: "Equipment",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EquipmentAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipmentID = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FileSize = table.Column<double>(type: "double precision", nullable: false),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UploadDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentAttachments_Equipment_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsAlive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecurityLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsAlive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentInspectionMethods",
                columns: table => new
                {
                    EquipmentID = table.Column<Guid>(type: "uuid", nullable: false),
                    InspectionMethodId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentInspectionMethods", x => new { x.EquipmentID, x.InspectionMethodId });
                    table.ForeignKey(
                        name: "FK_EquipmentInspectionMethods_Equipment_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentInspectionMethods_InspectionMethods_InspectionMeth~",
                        column: x => x.InspectionMethodId,
                        principalTable: "InspectionMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentSecurityLevels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    SecurityLevelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentSecurityLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentSecurityLevels_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentSecurityLevels_SecurityLevels_SecurityLevelId",
                        column: x => x.SecurityLevelId,
                        principalTable: "SecurityLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAttachments_EquipmentID",
                table: "EquipmentAttachments",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentInspectionMethods_InspectionMethodId",
                table: "EquipmentInspectionMethods",
                column: "InspectionMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSecurityLevels_EquipmentId",
                table: "EquipmentSecurityLevels",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSecurityLevels_SecurityLevelId",
                table: "EquipmentSecurityLevels",
                column: "SecurityLevelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentAttachments");

            migrationBuilder.DropTable(
                name: "EquipmentInspectionMethods");

            migrationBuilder.DropTable(
                name: "EquipmentSecurityLevels");

            migrationBuilder.DropTable(
                name: "InspectionMethods");

            migrationBuilder.DropTable(
                name: "SecurityLevels");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "Executor",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "SZZ",
                table: "Equipment");
        }
    }
}
