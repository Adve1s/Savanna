using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Savanna.WebUI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWorldSaves : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorldSaves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SaveName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SaveTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Iteration = table.Column<int>(type: "int", nullable: false),
                    AnimalCount = table.Column<int>(type: "int", nullable: false),
                    SaveData = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldSaves", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorldSaves");
        }
    }
}
