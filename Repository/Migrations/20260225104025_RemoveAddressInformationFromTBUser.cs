using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAddressInformationFromTBUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "TBUserHistoric");

            migrationBuilder.DropColumn(
                name: "City",
                table: "TBUserHistoric");

            migrationBuilder.DropColumn(
                name: "Complement",
                table: "TBUserHistoric");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "TBUserHistoric");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "TBUserHistoric");

            migrationBuilder.DropColumn(
                name: "State",
                table: "TBUserHistoric");

            migrationBuilder.DropColumn(
                name: "Zipcode",
                table: "TBUserHistoric");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "TBUser");

            migrationBuilder.DropColumn(
                name: "City",
                table: "TBUser");

            migrationBuilder.DropColumn(
                name: "Complement",
                table: "TBUser");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "TBUser");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "TBUser");

            migrationBuilder.DropColumn(
                name: "State",
                table: "TBUser");

            migrationBuilder.DropColumn(
                name: "Zipcode",
                table: "TBUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "TBUserHistoric",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "TBUserHistoric",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Complement",
                table: "TBUserHistoric",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "TBUserHistoric",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "TBUserHistoric",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "TBUserHistoric",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Zipcode",
                table: "TBUserHistoric",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "TBUser",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "TBUser",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Complement",
                table: "TBUser",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "TBUser",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "TBUser",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "TBUser",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Zipcode",
                table: "TBUser",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
