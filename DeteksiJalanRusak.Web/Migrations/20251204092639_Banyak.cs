using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeteksiJalanRusak.Web.Migrations
{
    /// <inheritdoc />
    public partial class Banyak : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LebarPiksel",
                table: "TblKerusakan",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TinggiPiksel",
                table: "TblKerusakan",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<double>(
                name: "PanjangKertas",
                table: "TblFoto",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Mask64Kertas",
                table: "TblFoto",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "LuasKertas",
                table: "TblFoto",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "LebarKertas",
                table: "TblFoto",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LebarPikselKertas",
                table: "TblFoto",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TinggiPikselKertas",
                table: "TblFoto",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LebarPiksel",
                table: "TblKerusakan");

            migrationBuilder.DropColumn(
                name: "TinggiPiksel",
                table: "TblKerusakan");

            migrationBuilder.DropColumn(
                name: "LebarPikselKertas",
                table: "TblFoto");

            migrationBuilder.DropColumn(
                name: "TinggiPikselKertas",
                table: "TblFoto");

            migrationBuilder.AlterColumn<double>(
                name: "PanjangKertas",
                table: "TblFoto",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<string>(
                name: "Mask64Kertas",
                table: "TblFoto",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<double>(
                name: "LuasKertas",
                table: "TblFoto",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "LebarKertas",
                table: "TblFoto",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");
        }
    }
}
