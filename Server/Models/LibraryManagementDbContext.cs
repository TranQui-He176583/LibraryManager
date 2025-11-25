using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public partial class LibraryManagementDbContext : DbContext
{
    public LibraryManagementDbContext()
    {
    }

    public LibraryManagementDbContext(DbContextOptions<LibraryManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BorrowingDetail> BorrowingDetails { get; set; }

    public virtual DbSet<BorrowingTicket> BorrowingTickets { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Fine> Fines { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }





    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Authors__70DAFC34C1934A6F");

            entity.Property(e => e.AuthorName).HasMaxLength(200);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Books__3DE0C207BA662C8E");

            entity.HasIndex(e => e.Isbn, "UQ__Books__447D36EA4AA81E44").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Isbn)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ISBN");
            entity.Property(e => e.Language)
                .HasMaxLength(50)
                .HasDefaultValue("Tiếng Việt");
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.Price)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Title).HasMaxLength(300);
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherId)
                .HasConstraintName("FK__Books__Publisher__52593CB8");

            entity.HasMany(d => d.Authors).WithMany(p => p.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookAuthor",
                    r => r.HasOne<Author>().WithMany()
                        .HasForeignKey("AuthorId")
                        .HasConstraintName("FK__BookAutho__Autho__5CD6CB2B"),
                    l => l.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .HasConstraintName("FK__BookAutho__BookI__5BE2A6F2"),
                    j =>
                    {
                        j.HasKey("BookId", "AuthorId").HasName("PK__BookAuth__6AED6DC43F0917CC");
                        j.ToTable("BookAuthors");
                    });

            entity.HasMany(d => d.Categories).WithMany(p => p.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK_BookCategories_Categories"),
                    l => l.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .HasConstraintName("FK_BookCategories_Books"),
                    j =>
                    {
                        j.HasKey("BookId", "CategoryId");
                        j.ToTable("BookCategories");
                    });
        });

        modelBuilder.Entity<BorrowingDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__Borrowin__135C316D5A6B7006");

            entity.HasIndex(e => new { e.TicketId, e.BookId }, "UQ_Ticket_Book").IsUnique();

            entity.Property(e => e.ReturnDate).HasPrecision(0);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Đang mượn");

            entity.HasOne(d => d.Book).WithMany(p => p.BorrowingDetails)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Borrowing__BookI__66603565");

            entity.HasOne(d => d.Ticket).WithMany(p => p.BorrowingDetails)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("FK__Borrowing__Ticke__656C112C");
        });

        modelBuilder.Entity<BorrowingTicket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Borrowin__712CC607D4534203");

            entity.Property(e => e.BorrowDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DueDate).HasPrecision(0);
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(d => d.Librarian).WithMany(p => p.BorrowingTicketLibrarians)
                .HasForeignKey(d => d.LibrarianId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Borrowing__Libra__60A75C0F");

            entity.HasOne(d => d.Member).WithMany(p => p.BorrowingTicketMembers)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Borrowing__Membe__5FB337D6");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0B47A332ED");

            entity.HasIndex(e => e.CategoryName, "UQ__Categori__8517B2E0DDD6FB44").IsUnique();

            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Fine>(entity =>
        {
            entity.HasKey(e => e.FineId).HasName("PK__Fines__9D4A9B2CE6A3D928");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.PaidDate).HasPrecision(0);
            entity.Property(e => e.Reason).HasMaxLength(100);

            entity.HasOne(d => d.BorrowDetail).WithMany(p => p.Fines)
                .HasForeignKey(d => d.BorrowDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Fines__BorrowDet__6B24EA82");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK__Members__0CF04B18834DB389");

            entity.HasIndex(e => e.UserId, "UQ__Members__1788CC4D3FAE3D1B").IsUnique();

            entity.HasIndex(e => e.IdcardNumber, "UQ__Members__2CEB98364DBD8076").IsUnique();

            entity.Property(e => e.IdcardNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("IDCardNumber");
            entity.Property(e => e.MaxBorrowLimit).HasDefaultValue(5);
            entity.Property(e => e.MembershipDate).HasDefaultValueSql("(CONVERT([date],getdate()))");
            entity.Property(e => e.SuspendedAt).HasPrecision(0);

            entity.HasOne(d => d.User).WithOne(p => p.Member)
                .HasForeignKey<Member>(d => d.UserId)
                .HasConstraintName("FK__Members__UserId__440B1D61");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E1214E4A21D");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.RelatedEntityType).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasDefaultValue("Info");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__71D1E811");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.PublisherId).HasName("PK__Publishe__4C657FAB65172EE4");

            entity.Property(e => e.PublisherName).HasMaxLength(200);
            entity.Property(e => e.Website)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A8B0718CC");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160424943EB").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C56DE8D3C");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E48D72ACFC").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534C75DBC67").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__3C69FB99");
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
