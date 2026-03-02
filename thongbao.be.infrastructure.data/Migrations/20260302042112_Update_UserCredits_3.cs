using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_UserCredits_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ToiDaHanMucCreditGiaHan",
                schema: "core",
                table: "UserCredits",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ToiDaHanMucCreditGiaHan",
                schema: "core",
                table: "UserCredits");
        }
    }
}
