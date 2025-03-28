using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPSRFileUpload.Data.Migrations
{
    /// <inheritdoc />
    public partial class AllMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileConfigDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Vin = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: false),
                    Startdate = table.Column<DateOnly>(type: "date", maxLength: 450, nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    SpgAcn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SpgOrganization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileConfigDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImportedFileNames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportedFileNames", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileConfigDetails_SpgAcn",
                table: "FileConfigDetails",
                column: "SpgAcn",
                unique: true)
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_FileConfigDetails_Vin",
                table: "FileConfigDetails",
                column: "Vin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImportedFileNames_FileName",
                table: "ImportedFileNames",
                column: "FileName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileConfigDetails");

            migrationBuilder.DropTable(
                name: "ImportedFileNames");
        }
    }
}
