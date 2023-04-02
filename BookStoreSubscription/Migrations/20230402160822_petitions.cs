using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreSubscription.Migrations
{
    public partial class petitions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Petitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeyAPIId = table.Column<int>(type: "int", nullable: false),
                    PetitionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Petitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Petitions_KeyAPIs_KeyAPIId",
                        column: x => x.KeyAPIId,
                        principalTable: "KeyAPIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Petitions_KeyAPIId",
                table: "Petitions",
                column: "KeyAPIId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Petitions");
        }
    }
}
