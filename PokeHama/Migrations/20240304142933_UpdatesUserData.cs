using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeHama.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesUserData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ImagePfp",
                table: "UsersData",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePfp",
                table: "UsersData");
        }
    }
}
