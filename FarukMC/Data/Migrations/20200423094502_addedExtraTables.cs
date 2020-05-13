using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FarukMC.Data.Migrations
{
    public partial class addedExtraTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AnesthesiaTechnique",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(name: "Created By", nullable: true),
                    ModifiedBy = table.Column<string>(name: "Modified By", nullable: true),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnesthesiaTechnique", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Anesthetics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(name: "Created By", nullable: true),
                    ModifiedBy = table.Column<string>(name: "Modified By", nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anesthetics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(name: "Created By", nullable: true),
                    ModifiedBy = table.Column<string>(name: "Modified By", nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Quantity = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SurgeryRoom",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(name: "Created By", nullable: true),
                    ModifiedBy = table.Column<string>(name: "Modified By", nullable: true),
                    SurgeryRoomDescription = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurgeryRoom", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SurgicalDepartment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(name: "Created By", nullable: true),
                    ModifiedBy = table.Column<string>(name: "Modified By", nullable: true),
                    SurgicalDepartmentDescription = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurgicalDepartment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(name: "Created By", nullable: true),
                    ModifiedBy = table.Column<string>(name: "Modified By", nullable: true),
                    SurgeryStartingTime = table.Column<DateTime>(nullable: false),
                    SurgeryEndingTime = table.Column<DateTime>(nullable: false),
                    SurgeryRoomId = table.Column<int>(nullable: false),
                    PatientFullName = table.Column<string>(nullable: false),
                    PatientDateOfBirth = table.Column<DateTime>(nullable: false),
                    PatientGender = table.Column<byte>(nullable: false),
                    PatientMRNnumber = table.Column<long>(nullable: false),
                    SurgeryName = table.Column<string>(nullable: false),
                    SurgerySite = table.Column<string>(nullable: true),
                    SurgicalDepartmentId = table.Column<int>(nullable: false),
                    SurgeonID = table.Column<string>(nullable: false),
                    BloodRequested = table.Column<bool>(nullable: false),
                    BloodRequestedText = table.Column<string>(nullable: true),
                    RequestedPostOperativeCare = table.Column<bool>(nullable: false),
                    SurgeryPosition = table.Column<bool>(nullable: false),
                    SurgeryPositionText = table.Column<string>(nullable: true),
                    FrozenSection = table.Column<bool>(nullable: false),
                    SpecialThingsLikeSutures = table.Column<bool>(nullable: false),
                    SpecialThingsLikeSuturesText = table.Column<string>(nullable: true),
                    Consumables = table.Column<bool>(nullable: false),
                    ConsumablesText = table.Column<string>(nullable: true),
                    AnesthesiaTechniqueId = table.Column<int>(nullable: false),
                    SpecialDevices = table.Column<bool>(nullable: false),
                    SpecialDevicesText = table.Column<string>(nullable: true),
                    Turniquet = table.Column<bool>(nullable: false),
                    CArm = table.Column<bool>(nullable: false),
                    Harmonic = table.Column<bool>(nullable: false),
                    Ligasure = table.Column<bool>(nullable: false),
                    Microscope = table.Column<bool>(nullable: false),
                    Others = table.Column<string>(nullable: true),
                    AnestheticsId = table.Column<int>(nullable: true),
                    AppointmentStatus = table.Column<byte>(nullable: false),
                    PatientStatus = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Bookings_AnesthesiaTechnique_AnesthesiaTechniqueId",
                        column: x => x.AnesthesiaTechniqueId,
                        principalTable: "AnesthesiaTechnique",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Anesthetics_AnestheticsId",
                        column: x => x.AnestheticsId,
                        principalTable: "Anesthetics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_AspNetUsers_SurgeonID",
                        column: x => x.SurgeonID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_SurgeryRoom_SurgeryRoomId",
                        column: x => x.SurgeryRoomId,
                        principalTable: "SurgeryRoom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_SurgicalDepartment_SurgicalDepartmentId",
                        column: x => x.SurgicalDepartmentId,
                        principalTable: "SurgicalDepartment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemsBooked",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(name: "Created By", nullable: true),
                    ModifiedBy = table.Column<string>(name: "Modified By", nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    BookingId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsBooked", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemsBooked_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemsBooked_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_AnesthesiaTechniqueId",
                table: "Bookings",
                column: "AnesthesiaTechniqueId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_AnestheticsId",
                table: "Bookings",
                column: "AnestheticsId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SurgeonID",
                table: "Bookings",
                column: "SurgeonID");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SurgeryRoomId",
                table: "Bookings",
                column: "SurgeryRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SurgicalDepartmentId",
                table: "Bookings",
                column: "SurgicalDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsBooked_BookingId",
                table: "ItemsBooked",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsBooked_ItemId",
                table: "ItemsBooked",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsBooked_BookingId_ItemId",
                table: "ItemsBooked",
                columns: new[] { "BookingId", "ItemId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemsBooked");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "AnesthesiaTechnique");

            migrationBuilder.DropTable(
                name: "Anesthetics");

            migrationBuilder.DropTable(
                name: "SurgeryRoom");

            migrationBuilder.DropTable(
                name: "SurgicalDepartment");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "AspNetUsers");
        }
    }
}
