using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeteksiJalanRusak.Web.Migrations
{
    /// <inheritdoc />
    public partial class UbahNamaPenggunaJadiPembuat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NamaPengguna",
                table: "TblAnalisis",
                newName: "Pembuat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Pembuat",
                table: "TblAnalisis",
                newName: "NamaPengguna");
        }
    }
}
