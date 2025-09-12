using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_DanhBa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailHuce",
                schema: "core",
                table: "DanhBaChienDichChiTiet",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Khoa",
                schema: "core",
                table: "DanhBaChienDichChiTiet",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KhoaSinhVien",
                schema: "core",
                table: "DanhBaChienDichChiTiet",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LaNguoiDung",
                schema: "core",
                table: "DanhBaChienDichChiTiet",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Lop",
                schema: "core",
                table: "DanhBaChienDichChiTiet",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaSoKhoa",
                schema: "core",
                table: "DanhBaChienDichChiTiet",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MaSoNguoiDung",
                schema: "core",
                table: "DanhBaChienDichChiTiet",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailHuce",
                schema: "core",
                table: "DanhBaChienDichChiTiet");

            migrationBuilder.DropColumn(
                name: "Khoa",
                schema: "core",
                table: "DanhBaChienDichChiTiet");

            migrationBuilder.DropColumn(
                name: "KhoaSinhVien",
                schema: "core",
                table: "DanhBaChienDichChiTiet");

            migrationBuilder.DropColumn(
                name: "LaNguoiDung",
                schema: "core",
                table: "DanhBaChienDichChiTiet");

            migrationBuilder.DropColumn(
                name: "Lop",
                schema: "core",
                table: "DanhBaChienDichChiTiet");

            migrationBuilder.DropColumn(
                name: "MaSoKhoa",
                schema: "core",
                table: "DanhBaChienDichChiTiet");

            migrationBuilder.DropColumn(
                name: "MaSoNguoiDung",
                schema: "core",
                table: "DanhBaChienDichChiTiet");
        }
    }
}
