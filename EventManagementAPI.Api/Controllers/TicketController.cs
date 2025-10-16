
using BarcodeLib;
using EventManagementAPI.Data;
using EventManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace EventManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly EventDB _dbContext;

        public TicketController(EventDB dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("DownloadTicket/{bookingId}")]
        public IActionResult DownloadTicket(int bookingId)
        {
            // Fetch booking with related user and event
            var booking = _dbContext.BookingList
                .Include(b => b.UserRegistration)
                .Include(b => b.EventRegister)
                .FirstOrDefault(b => b.BookingID == bookingId);

            if (booking == null)
                return NotFound(new ApiResponse("fail","Booking not found."));

            var payment = _dbContext.PaymentList
                .FirstOrDefault(p => p.BookingID == bookingId);

            if (payment == null)
                return NotFound(new ApiResponse("fail", "Payment not found."));

            // Generate barcode
            var barcode = new Barcode
            {
                IncludeLabel = true
            };
            Image barcodeImage = barcode.Encode(TYPE.CODE128, $"BOOKINGID-{booking.BookingID}", Color.Black, Color.White, 300, 100);
            using var barcodeStream = new MemoryStream();
            barcodeImage.Save(barcodeStream, ImageFormat.Png);
            barcodeStream.Position = 0;

            var barcodeXImage = XImage.FromStream(() => barcodeStream);

            // Create PDF
            var pdfDoc = new PdfDocument();
            var page = pdfDoc.AddPage();
            page.Size = PdfSharpCore.PageSize.A4;
            var gfx = XGraphics.FromPdfPage(page);

            var titleFont = new XFont("Arial", 18, XFontStyle.Bold);
            var labelFont = new XFont("Arial", 12, XFontStyle.Bold);
            var valueFont = new XFont("Arial", 12, XFontStyle.Regular);

            double y = 40;
            gfx.DrawString("Event Ticket", titleFont, XBrushes.DarkBlue, new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
            y += 40;

            gfx.DrawImage(barcodeXImage, 150, y);
            y += 110;

            void DrawLabelValue(string label, string value)
            {
                gfx.DrawString($"{label}:", labelFont, XBrushes.Black, new XPoint(50, y));
                gfx.DrawString(value, valueFont, XBrushes.Black, new XPoint(200, y));
                y += 25;
            }

            DrawLabelValue("Booking ID", booking.BookingID.ToString());
            DrawLabelValue("Booking Date", booking.BookingDate.ToString("dd-MM-yyyy"));
            DrawLabelValue("Tickets", booking.NoOfSeats.ToString());
            DrawLabelValue("Amount Paid", $"Rupees:{payment.Amount:F2}");
            DrawLabelValue("Payment Status", payment.PaymentStatus);

            y += 10;
            gfx.DrawString("Attendee Details", labelFont, XBrushes.DarkGreen, new XPoint(50, y));
            y += 25;

            DrawLabelValue("Name", booking.UserRegistration?.UserName ?? "N/A");
            DrawLabelValue("Email", booking.UserRegistration?.UserEmail ?? "N/A");
            DrawLabelValue("Phone", booking.UserRegistration?.PhoneNumber ?? "N/A");

            y += 10;
            gfx.DrawString("Event Details", labelFont, XBrushes.DarkRed, new XPoint(50, y));
            y += 25;

            DrawLabelValue("Event Name", booking.EventRegister?.EventName ?? "N/A");
            DrawLabelValue("Description", booking.EventRegister?.EventDescription ?? "N/A");
            DrawLabelValue("Date", booking.EventRegister?.EventDate.ToString("dd-MM-yyyy") ?? "N/A");
            DrawLabelValue("Time", booking.EventRegister?.EventTime.ToString(@"hh\:mm") ?? "N/A");
            DrawLabelValue("Location", booking.EventRegister?.Location ?? "N/A");
            //DrawLabelValue("Category", booking.EventRegister?. ?? "N/A");

            var stream = new MemoryStream();
            pdfDoc.Save(stream, false);
            stream.Position = 0;

            return File(stream, "application/pdf", $"Ticket_{booking.BookingID}.pdf");
        }
    }
}


