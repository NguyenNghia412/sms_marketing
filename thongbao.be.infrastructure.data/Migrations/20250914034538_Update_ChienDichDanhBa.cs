using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thongbao.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_ChienDichDanhBa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "PK_ChienDichDanhBa",
                schema: "core",
                table: "ChienDichDanhBa",
                columns: new[] { "IdChienDich", "IdDanhBa" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ChienDichDanhBa",
                schema: "core",
                table: "ChienDichDanhBa");
        }
    }
}
