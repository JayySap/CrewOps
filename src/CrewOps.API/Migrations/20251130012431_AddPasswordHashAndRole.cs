using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrewOps.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordHashAndRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "CrewMembers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "CrewMembers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "CrewMembers");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "CrewMembers");
        }
    }
}
