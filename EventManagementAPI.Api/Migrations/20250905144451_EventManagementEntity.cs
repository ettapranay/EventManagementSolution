using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class EventManagementEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRegisterationList",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ConfirmPassword = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Pincode = table.Column<int>(type: "int", nullable: false),
                    Role_RoleID = table.Column<int>(type: "int", nullable: true),
                    Role_RoleName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRegisterationList", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "EventRegisterList",
                columns: table => new
                {
                    EventID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    EventName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EventDescription = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EventDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EventTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EventEndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TotalSeats = table.Column<int>(type: "int", nullable: false),
                    TicketPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventRegisterList", x => x.EventID);
                    table.ForeignKey(
                        name: "FK_EventRegisterList_UserRegisterationList_UserID",
                        column: x => x.UserID,
                        principalTable: "UserRegisterationList",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingList",
                columns: table => new
                {
                    BookingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    EventID = table.Column<int>(type: "int", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoOfSeats = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingList", x => x.BookingID);
                    table.ForeignKey(
                        name: "FK_BookingList_EventRegisterList_EventID",
                        column: x => x.EventID,
                        principalTable: "EventRegisterList",
                        principalColumn: "EventID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingList_UserRegisterationList_UserID",
                        column: x => x.UserID,
                        principalTable: "UserRegisterationList",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "EventOrganizeList",
                columns: table => new
                {
                    OrganizeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizerID = table.Column<int>(type: "int", nullable: false),
                    EventID = table.Column<int>(type: "int", nullable: false),
                    RoleDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventOrganizeList", x => x.OrganizeID);
                    table.ForeignKey(
                        name: "FK_EventOrganizeList_EventRegisterList_EventID",
                        column: x => x.EventID,
                        principalTable: "EventRegisterList",
                        principalColumn: "EventID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventOrganizeList_UserRegisterationList_OrganizerID",
                        column: x => x.OrganizerID,
                        principalTable: "UserRegisterationList",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "EventReportsList",
                columns: table => new
                {
                    EventReportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventID = table.Column<int>(type: "int", nullable: false),
                    ReportDetails = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventReportsList", x => x.EventReportID);
                    table.ForeignKey(
                        name: "FK_EventReportsList_EventRegisterList_EventID",
                        column: x => x.EventID,
                        principalTable: "EventRegisterList",
                        principalColumn: "EventID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackList",
                columns: table => new
                {
                    FeedbackID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    EventID = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackList", x => x.FeedbackID);
                    table.ForeignKey(
                        name: "FK_FeedbackList_EventRegisterList_EventID",
                        column: x => x.EventID,
                        principalTable: "EventRegisterList",
                        principalColumn: "EventID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedbackList_UserRegisterationList_UserID",
                        column: x => x.UserID,
                        principalTable: "UserRegisterationList",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "PaymentList",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingID = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentList", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK_PaymentList_BookingList_BookingID",
                        column: x => x.BookingID,
                        principalTable: "BookingList",
                        principalColumn: "BookingID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingList_EventID",
                table: "BookingList",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_BookingList_UserID",
                table: "BookingList",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_EventOrganizeList_EventID",
                table: "EventOrganizeList",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_EventOrganizeList_OrganizerID",
                table: "EventOrganizeList",
                column: "OrganizerID");

            migrationBuilder.CreateIndex(
                name: "IX_EventRegisterList_UserID",
                table: "EventRegisterList",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_EventReportsList_EventID",
                table: "EventReportsList",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackList_EventID",
                table: "FeedbackList",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackList_UserID",
                table: "FeedbackList",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentList_BookingID",
                table: "PaymentList",
                column: "BookingID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventOrganizeList");

            migrationBuilder.DropTable(
                name: "EventReportsList");

            migrationBuilder.DropTable(
                name: "FeedbackList");

            migrationBuilder.DropTable(
                name: "PaymentList");

            migrationBuilder.DropTable(
                name: "BookingList");

            migrationBuilder.DropTable(
                name: "EventRegisterList");

            migrationBuilder.DropTable(
                name: "UserRegisterationList");
        }
    }
}
