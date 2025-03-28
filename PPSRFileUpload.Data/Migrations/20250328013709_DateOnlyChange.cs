using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fileupload.Data.Migrations
{
    /// <inheritdoc />
    public partial class DateOnlyChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "Startdate",
                table: "FileConfigDetails",
                type: "date",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldMaxLength: 450);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Startdate",
                table: "FileConfigDetails",
                type: "datetime2",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldMaxLength: 450);
        }
    }
}
