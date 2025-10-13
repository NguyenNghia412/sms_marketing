using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_SmsLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdDanhBa",
                schema: "core",
                table: "GuiTinNhanLogChiTiet",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "NoiDungChiTiet",
                schema: "core",
                table: "GuiTinNhanLogChiTiet",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "IdDanhBa",
                schema: "core",
                table: "ChienDichLogTrangThaiGui",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "NoiDung",
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
                name: "NoiDungChiTiet",
                schema: "core",
                table: "GuiTinNhanLogChiTiet");

            migrationBuilder.DropColumn(
                name: "NoiDung",
                schema: "core",
                table: "ChienDichLogTrangThaiGui");

            migrationBuilder.AlterColumn<int>(
                name: "IdDanhBa",
                schema: "core",
                table: "GuiTinNhanLogChiTiet",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdDanhBa",
                schema: "core",
                table: "ChienDichLogTrangThaiGui",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
