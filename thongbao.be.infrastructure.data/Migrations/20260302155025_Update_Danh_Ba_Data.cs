using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Danh_Ba_Data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdDanhBaChienDich",
                schema: "core",
                table: "DanhBaData",
                newName: "IdDanhBa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdDanhBa",
                schema: "core",
                table: "DanhBaData",
                newName: "IdDanhBaChienDich");
        }
    }
}
