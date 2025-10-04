using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_TraoBang_DanhSachSinhVienNhanBang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TruongKhoa",
                schema: "tb",
                table: "SubPlan",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CapBang",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HoVaTen",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KhoaQuanLy",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Lop",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayQuyetDinh",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "NgaySinh",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SoQuyetDinhTotNghiep",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenNganhDaoTao",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ThanhTich",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "XepHang",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TruongKhoa",
                schema: "tb",
                table: "SubPlan");

            migrationBuilder.DropColumn(
                name: "CapBang",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang");

            migrationBuilder.DropColumn(
                name: "HoVaTen",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang");

            migrationBuilder.DropColumn(
                name: "KhoaQuanLy",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang");

            migrationBuilder.DropColumn(
                name: "Lop",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang");

            migrationBuilder.DropColumn(
                name: "NgayQuyetDinh",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang");

            migrationBuilder.DropColumn(
                name: "NgaySinh",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang");

            migrationBuilder.DropColumn(
                name: "SoQuyetDinhTotNghiep",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang");

            migrationBuilder.DropColumn(
                name: "TenNganhDaoTao",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang");

            migrationBuilder.DropColumn(
                name: "ThanhTich",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang");

            migrationBuilder.DropColumn(
                name: "XepHang",
                schema: "tb",
                table: "DanhSachSinhVienNhanBang");
        }
    }
}
