
using EventManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagementAPI.Data
{
	public class EventDB : DbContext
	{
		public DbSet<UserRegistration> UserRegisterationList { get; set; }
		public DbSet<EventRegister> EventRegisterList { get; set; }

		public DbSet<Booking> BookingList { get; set; }
		public DbSet<Payment> PaymentList { get; set; }
		public DbSet<EventOrganize> EventOrganizeList { get; set; }
		public DbSet<EventReport> EventReportsList { get; set; }
		public DbSet<Feedback> FeedbackList { get; set; }

		//public DbSet<EventTicketDetails> EventTickets { get; set; }
		//public DbSet<Role> RoleList { get; set; }

		public EventDB(DbContextOptions<EventDB> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<UserRegistration>().ToTable("UserRegisterationList");
			modelBuilder.Entity<EventRegister>().ToTable("EventRegisterList");
			modelBuilder.Entity<Booking>().ToTable("BookingList");
			modelBuilder.Entity<Payment>().ToTable("PaymentList");
			modelBuilder.Entity<EventOrganize>().ToTable("EventOrganizeList");
			modelBuilder.Entity<EventReport>().ToTable("EventReportsList");
			modelBuilder.Entity<Feedback>().ToTable("FeedbackList");
			//modelBuilder.Entity<Role>().HasNoKey();
            //modelBuilder.Entity<EventTicketDetails>().HasNoKey().ToTable("EventTickets");
            modelBuilder.Entity<UserRegistration>()
				.OwnsOne(u => u.Role); // assumes UserRegistration has a Role property
						   // 1. EventRegister -> UserRegistration (User creates Event)
						   // If a User is deleted, their created events are deleted.
			modelBuilder.Entity<EventRegister>()
				.HasOne(e => e.UserRegistration)
				.WithMany()
				.HasForeignKey(e => e.UserID)
				.OnDelete(DeleteBehavior.Cascade);

			// 2. EventOrganize -> EventRegister (Organizer for a specific Event)
			// If an Event is deleted, its organizing roles are deleted.
			modelBuilder.Entity<EventOrganize>()
				.HasOne(eo => eo.EventRegister)
				.WithMany()
				.HasForeignKey(eo => eo.EventID)
				.OnDelete(DeleteBehavior.Cascade);

			// 3. EventOrganize -> UserRegistration (User acts as Organizer)
			// To break the cascade cycle (UserRegistration -> EventRegister -> EventOrganize vs. UserRegistration -> EventOrganize)
			modelBuilder.Entity<EventOrganize>()
				.HasOne(eo => eo.UserRegistration)
				.WithMany()
				.HasForeignKey(eo => eo.OrganizerID)
				.OnDelete(DeleteBehavior.NoAction); // <--- Remains NoAction (correct)



			// 4. Booking -> UserRegistration (User makes Booking)
			// This is the source of your current error.
			// There's a cascade path: UserRegistration -> EventRegister -> Booking.
			// This direct cascade from UserRegistration -> Booking creates a multiple cascade path.
			modelBuilder.Entity<Booking>()
				.HasOne(b => b.UserRegistration)
				.WithMany()
				.HasForeignKey(b => b.UserID)
				.OnDelete(DeleteBehavior.NoAction); // THIS IS THE FIX // <--- FIX: Change this to NoAction for Booking to UserRegistration


			// 5. Booking -> EventRegister (Booking for an Event)
			// If an Event is deleted, its bookings should definitely be deleted.
			modelBuilder.Entity<Booking>()
				.HasOne(b => b.EventRegister)
				.WithMany()
				.HasForeignKey(b => b.EventID)
				.OnDelete(DeleteBehavior.Cascade); // Keep as Cascade


			// 6. Payment -> Booking (Payment for a Booking)
			// If a Booking is deleted, its associated Payment should also be deleted.
			modelBuilder.Entity<Payment>()
				.HasOne(p => p.Booking)
				.WithMany()
				.HasForeignKey(p => p.BookingID)
				.OnDelete(DeleteBehavior.Cascade); // Keep as Cascade


			// 7. Feedback -> UserRegistration (User provides Feedback)
			// Still advisable to be NoAction to prevent cycles with UserRegistration cascades.
			modelBuilder.Entity<Feedback>()
				.HasOne(f => f.UserRegistration)
				.WithMany()
				.HasForeignKey(f => f.UserID)
				.OnDelete(DeleteBehavior.NoAction); // <--- Remains NoAction (correct)

			// 8. Feedback -> EventRegister (Feedback for an Event)
			// If an Event is deleted, its feedback should be deleted.
			modelBuilder.Entity<Feedback>()
				.HasOne(f => f.EventRegister)
				.WithMany()
				.HasForeignKey(f => f.EventID)
				.OnDelete(DeleteBehavior.Cascade); // Keep as Cascade
		}
	}
}