using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_TraoBang_SubPlan_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsShowKetBai",
                schema: "tb",
                table: "SubPlan",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowMoBai",
                schema: "tb",
                table: "SubPlan",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShowKetBai",
                schema: "tb",
                table: "SubPlan");

            migrationBuilder.DropColumn(
                name: "IsShowMoBai",
                schema: "tb",
                table: "SubPlan");
        }
    }
}
