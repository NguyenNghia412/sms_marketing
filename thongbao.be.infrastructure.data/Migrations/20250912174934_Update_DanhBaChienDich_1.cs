using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_DanhBaChienDich_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "core",
                table: "ChienDichDanhBa",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                schema: "core",
                table: "ChienDichDanhBa",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "getdate()");

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                schema: "core",
                table: "ChienDichDanhBa",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                schema: "core",
                table: "ChienDichDanhBa",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                schema: "core",
                table: "ChienDichDanhBa",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "core",
                table: "ChienDichDanhBa");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                schema: "core",
                table: "ChienDichDanhBa");

            migrationBuilder.DropColumn(
                name: "Deleted",
                schema: "core",
                table: "ChienDichDanhBa");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "core",
                table: "ChienDichDanhBa");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                schema: "core",
                table: "ChienDichDanhBa");
        }
    }
}
