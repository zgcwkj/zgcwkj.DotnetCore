using Microsoft.EntityFrameworkCore.Migrations;

namespace zgcwkj.Model.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sys_user",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    user_name = table.Column<string>(type: "text", nullable: true),
                    password = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_user", x => x.user_id);
                });

            migrationBuilder.InsertData(
                table: "sys_user",
                columns: new[] { "user_id", "password", "user_name" },
                values: new object[] { "d5916dc8cb46ccc35026a9c54fbf16e7", "Password", "UserName" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sys_user");
        }
    }
}
