using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class UserAddressTableMigrationV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TBUserAddress",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TBUserAddress_UserId",
                table: "TBUserAddress",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBUserAddress_TBUser_UserId",
                table: "TBUserAddress",
                column: "UserId",
                principalTable: "TBUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBUserAddress_TBUser_UserId",
                table: "TBUserAddress");

            migrationBuilder.DropIndex(
                name: "IX_TBUserAddress_UserId",
                table: "TBUserAddress");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TBUserAddress");
        }
    }
}
