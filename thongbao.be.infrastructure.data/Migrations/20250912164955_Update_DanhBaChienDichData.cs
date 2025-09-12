using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_DanhBaChienDichData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdThongTinDanhBa",
                schema: "core",
                table: "DanhBaChienDichData",
                newName: "IdDanhBaChienDich");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdDanhBaChienDich",
                schema: "core",
                table: "DanhBaChienDichData",
                newName: "IdThongTinDanhBa");
        }
    }
}
