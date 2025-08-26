using Microsoft.EntityFrameworkCore.Migrations;

namespace TT.Lib.Migrations
{
    public partial class AddTypeToProductProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ProductProperties",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "ProductProperties");
        }
    }
}
