using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FarukMC.Data.Migrations
{
    public partial class SMS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SMSId",
                table: "Bookings",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(name: "Created By", nullable: true),
                    ModifiedBy = table.Column<string>(name: "Modified By", nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMS", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SMSId",
                table: "Bookings",
                column: "SMSId",
                unique: true,
                filter: "[SMSId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_SMS_SMSId",
                table: "Bookings",
                column: "SMSId",
                principalTable: "SMS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_SMS_SMSId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "SMS");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_SMSId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "SMSId",
                table: "Bookings");
        }
    }
}
