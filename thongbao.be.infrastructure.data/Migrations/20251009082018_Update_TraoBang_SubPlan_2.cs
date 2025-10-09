using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_TraoBang_SubPlan_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KetBaiNote",
                schema: "tb",
                table: "SubPlan",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MoBaiNote",
                schema: "tb",
                table: "SubPlan",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KetBaiNote",
                schema: "tb",
                table: "SubPlan");

            migrationBuilder.DropColumn(
                name: "MoBaiNote",
                schema: "tb",
                table: "SubPlan");
        }
    }
}
