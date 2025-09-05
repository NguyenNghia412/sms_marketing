using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Add_ThongTinDiemDanh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ThoiHanDiemDanh",
                schema: "core",
                table: "HopTrucTuyen",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ThongTinDiemDanh",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCuocHop = table.Column<int>(type: "int", nullable: false),
                    MaSoSinhVien = table.Column<int>(type: "int", nullable: false),
                    HoVaTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoDem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Khoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LopQuanLy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailHuce = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TinNhan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiDiemDanh = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongTinDiemDanh", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThongTinDiemDanh",
                schema: "core",
                table: "ThongTinDiemDanh",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThongTinDiemDanh",
                schema: "core");

            migrationBuilder.DropColumn(
                name: "ThoiHanDiemDanh",
                schema: "core",
                table: "HopTrucTuyen");
        }
    }
}
