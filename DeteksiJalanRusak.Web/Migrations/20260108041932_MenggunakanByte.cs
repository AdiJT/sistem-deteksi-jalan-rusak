using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeteksiJalanRusak.Web.Migrations
{
    /// <inheritdoc />
    public partial class MenggunakanByte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaskBase64",
                table: "TblKerusakan");

            migrationBuilder.DropColumn(
                name: "ImageBase64",
                table: "TblFoto");

            migrationBuilder.DropColumn(
                name: "Mask64Kertas",
                table: "TblFoto");

            migrationBuilder.AddColumn<byte[]>(
                name: "Mask",
                table: "TblKerusakan",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "TblFoto",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "MaskKertas",
                table: "TblFoto",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mask",
                table: "TblKerusakan");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "TblFoto");

            migrationBuilder.DropColumn(
                name: "MaskKertas",
                table: "TblFoto");

            migrationBuilder.AddColumn<string>(
                name: "MaskBase64",
                table: "TblKerusakan",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageBase64",
                table: "TblFoto",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Mask64Kertas",
                table: "TblFoto",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
