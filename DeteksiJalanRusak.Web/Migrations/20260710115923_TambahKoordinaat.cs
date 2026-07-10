using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeteksiJalanRusak.Web.Migrations
{
    /// <inheritdoc />
    public partial class TambahKoordinaat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Lat",
                table: "TblSegmen",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Lng",
                table: "TblSegmen",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lat",
                table: "TblSegmen");

            migrationBuilder.DropColumn(
                name: "Lng",
                table: "TblSegmen");
        }
    }
}
