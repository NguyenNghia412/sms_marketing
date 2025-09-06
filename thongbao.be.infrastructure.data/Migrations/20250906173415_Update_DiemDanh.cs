using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_DiemDanh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MsTeamConfig",
                schema: "core");

            migrationBuilder.DropColumn(
                name: "ThoiHanDiemDanh",
                schema: "core",
                table: "HopTrucTuyen");

            migrationBuilder.RenameColumn(
                name: "ThoiGianDiemDanh",
                schema: "core",
                table: "HopTrucTuyen",
                newName: "KetThucDiemDanh");

            migrationBuilder.AddColumn<DateTime>(
                name: "BatDauDiemDanh",
                schema: "core",
                table: "HopTrucTuyen",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "getdate()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BatDauDiemDanh",
                schema: "core",
                table: "HopTrucTuyen");

            migrationBuilder.RenameColumn(
                name: "KetThucDiemDanh",
                schema: "core",
                table: "HopTrucTuyen",
                newName: "ThoiGianDiemDanh");

            migrationBuilder.AddColumn<int>(
                name: "ThoiHanDiemDanh",
                schema: "core",
                table: "HopTrucTuyen",
                type: "int",
                nullable: true);

            
        }
    }
}
