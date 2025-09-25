using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_GuiTinNhanLogChiTiet_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrangThai",
                schema: "core",
                table: "GuiTinNhanLogChiTiet",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SmsSendFailed",
                schema: "core",
                table: "ChienDichLogTrangThaiGui",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TrangThai",
                schema: "core",
                table: "ChienDichLogTrangThaiGui",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrangThai",
                schema: "core",
                table: "GuiTinNhanLogChiTiet");

            migrationBuilder.DropColumn(
                name: "SmsSendFailed",
                schema: "core",
                table: "ChienDichLogTrangThaiGui");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                schema: "core",
                table: "ChienDichLogTrangThaiGui");
        }
    }
}
