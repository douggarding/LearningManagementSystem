using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class Team2Context : DbContext
    {
        public virtual DbSet<Administrators> Administrators { get; set; }
        public virtual DbSet<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual DbSet<Assignments> Assignments { get; set; }
        public virtual DbSet<Classes> Classes { get; set; }
        public virtual DbSet<Courses> Courses { get; set; }
        public virtual DbSet<Departments> Departments { get; set; }
        public virtual DbSet<Enrolled> Enrolled { get; set; }
        public virtual DbSet<Professors> Professors { get; set; }
        public virtual DbSet<Students> Students { get; set; }
        public virtual DbSet<Submissions> Submissions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("Server=atr.eng.utah.edu;User Id=;Password=;Database=Team2");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrators>(entity =>

            {
                entity.HasKey(e => e.UId);

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            });

            modelBuilder.Entity<AssignmentCategories>(entity =>
            {
                entity.HasKey(e => e.CategoryId);

                entity.HasIndex(e => e.Class)
                    .HasName("ClassID_idx");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("categoryID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Class).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Weight).HasColumnType("int(11)");

                entity.HasOne(d => d.ClassNavigation)
                    .WithMany(p => p.AssignmentCategories)
                    .HasForeignKey(d => d.Class)
                    .HasConstraintName("ClassID");
            });

            modelBuilder.Entity<Assignments>(entity =>
            {
                entity.HasKey(e => e.AssignmentId);

                entity.HasIndex(e => e.Category)
                    .HasName("Category_idx");

                entity.Property(e => e.AssignmentId)
                    .HasColumnName("assignmentID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Category).HasColumnType("int(11)");

                entity.Property(e => e.Contents)
                    .IsRequired()
                    .HasColumnType("varchar(8192)");

                entity.Property(e => e.Due).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Points).HasColumnType("int(11)");

                entity.Property(e => e.SubmissionType)
                    .HasColumnName("submissionType")
                    .HasColumnType("tinyint(4)");

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.Category)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Category");
            });

            modelBuilder.Entity<Classes>(entity =>
            {
                entity.HasKey(e => e.ClassId);

                entity.HasIndex(e => e.Offering)
                    .HasName("CourseID_idx");

                entity.HasIndex(e => e.TaughtBy)
                    .HasName("Professor_idx");

                entity.Property(e => e.ClassId)
                    .HasColumnName("classID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.End).HasColumnType("time");

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Offering).HasColumnType("int(11)");

                entity.Property(e => e.Season)
                    .IsRequired()
                    .HasMaxLength(6);

                entity.Property(e => e.Start).HasColumnType("time");

                entity.Property(e => e.TaughtBy)
                    .IsRequired()
                    .HasColumnType("char(8)");

                entity.HasOne(d => d.OfferingNavigation)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.Offering)
                    .HasConstraintName("CourseID");

                entity.HasOne(d => d.TaughtByNavigation)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.TaughtBy)
                    .HasConstraintName("Professor");

                entity.Property(e => e.Year).HasColumnType("year");
            });

            modelBuilder.Entity<Courses>(entity =>
            {
                entity.HasKey(e => e.CatalogId);

                entity.HasIndex(e => e.Department)
                    .HasName("Department_idx");

                entity.HasIndex(e => new { e.Number, e.Department })
                    .HasName("Number_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.CatalogId)
                    .HasColumnName("CatalogID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Department)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Number).HasColumnType("int(11)");

                entity.HasOne(d => d.DepartmentNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.Department)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Department");
            });

            modelBuilder.Entity<Departments>(entity =>
            {
                entity.HasKey(e => e.DId);

                entity.Property(e => e.DId)
                    .HasColumnName("dID")
                    .HasMaxLength(4);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Enrolled>(entity =>
            {
                entity.HasKey(e => new { e.Student, e.Class });

                entity.HasIndex(e => e.Class)
                    .HasName("Class_idx");

                entity.Property(e => e.Student).HasColumnType("char(8)");

                entity.Property(e => e.Class).HasColumnType("int(11)");

                entity.Property(e => e.Grade).HasMaxLength(2);

                entity.HasOne(d => d.ClassNavigation)
                    .WithMany(p => p.Enrolled)
                    .HasForeignKey(d => d.Class)
                    .HasConstraintName("Class");

                entity.HasOne(d => d.StudentNavigation)
                    .WithMany(p => p.Enrolled)
                    .HasForeignKey(d => d.Student)
                    .HasConstraintName("Student");
            });

            modelBuilder.Entity<Professors>(entity =>
            {
                entity.HasKey(e => e.UId);

                entity.HasIndex(e => e.WorksIn)
                    .HasName("WorksIn_idx");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.WorksIn)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.HasOne(d => d.WorksInNavigation)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.WorksIn)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("WorksIn");

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            });

            modelBuilder.Entity<Students>(entity =>
            {
                entity.HasKey(e => e.UId);

                entity.HasIndex(e => e.Major)
                    .HasName("Major_idx");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Major)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(d => d.MajorNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.Major)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Major");

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            });

            modelBuilder.Entity<Submissions>(entity =>
            {
                entity.HasKey(e => new { e.Assignment, e.Student });

                entity.HasIndex(e => e.Student)
                    .HasName("StudentID_idx");

                entity.Property(e => e.Assignment).HasColumnType("int(11)");

                entity.Property(e => e.Student).HasColumnType("char(8)");

                entity.Property(e => e.BinaryContents).HasColumnType("blob");

                entity.Property(e => e.Score).HasColumnType("int(11)");

                entity.Property(e => e.TextContents).HasColumnType("varchar(8192)");

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.HasOne(d => d.AssignmentNavigation)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.Assignment)
                    .HasConstraintName("Assignment");

                entity.HasOne(d => d.StudentNavigation)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.Student)
                    .HasConstraintName("StudentID");
            });
        }
    }
}
