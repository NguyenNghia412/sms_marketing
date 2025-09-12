using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Add_DanhBa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DanhBaChienDich",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDanhBa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mota = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhBaChienDich", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhBaChienDichChiTiet",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDanhBa = table.Column<int>(type: "int", nullable: false),
                    HoVaTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhBaChienDichChiTiet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhBaChienDichData",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdTruongData = table.Column<int>(type: "int", nullable: false),
                    IdThongTinDanhBa = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhBaChienDichData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhBaChienDichTruongData",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDanhBa = table.Column<int>(type: "int", nullable: false),
                    TenTruong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TruongImport = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhBaChienDichTruongData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhBaCungChiTiet",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoVaTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoDem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailHuce = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    GioiTinh = table.Column<int>(type: "int", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: true),
                    LaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    MaSoNguoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Khoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaSoKhoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lop = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KhoaSinhVien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiHoatDong = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhBaCungChiTiet", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhBaChienDich",
                schema: "core",
                table: "DanhBaChienDich",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DanhBaChienDichChiTiet",
                schema: "core",
                table: "DanhBaChienDichChiTiet",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DanhBaChienDichData",
                schema: "core",
                table: "DanhBaChienDichData",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DanhBaChienDichTruongData",
                schema: "core",
                table: "DanhBaChienDichTruongData",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DanhBaCungChiTiet",
                schema: "core",
                table: "DanhBaCungChiTiet",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DanhBaChienDich",
                schema: "core");

            migrationBuilder.DropTable(
                name: "DanhBaChienDichChiTiet",
                schema: "core");

            migrationBuilder.DropTable(
                name: "DanhBaChienDichData",
                schema: "core");

            migrationBuilder.DropTable(
                name: "DanhBaChienDichTruongData",
                schema: "core");

            migrationBuilder.DropTable(
                name: "DanhBaCungChiTiet",
                schema: "core");
        }
    }
}
