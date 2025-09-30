using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_DanhBaSms_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaSoNguoiDung",
                schema: "core",
                table: "DanhBaSms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaSoNguoiDung",
                schema: "core",
                table: "DanhBaSms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
