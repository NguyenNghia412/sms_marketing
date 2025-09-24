using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_GuiTinNhanLogChiTiet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoDienThoai",
                schema: "core",
                table: "GuiTinNhanLogChiTiet");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SoDienThoai",
                schema: "core",
                table: "GuiTinNhanLogChiTiet",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
