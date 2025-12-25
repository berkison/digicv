using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simple.Migrations
{
    /// <inheritdoc />
    public partial class AddPrivacySetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProfilePublic",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProfilePublic",
                table: "AspNetUsers");
        }
    }
}
