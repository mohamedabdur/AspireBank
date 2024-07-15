using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sample.Migrations
{
    public partial class IFSCcodeadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitialDepositAmount",
                table: "CustomerAccountInfos");

            migrationBuilder.AddColumn<string>(
                name: "IFSCCode",
                table: "CustomerAccountInfos",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IFSCCode",
                table: "CustomerAccountInfos");

            migrationBuilder.AddColumn<decimal>(
                name: "InitialDepositAmount",
                table: "CustomerAccountInfos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
