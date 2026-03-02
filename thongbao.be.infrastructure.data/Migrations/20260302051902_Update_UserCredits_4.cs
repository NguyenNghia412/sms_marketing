using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_UserCredits_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ToiDaHanMucCreditGiaHan",
                schema: "core",
                table: "UserCredits");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ToiDaHanMucCreditGiaHan",
                schema: "core",
                table: "UserCredits",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
