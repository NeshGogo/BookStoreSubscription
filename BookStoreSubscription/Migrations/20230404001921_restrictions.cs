using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreSubscription.Migrations
{
    public partial class restrictions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DomainRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeyAPIId = table.Column<int>(type: "int", nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DomainRestrictions_KeyAPIs_KeyAPIId",
                        column: x => x.KeyAPIId,
                        principalTable: "KeyAPIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IpRestrictionss",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeyAPIId = table.Column<int>(type: "int", nullable: false),
                    IP = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpRestrictionss", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IpRestrictionss_KeyAPIs_KeyAPIId",
                        column: x => x.KeyAPIId,
                        principalTable: "KeyAPIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DomainRestrictions_KeyAPIId",
                table: "DomainRestrictions",
                column: "KeyAPIId");

            migrationBuilder.CreateIndex(
                name: "IX_IpRestrictionss_KeyAPIId",
                table: "IpRestrictionss",
                column: "KeyAPIId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainRestrictions");

            migrationBuilder.DropTable(
                name: "IpRestrictionss");
        }
    }
}
