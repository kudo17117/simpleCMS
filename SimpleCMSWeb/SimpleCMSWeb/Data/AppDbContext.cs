using SimpleCMSWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace SimpleCMSWeb.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Patientuser> PatientUsers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<EmployeeType> EmployeeTypes { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Specialization> Specializations { get; set; } 
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Clinicdoctor> ClinicDoctors { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Municipality> Municipalities { get; set; }
        public DbSet<AdminNotification> AdminNotifications { get; set; }

        public DbSet<ClinicSchedule> ClinicSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User relationship to Employee configuration
            modelBuilder.Entity<User>()
                .HasKey(u => u.Employeeid);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Employee)
                .WithOne(e => e.User)
                .HasForeignKey<User>(u => u.Employeeid);

            // Employee relationship to EmployeeType configuration
            modelBuilder.Entity<Employee>()
                .HasKey(e => e.Employeeid);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.EmployeeType)
                .WithMany(et => et.Employees)
                .HasForeignKey(e => e.EmployeeTypeId);

            modelBuilder.Entity<Patientuser>()
                .HasOne(u => u.Patient)
                .WithMany(p => p.Patientusers)
                .HasForeignKey(u => u.Patientid);

            modelBuilder.Entity<Clinicdoctor>(entity =>
            {
                entity.HasKey(cd => cd.Clinicdoctorid);

                // Relationship: Clinicdoctor ➡️ Clinic (Many to One)
                entity.HasOne(cd => cd.Clinic)
                    .WithMany(c => c.ClinicDoctors)
                    .HasForeignKey(cd => cd.Clinicid)
                    .OnDelete(DeleteBehavior.Restrict); // Or your preferred behavior

                // Relationship: Clinicdoctor ➡️ Employee (Many to One)
                entity.HasOne(cd => cd.Employee)
                    .WithMany(e => e.ClinicDoctors)
                    .HasForeignKey(cd => cd.Employeeid)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relationship: Clinicdoctor ➡️ ClinicSchedules (One to Many)
                entity.HasMany(cd => cd.ClinicSchedules)
                    .WithOne(cs => cs.Clinicdoctors)
                    .HasForeignKey(cs => cs.ClinicDoctorId)
                    .OnDelete(DeleteBehavior.Restrict);

                //// Optional: Appointments
                //entity.HasMany(cd => cd.Appointments)
                //    .WithOne(a => a.ClinicDoctor)
                //    .HasForeignKey(a => a.ClinicDoctorId)
                //    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Employee>()
             .HasOne(s => s.Specialization)
             .WithMany(e => e.Employees)
             .HasForeignKey(s => s.SpecializationId);

            modelBuilder.Entity<Clinic>()
                .HasOne(m => m.Municipality)
                .WithMany(c => c.Clinics)
                .HasForeignKey(m => m.MunicipalityId);

            modelBuilder.Entity<Municipality>()
                .HasOne(m => m.Province)
                .WithMany(c => c.Municipalities)
                .HasForeignKey(m => m.ProvinceId);

            modelBuilder.Entity<Province>()
                .HasOne(m => m.Region)
                .WithMany(c => c.Provinces)
                .HasForeignKey(m => m.RegionId);

            modelBuilder.Entity<Clinicdoctor>()
                .HasOne(c => c.Employee)
                .WithMany(e => e.ClinicDoctors)
                .HasForeignKey(c => c.Employeeid);
        }

    }
}
