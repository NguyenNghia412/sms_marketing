using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Danh_Ba_ISoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                schema: "core",
                table: "DanhBaSms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                schema: "core",
                table: "DanhBaSms",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                schema: "core",
                table: "DanhBaData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                schema: "core",
                table: "DanhBaData",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "core",
                table: "DanhBaSms");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                schema: "core",
                table: "DanhBaSms");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "core",
                table: "DanhBaData");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                schema: "core",
                table: "DanhBaData");
        }
    }
}
