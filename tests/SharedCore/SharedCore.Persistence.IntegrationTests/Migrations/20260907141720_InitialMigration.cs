using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedCore.Persistence.IntegrationTests.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "testing");

            migrationBuilder.CreateTable(
                name: "TestEntitiesTable",
                schema: "testing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    OwnedEntity_Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    OwnerId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestEntitiesTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestChildEntitiesTable",
                schema: "testing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestChildEntitiesTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestChildEntitiesTable_TestEntitiesTable_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "testing",
                        principalTable: "TestEntitiesTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestChildEntitiesTable_Code",
                schema: "testing",
                table: "TestChildEntitiesTable",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestChildEntitiesTable_ParentId",
                schema: "testing",
                table: "TestChildEntitiesTable",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_TestEntitiesTable_Code",
                schema: "testing",
                table: "TestEntitiesTable",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestChildEntitiesTable",
                schema: "testing");

            migrationBuilder.DropTable(
                name: "TestEntitiesTable",
                schema: "testing");
        }
    }
}
