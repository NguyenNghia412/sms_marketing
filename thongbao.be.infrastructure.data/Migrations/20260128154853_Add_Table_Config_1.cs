using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Table_Config_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiApiCredit",
                schema: "core",
                table: "UserCredits");

            migrationBuilder.RenameColumn(
                name: "ThoiGianTao",
                schema: "core",
                table: "BrandName",
                newName: "ThoiGianKetThucHoatDong");

            migrationBuilder.RenameColumn(
                name: "ThoiGianKetThuc",
                schema: "core",
                table: "BrandName",
                newName: "ThoiGianBatDauHoatDong");

            migrationBuilder.AddColumn<int>(
                name: "IdNhaCungCapDichVu",
                schema: "core",
                table: "CauHinhDonGia",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdNhaCungCapDichVu",
                schema: "core",
                table: "BrandName",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReportUserCredits",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HanMucCredit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGianBatDauApDungHanMuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianKetThucApDungHanMuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreditDaSuDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditChuaSuDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditConSauKhiKetThucThoiGianApDungHanMuc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonVi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportUserCredits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserNhaCungCapDichVu",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdNhaCungCapDichVu = table.Column<int>(type: "int", nullable: false),
                    IdBrandName = table.Column<int>(type: "int", nullable: false),
                    ThoiGianBatDauSuDungDichVu = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThoiGianKetThucSuDungDichVu = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNhaCungCapDichVu", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportUserCredits",
                schema: "core",
                table: "ReportUserCredits",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserNhaCungCapDichVu",
                schema: "core",
                table: "UserNhaCungCapDichVu",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportUserCredits",
                schema: "core");

            migrationBuilder.DropTable(
                name: "UserNhaCungCapDichVu",
                schema: "core");

            migrationBuilder.DropColumn(
                name: "IdNhaCungCapDichVu",
                schema: "core",
                table: "CauHinhDonGia");

            migrationBuilder.DropColumn(
                name: "IdNhaCungCapDichVu",
                schema: "core",
                table: "BrandName");

            migrationBuilder.RenameColumn(
                name: "ThoiGianKetThucHoatDong",
                schema: "core",
                table: "BrandName",
                newName: "ThoiGianTao");

            migrationBuilder.RenameColumn(
                name: "ThoiGianBatDauHoatDong",
                schema: "core",
                table: "BrandName",
                newName: "ThoiGianKetThuc");

            migrationBuilder.AddColumn<int>(
                name: "LoaiApiCredit",
                schema: "core",
                table: "UserCredits",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
