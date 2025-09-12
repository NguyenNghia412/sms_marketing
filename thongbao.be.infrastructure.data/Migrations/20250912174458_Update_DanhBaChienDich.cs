using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_DanhBaChienDich : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "ChienDichDanhBa",
                schema: "core",
                columns: table => new
                {
                    IdChienDich = table.Column<int>(type: "int", nullable: false),
                    IdDanhBa = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "DanhBa",
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
                    table.PrimaryKey("PK_DanhBa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhBaChiTiet",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDanhBa = table.Column<int>(type: "int", nullable: false),
                    HoVaTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaSoNguoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailHuce = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Khoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaSoKhoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    Lop = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KhoaSinhVien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhBaChiTiet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhBaData",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdTruongData = table.Column<int>(type: "int", nullable: false),
                    IdDanhBaChienDich = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhBaData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhBaTruongData",
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
                    table.PrimaryKey("PK_DanhBaTruongData", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhBa",
                schema: "core",
                table: "DanhBa",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DanhBaChiTiet",
                schema: "core",
                table: "DanhBaChiTiet",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DanhBaData",
                schema: "core",
                table: "DanhBaData",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DanhBaTruongData",
                schema: "core",
                table: "DanhBaTruongData",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChienDichDanhBa",
                schema: "core");

            migrationBuilder.DropTable(
                name: "DanhBa",
                schema: "core");

            migrationBuilder.DropTable(
                name: "DanhBaChiTiet",
                schema: "core");

            migrationBuilder.DropTable(
                name: "DanhBaData",
                schema: "core");

            migrationBuilder.DropTable(
                name: "DanhBaTruongData",
                schema: "core");

            migrationBuilder.CreateTable(
                name: "DanhBaChienDich",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mota = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenDanhBa = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailHuce = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoVaTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdDanhBa = table.Column<int>(type: "int", nullable: false),
                    Khoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KhoaSinhVien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    Lop = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaSoKhoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaSoNguoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdDanhBaChienDich = table.Column<int>(type: "int", nullable: false),
                    IdTruongData = table.Column<int>(type: "int", nullable: false)
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
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdDanhBa = table.Column<int>(type: "int", nullable: false),
                    TenTruong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TruongImport = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhBaChienDichTruongData", x => x.Id);
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
        }
    }
}
