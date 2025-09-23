using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_DanhBaSms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DanhBaChiTiet",
                schema: "core");

            migrationBuilder.RenameColumn(
                name: "EmailHuce",
                schema: "core",
                table: "DanhBaCungChiTiet",
                newName: "Email");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "core",
                table: "DanhBa",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DanhBaSms",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDanhBa = table.Column<int>(type: "int", nullable: false),
                    HoVaTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaSoNguoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhBaSms", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhBaSms",
                schema: "core",
                table: "DanhBaSms",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DanhBaSms",
                schema: "core");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "core",
                table: "DanhBa");

            migrationBuilder.RenameColumn(
                name: "Email",
                schema: "core",
                table: "DanhBaCungChiTiet",
                newName: "EmailHuce");

            migrationBuilder.CreateTable(
                name: "DanhBaChiTiet",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailHuce = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoVaTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdDanhBa = table.Column<int>(type: "int", nullable: false),
                    MaSoNguoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhBaChiTiet", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhBaChiTiet",
                schema: "core",
                table: "DanhBaChiTiet",
                column: "Id");
        }
    }
}
