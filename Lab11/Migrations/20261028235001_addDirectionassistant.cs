using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventos.Web.Migrations
{
    /// <inheritdoc />
    public partial class addDirectionassistant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Asistentes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Asistentes");
        }
    }
}
