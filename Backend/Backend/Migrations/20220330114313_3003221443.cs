using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class _3003221443 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "DesignProject",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "DesignProject");
        }
    }
}
