using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MiPrimerWebApiM3.Migrations
{
    public partial class cambioAutor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "Autores",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Identificacion",
                table: "Autores",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "Autores");

            migrationBuilder.DropColumn(
                name: "Identificacion",
                table: "Autores");

        }
    }
}
