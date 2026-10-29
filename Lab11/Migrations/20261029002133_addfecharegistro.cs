using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventos.Web.Migrations
{
    /// <inheritdoc />
    public partial class addfecharegistro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Asistentes",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Asistentes");
        }
    }
}
