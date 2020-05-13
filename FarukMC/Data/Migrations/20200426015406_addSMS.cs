using Microsoft.EntityFrameworkCore.Migrations;

namespace FarukMC.Data.Migrations
{
    public partial class addSMS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_SMS_SMSId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_SMSId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "SMSId",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "BookingID",
                table: "SMS",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SMS_BookingID",
                table: "SMS",
                column: "BookingID");

            migrationBuilder.AddForeignKey(
                name: "FK_SMS_Bookings_BookingID",
                table: "SMS",
                column: "BookingID",
                principalTable: "Bookings",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SMS_Bookings_BookingID",
                table: "SMS");

            migrationBuilder.DropIndex(
                name: "IX_SMS_BookingID",
                table: "SMS");

            migrationBuilder.DropColumn(
                name: "BookingID",
                table: "SMS");

            migrationBuilder.AddColumn<int>(
                name: "SMSId",
                table: "Bookings",
                type: "int",
                nullable: true);

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
    }
}
