using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Add_MauNoiDung_LogTrangThaiGui_BrandName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiToChuc",
                schema: "core",
                table: "ToChuc");

            migrationBuilder.DropColumn(
                name: "TruongImport",
                schema: "core",
                table: "DanhBaTruongData");

            migrationBuilder.DropColumn(
                name: "NoiDung",
                schema: "core",
                table: "ChienDich");

            migrationBuilder.AddColumn<int>(
                name: "IdBrandName",
                schema: "core",
                table: "ChienDich",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BrandName",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenBrandName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mota = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandName", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChienDichLogTrangThaiGui",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdChienDich = table.Column<int>(type: "int", nullable: false),
                    IdDanhBa = table.Column<int>(type: "int", nullable: false),
                    IdBrandName = table.Column<int>(type: "int", nullable: false),
                    ThoiGianGui = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChienDichLogTrangThaiGui", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MauNoiDung",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MauNoiDung", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChienDichLogTrangThaiGui",
                schema: "core",
                table: "ChienDichLogTrangThaiGui",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandName",
                schema: "core");

            migrationBuilder.DropTable(
                name: "ChienDichLogTrangThaiGui",
                schema: "core");

            migrationBuilder.DropTable(
                name: "MauNoiDung",
                schema: "core");

            migrationBuilder.DropColumn(
                name: "IdBrandName",
                schema: "core",
                table: "ChienDich");

            migrationBuilder.AddColumn<int>(
                name: "LoaiToChuc",
                schema: "core",
                table: "ToChuc",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TruongImport",
                schema: "core",
                table: "DanhBaTruongData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NoiDung",
                schema: "core",
                table: "ChienDich",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");
        }
    }
}
