using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DeteksiJalanRusak.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblAnalisis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nama = table.Column<string>(type: "text", nullable: false),
                    Lokasi = table.Column<string>(type: "text", nullable: false),
                    NamaPengguna = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblAnalisis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblSegmen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LuasSampel = table.Column<double>(type: "double precision", nullable: false),
                    AnalisisId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblSegmen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblSegmen_TblAnalisis_AnalisisId",
                        column: x => x.AnalisisId,
                        principalTable: "TblAnalisis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblFoto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    ImageBase64 = table.Column<string>(type: "text", nullable: false),
                    Lebar = table.Column<double>(type: "double precision", nullable: false),
                    Tinggi = table.Column<double>(type: "double precision", nullable: false),
                    M2PerPiksel = table.Column<double>(type: "double precision", nullable: false),
                    MPerPiksel = table.Column<double>(type: "double precision", nullable: false),
                    AdaKertas = table.Column<bool>(type: "boolean", nullable: false),
                    Mask64Kertas = table.Column<string>(type: "text", nullable: true),
                    LuasKertas = table.Column<double>(type: "double precision", nullable: true),
                    PanjangKertas = table.Column<double>(type: "double precision", nullable: true),
                    LebarKertas = table.Column<double>(type: "double precision", nullable: true),
                    SegmenId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblFoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblFoto_TblSegmen_SegmenId",
                        column: x => x.SegmenId,
                        principalTable: "TblSegmen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblKerusakan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaskBase64 = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<int>(type: "integer", nullable: false),
                    Luas = table.Column<double>(type: "double precision", nullable: false),
                    Panjang = table.Column<double>(type: "double precision", nullable: false),
                    Lebar = table.Column<double>(type: "double precision", nullable: false),
                    FotoKerusakanId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblKerusakan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblKerusakan_TblFoto_FotoKerusakanId",
                        column: x => x.FotoKerusakanId,
                        principalTable: "TblFoto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblFoto_SegmenId",
                table: "TblFoto",
                column: "SegmenId");

            migrationBuilder.CreateIndex(
                name: "IX_TblKerusakan_FotoKerusakanId",
                table: "TblKerusakan",
                column: "FotoKerusakanId");

            migrationBuilder.CreateIndex(
                name: "IX_TblSegmen_AnalisisId",
                table: "TblSegmen",
                column: "AnalisisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblKerusakan");

            migrationBuilder.DropTable(
                name: "TblFoto");

            migrationBuilder.DropTable(
                name: "TblSegmen");

            migrationBuilder.DropTable(
                name: "TblAnalisis");
        }
    }
}
