using Microsoft.EntityFrameworkCore.Migrations;

namespace Jessamine.Server.Migrations
{
    public partial class lasstmessagestatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastMessageRead",
                table: "Conversations");

            migrationBuilder.AddColumn<int>(
                name: "LastMessageStatus",
                table: "Conversations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastMessageStatus",
                table: "Conversations");

            migrationBuilder.AddColumn<bool>(
                name: "LastMessageRead",
                table: "Conversations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
