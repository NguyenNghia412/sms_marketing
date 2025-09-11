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
            migrationBuilder.DropColumn(
                name: "HoVaTen",
                schema: "core",
                table: "DanhBa");

            migrationBuilder.RenameColumn(
                name: "SoDienThoai",
                schema: "core",
                table: "DanhBa",
                newName: "TenDanhBa");

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                schema: "core",
                table: "DanhBa",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mota",
                schema: "core",
                table: "DanhBa",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GhiChu",
                schema: "core",
                table: "DanhBa");

            migrationBuilder.DropColumn(
                name: "Mota",
                schema: "core",
                table: "DanhBa");

            migrationBuilder.RenameColumn(
                name: "TenDanhBa",
                schema: "core",
                table: "DanhBa",
                newName: "SoDienThoai");

            migrationBuilder.AddColumn<string>(
                name: "HoVaTen",
                schema: "core",
                table: "DanhBa",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
