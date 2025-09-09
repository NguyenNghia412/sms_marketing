using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Add_DotDiemDanh_GhiNhanDiemDanh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DotDiemDanh",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDotDiemDanh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenMonHoc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaMonHoc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotDiemDanh", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GhiNhanDiemDanh",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDotDiemDanh = table.Column<int>(type: "int", nullable: false),
                    EmailHuce = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoVaTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGianDiemDanh = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    TrangThaiHoanTatDiemDanh = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GhiNhanDiemDanh", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DotDiemDanh",
                schema: "core",
                table: "DotDiemDanh",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GhiNhanDiemDanh",
                schema: "core",
                table: "GhiNhanDiemDanh",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DotDiemDanh",
                schema: "core");

            migrationBuilder.DropTable(
                name: "GhiNhanDiemDanh",
                schema: "core");
        }
    }
}
