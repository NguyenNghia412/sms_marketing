using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Toi_Da_Credit_GiaHan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ToiDaHanMucCreditsGiaHan",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ToiDaHanMucCreditGiaHan = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_ToiDaHanMucCreditsGiaHan", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToiDaHanMucCreditsGiaHan",
                schema: "core",
                table: "ToiDaHanMucCreditsGiaHan",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ToiDaHanMucCreditsGiaHan",
                schema: "core");
        }
    }
}
