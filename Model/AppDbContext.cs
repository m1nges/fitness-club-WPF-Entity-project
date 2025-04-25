using Microsoft.EntityFrameworkCore;
using fitness_club.Classes;

namespace fitness_club.Model
{
    public class AppDbContext : DbContext
    {
        public DbSet<Class> Class { get; set; }
        public DbSet<ClassVisits> ClassVisits { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<TrainerReview> TrainerReviews { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<ClassType> ClassTypes { get; set; }
        public DbSet<ClientMembership> ClientMemberships { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<MembershipType> MembershipTypes { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<MembershipService> MembershipServices { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<EquipmentCondition> EquipmentConditions { get; set; }
        public DbSet<ClassInfo> ClassInfo { get; set; }
        public DbSet<ClassReviews> ClassReviews { get; set; }
        public DbSet<RentedLocker> RentedLockers { get; set; }
        public DbSet<Locker> Lockers { get; set; }
        public DbSet<HallEquipment> HallEquipments { get; set; }
        public DbSet<ClassPayments> ClassPayments { get; set; }
        public DbSet<MembershipPayments> MembershipPayments { get; set; }
        public DbSet<ServicesPayments> ServicesPayments { get; set; }
        public DbSet<TrainingPlan> TrainingPlans { get; set; }
        public DbSet<ClientTransaction> ClientTransactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5050;Database=fitness-club;Username=postgres;Password=1");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HallEquipment>()
                .HasKey(he => new { he.HallId, he.EquipmentId });

            base.OnModelCreating(modelBuilder);
        }

        public AppDbContext()
        {
            Database.EnsureCreated();
        }
    }
}
