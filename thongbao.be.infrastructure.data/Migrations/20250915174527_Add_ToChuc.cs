using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Add_ToChuc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Khoa",
                schema: "core",
                table: "DanhBaCungChiTiet");

            migrationBuilder.DropColumn(
                name: "KhoaSinhVien",
                schema: "core",
                table: "DanhBaCungChiTiet");

            migrationBuilder.DropColumn(
                name: "Lop",
                schema: "core",
                table: "DanhBaCungChiTiet");

            migrationBuilder.DropColumn(
                name: "MaSoKhoa",
                schema: "core",
                table: "DanhBaCungChiTiet");

            migrationBuilder.DropColumn(
                name: "Khoa",
                schema: "core",
                table: "DanhBaChiTiet");

            migrationBuilder.DropColumn(
                name: "KhoaSinhVien",
                schema: "core",
                table: "DanhBaChiTiet");

            migrationBuilder.DropColumn(
                name: "LaNguoiDung",
                schema: "core",
                table: "DanhBaChiTiet");

            migrationBuilder.DropColumn(
                name: "Lop",
                schema: "core",
                table: "DanhBaChiTiet");

            migrationBuilder.DropColumn(
                name: "MaSoKhoa",
                schema: "core",
                table: "DanhBaChiTiet");

            migrationBuilder.CreateTable(
                name: "ToChuc",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenToChuc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiToChuc = table.Column<int>(type: "int", nullable: false),
                    MaSoToChuc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToChuc", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ToChucDanhBaChiTiet",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdToChuc = table.Column<int>(type: "int", nullable: false),
                    IdDanhBaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToChucDanhBaChiTiet", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToChuc",
                schema: "core",
                table: "ToChuc",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ToChucDanhBaChiTiet",
                schema: "core",
                table: "ToChucDanhBaChiTiet",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ToChuc",
                schema: "core");

            migrationBuilder.DropTable(
                name: "ToChucDanhBaChiTiet",
                schema: "core");

            migrationBuilder.AddColumn<string>(
                name: "Khoa",
                schema: "core",
                table: "DanhBaCungChiTiet",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KhoaSinhVien",
                schema: "core",
                table: "DanhBaCungChiTiet",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lop",
                schema: "core",
                table: "DanhBaCungChiTiet",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaSoKhoa",
                schema: "core",
                table: "DanhBaCungChiTiet",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Khoa",
                schema: "core",
                table: "DanhBaChiTiet",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KhoaSinhVien",
                schema: "core",
                table: "DanhBaChiTiet",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LaNguoiDung",
                schema: "core",
                table: "DanhBaChiTiet",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Lop",
                schema: "core",
                table: "DanhBaChiTiet",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaSoKhoa",
                schema: "core",
                table: "DanhBaChiTiet",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
