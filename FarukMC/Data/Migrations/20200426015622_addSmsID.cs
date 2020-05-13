using Microsoft.EntityFrameworkCore.Migrations;

namespace FarukMC.Data.Migrations
{
    public partial class addSmsID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SMS_Bookings_BookingID",
                table: "SMS");

            migrationBuilder.RenameColumn(
                name: "BookingID",
                table: "SMS",
                newName: "BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_SMS_BookingID",
                table: "SMS",
                newName: "IX_SMS_BookingId");

            migrationBuilder.AlterColumn<int>(
                name: "BookingId",
                table: "SMS",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SMS_Bookings_BookingId",
                table: "SMS",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SMS_Bookings_BookingId",
                table: "SMS");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "SMS",
                newName: "BookingID");

            migrationBuilder.RenameIndex(
                name: "IX_SMS_BookingId",
                table: "SMS",
                newName: "IX_SMS_BookingID");

            migrationBuilder.AlterColumn<int>(
                name: "BookingID",
                table: "SMS",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_SMS_Bookings_BookingID",
                table: "SMS",
                column: "BookingID",
                principalTable: "Bookings",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
