using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_ConfigDonGia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NhaMang",
                schema: "core",
                table: "CauHinhDonGia");

            migrationBuilder.AddColumn<int>(
                name: "IdNhaMang",
                schema: "core",
                table: "CauHinhDonGia",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Credits",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThoiHanApDungCredits = table.Column<int>(type: "int", nullable: false),
                    DonVi = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Credits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NhaMang",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNhaMang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_NhaMang", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserCredits",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HanMucCredit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGianBatDauApDungHanMuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianKetThucApDungHanMuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreditDaSuDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreditChuaSuDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreditConSauKhiKetThucThoiGianApDungHanMuc = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_UserCredits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Credits",
                schema: "core",
                table: "Credits",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_NhaMang",
                schema: "core",
                table: "NhaMang",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserCredits",
                schema: "core",
                table: "UserCredits",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Credits",
                schema: "core");

            migrationBuilder.DropTable(
                name: "NhaMang",
                schema: "core");

            migrationBuilder.DropTable(
                name: "UserCredits",
                schema: "core");

            migrationBuilder.DropColumn(
                name: "IdNhaMang",
                schema: "core",
                table: "CauHinhDonGia");

            migrationBuilder.AddColumn<string>(
                name: "NhaMang",
                schema: "core",
                table: "CauHinhDonGia",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
