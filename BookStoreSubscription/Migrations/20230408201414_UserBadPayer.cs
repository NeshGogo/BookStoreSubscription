using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreSubscription.Migrations
{
    public partial class UserBadPayer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IpRestrictionss_KeyAPIs_KeyAPIId",
                table: "IpRestrictionss");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IpRestrictionss",
                table: "IpRestrictionss");

            migrationBuilder.RenameTable(
                name: "IpRestrictionss",
                newName: "IpRestrictions");

            migrationBuilder.RenameIndex(
                name: "IX_IpRestrictionss_KeyAPIId",
                table: "IpRestrictions",
                newName: "IX_IpRestrictions_KeyAPIId");

            migrationBuilder.AddColumn<bool>(
                name: "BadPayer",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IpRestrictions",
                table: "IpRestrictions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IpRestrictions_KeyAPIs_KeyAPIId",
                table: "IpRestrictions",
                column: "KeyAPIId",
                principalTable: "KeyAPIs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IpRestrictions_KeyAPIs_KeyAPIId",
                table: "IpRestrictions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IpRestrictions",
                table: "IpRestrictions");

            migrationBuilder.DropColumn(
                name: "BadPayer",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "IpRestrictions",
                newName: "IpRestrictionss");

            migrationBuilder.RenameIndex(
                name: "IX_IpRestrictions_KeyAPIId",
                table: "IpRestrictionss",
                newName: "IX_IpRestrictionss_KeyAPIId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IpRestrictionss",
                table: "IpRestrictionss",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IpRestrictionss_KeyAPIs_KeyAPIId",
                table: "IpRestrictionss",
                column: "KeyAPIId",
                principalTable: "KeyAPIs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
