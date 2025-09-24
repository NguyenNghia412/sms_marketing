using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Add_GuiTinNhanLogChiTiet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SmsSendSuccess",
                schema: "core",
                table: "ChienDichLogTrangThaiGui",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GuiTinNhanLogChiTiet",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdChienDich = table.Column<int>(type: "int", nullable: false),
                    IdDanhBa = table.Column<int>(type: "int", nullable: false),
                    IdBrandName = table.Column<int>(type: "int", nullable: false),
                    IdDanhBaSms = table.Column<int>(type: "int", nullable: false),
                    SoDienThoai = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGianGui = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuiTinNhanLogChiTiet", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuiTinNhanLogChiTiet",
                schema: "core",
                table: "GuiTinNhanLogChiTiet",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuiTinNhanLogChiTiet",
                schema: "core");

            migrationBuilder.DropColumn(
                name: "SmsSendSuccess",
                schema: "core",
                table: "ChienDichLogTrangThaiGui");
        }
    }
}
