using Microsoft.EntityFrameworkCore.Migrations;

namespace BattleAuth.Api.Migrations
{
    public partial class CustomUserAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "UserTokens",
                type: "varchar(255) CHARACTER SET latin1",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128) CHARACTER SET latin1",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "UserTokens",
                type: "varchar(255) CHARACTER SET latin1",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128) CHARACTER SET latin1",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "UserLogins",
                type: "varchar(255) CHARACTER SET latin1",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128) CHARACTER SET latin1",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "UserLogins",
                type: "varchar(255) CHARACTER SET latin1",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128) CHARACTER SET latin1",
                oldMaxLength: 128);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "UserTokens",
                type: "varchar(128) CHARACTER SET latin1",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET latin1");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "UserTokens",
                type: "varchar(128) CHARACTER SET latin1",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET latin1");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "UserLogins",
                type: "varchar(128) CHARACTER SET latin1",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET latin1");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "UserLogins",
                type: "varchar(128) CHARACTER SET latin1",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET latin1");
        }
    }
}
