using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Add_DiemDanh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HopTrucTuyen",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenCuocHop = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    BatDauDiemDanh = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    IdCuocHop = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LinkCuocHop = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdTinNhanChung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserIdCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KetThucDiemDanh = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    ThoiGianTaoCuocHop = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HopTrucTuyen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThongTinDiemDanh",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdHopTrucTuyen = table.Column<int>(type: "int", nullable: false),
                    MaSoSinhVien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoVaTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoDem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Khoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LopQuanLy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailHuce = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiDiemDanh = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongTinDiemDanh", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TinNhanHopTrucTuyen",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CuocHopId = table.Column<int>(type: "int", nullable: false),
                    ThongTinDiemDanhId = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGianGui = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinNhanHopTrucTuyen", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HopTrucTuyen",
                schema: "core",
                table: "HopTrucTuyen",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ThongTinDiemDanh",
                schema: "core",
                table: "ThongTinDiemDanh",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhanHopTrucTuyen",
                schema: "core",
                table: "TinNhanHopTrucTuyen",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HopTrucTuyen",
                schema: "core");

            migrationBuilder.DropTable(
                name: "ThongTinDiemDanh",
                schema: "core");

            migrationBuilder.DropTable(
                name: "TinNhanHopTrucTuyen",
                schema: "core");
        }
    }
}
