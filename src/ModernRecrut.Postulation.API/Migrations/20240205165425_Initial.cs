using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModernRecrut.Postulation.API.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Postulation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidatId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OffreDEmploiId = table.Column<int>(type: "int", nullable: false),
                    PretentionSalariale = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateDisponibilite = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postulation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Note",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoteDetail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomEmeteur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostulationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Note", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Note_Postulation_PostulationId",
                        column: x => x.PostulationId,
                        principalTable: "Postulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Note_PostulationId",
                table: "Note",
                column: "PostulationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Note");

            migrationBuilder.DropTable(
                name: "Postulation");
        }
    }
}
