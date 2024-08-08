using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GetFitApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFitnessClassTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "FitnessClasses",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "FitnessClasses",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "FitnessClasses");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "FitnessClasses");
        }
    }
}
