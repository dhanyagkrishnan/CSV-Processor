using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CsvProcessor.Shared.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFileProcessAndCsvRow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastProcessedRowIndex",
                table: "FileProcesses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RowData",
                table: "CsvRows",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastProcessedRowIndex",
                table: "FileProcesses");

            migrationBuilder.DropColumn(
                name: "RowData",
                table: "CsvRows");
        }
    }
}
