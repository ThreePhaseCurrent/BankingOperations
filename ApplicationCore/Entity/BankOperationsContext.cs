using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ApplicationCore.Entity
{
    public partial class BankOperationsContext : DbContext
    {
        public BankOperationsContext()
        {
        }

        public BankOperationsContext(DbContextOptions<BankOperationsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Accrual> Accrual { get; set; }
        public virtual DbSet<BankAccount> BankAccount { get; set; }
        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<Credit> Credit { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<Deposit> Deposit { get; set; }
        public virtual DbSet<ExchangeRate> ExchangeRate { get; set; }
        public virtual DbSet<LegalPerson> LegalPerson { get; set; }
        public virtual DbSet<Operation> Operation { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<PhysicalPerson> PhysicalPerson { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accrual>(entity =>
            {
                entity.HasKey(e => e.IdAccrual);

                entity.Property(e => e.IdAccrual).HasColumnName("id_accrual");

                entity.Property(e => e.AccrualAmount)
                    .HasColumnName("accrual_amount")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.AccrualDate)
                    .HasColumnName("accrual_date")
                    .HasColumnType("date");

                entity.Property(e => e.IdDeposit).HasColumnName("id_deposit");

                entity.HasOne(d => d.IdDepositNavigation)
                    .WithMany(p => p.Accrual)
                    .HasForeignKey(d => d.IdDeposit)
                    .HasConstraintName("FK_Accrual_deposit");
            });

            modelBuilder.Entity<BankAccount>(entity =>
            {
                entity.HasKey(e => e.IdAccount)
                    .HasName("PK__bank_acc__B2C7C783FDC62652");

                entity.ToTable("bank_account");

                entity.Property(e => e.IdAccount).HasColumnName("id_account");

                entity.Property(e => e.AccountType)
                    .HasColumnName("account_type")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.DateClose)
                    .HasColumnName("date_close")
                    .HasColumnType("date");

                entity.Property(e => e.DateOpen)
                    .HasColumnName("date_open")
                    .HasColumnType("date");

                entity.Property(e => e.IdClient).HasColumnName("id_client");

                entity.Property(e => e.IdCurrency).HasColumnName("id_currency");

                entity.HasOne(d => d.IdClientNavigation)
                    .WithMany(p => p.BankAccount)
                    .HasForeignKey(d => d.IdClient)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__bank_acco__id_cl__412EB0B6");

                entity.HasOne(d => d.IdCurrencyNavigation)
                    .WithMany(p => p.BankAccount)
                    .HasForeignKey(d => d.IdCurrency)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__bank_acco__id_cu__4316F928");
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.IdClient)
                    .HasName("PK__client__6EC2B6C0AA0D35CD");

                entity.ToTable("client");

                entity.Property(e => e.IdClient).HasColumnName("id_client");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnName("address")
                    .HasMaxLength(120)
                    .IsUnicode(false);

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasColumnName("login")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(24)
                    .IsUnicode(false);

                entity.Property(e => e.TelNumber)
                    .IsRequired()
                    .HasColumnName("tel_number")
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Credit>(entity =>
            {
                entity.HasKey(e => e.IdCredit);

                entity.ToTable("credit");

                entity.Property(e => e.IdCredit).HasColumnName("id_credit");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.DateCredit)
                    .HasColumnName("date_credit")
                    .HasColumnType("date");

                entity.Property(e => e.DateCreditFinish)
                    .HasColumnName("date_credit_finish")
                    .HasColumnType("date");

                entity.Property(e => e.IdAccount).HasColumnName("id_account");

                entity.Property(e => e.PercentCredit)
                    .HasColumnName("percent_credit")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.IdAccountNavigation)
                    .WithMany(p => p.Credit)
                    .HasForeignKey(d => d.IdAccount)
                    .HasConstraintName("FK_credit_bank_account");
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(e => e.IdCurrency)
                    .HasName("PK__currency__D9AE349157E6BB9A");

                entity.ToTable("currency");

                entity.Property(e => e.IdCurrency).HasColumnName("id_currency");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ShortName)
                    .IsRequired()
                    .HasColumnName("short_name")
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Deposit>(entity =>
            {
                entity.HasKey(e => e.IdDeposit);

                entity.ToTable("deposit");

                entity.Property(e => e.IdDeposit).HasColumnName("id_deposit");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.DateDeposit)
                    .HasColumnName("date_deposit")
                    .HasColumnType("date");

                entity.Property(e => e.DateDepositFinish)
                    .HasColumnName("date_deposit_finish")
                    .HasColumnType("date");

                entity.Property(e => e.IdAccount).HasColumnName("id_account");

                entity.Property(e => e.PercentDeposit)
                    .HasColumnName("percent_deposit")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.IdAccountNavigation)
                    .WithMany(p => p.Deposit)
                    .HasForeignKey(d => d.IdAccount)
                    .HasConstraintName("FK_deposit_deposit");
            });

            modelBuilder.Entity<ExchangeRate>(entity =>
            {
                entity.HasKey(e => new { e.DateRate, e.IdCurrency })
                    .HasName("PK__exchange__180651E1A5EECBD7");

                entity.ToTable("exchange_rate");

                entity.Property(e => e.DateRate)
                    .HasColumnName("date_rate")
                    .HasColumnType("date");

                entity.Property(e => e.IdCurrency).HasColumnName("id_currency");

                entity.Property(e => e.RateBuy)
                    .HasColumnName("rate_buy")
                    .HasColumnType("decimal(10, 5)");

                entity.Property(e => e.RateSale)
                    .HasColumnName("rate_sale")
                    .HasColumnType("decimal(10, 5)");

                entity.HasOne(d => d.IdCurrencyNavigation)
                    .WithMany(p => p.ExchangeRate)
                    .HasForeignKey(d => d.IdCurrency)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__exchange___id_cu__45F365D3");
            });

            modelBuilder.Entity<LegalPerson>(entity =>
            {
                entity.HasKey(e => e.IdEdrpou);

                entity.ToTable("legal_person");

                entity.Property(e => e.IdEdrpou)
                    .HasColumnName("id_edrpou")
                    .ValueGeneratedNever();

                entity.Property(e => e.Director)
                    .IsRequired()
                    .HasColumnName("director")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OwnershipType)
                    .IsRequired()
                    .HasColumnName("ownership_type")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdEdrpouNavigation)
                    .WithOne(p => p.LegalPerson)
                    .HasForeignKey<LegalPerson>(d => d.IdEdrpou)
                    .HasConstraintName("FK_LegalPerson_Client");
            });

            modelBuilder.Entity<Operation>(entity =>
            {
                entity.HasKey(e => new { e.OperationTime, e.IdAccount });

                entity.ToTable("operation");

                entity.Property(e => e.OperationTime)
                    .HasColumnName("operation_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.IdAccount).HasColumnName("id_account");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.TypeOperation)
                    .IsRequired()
                    .HasColumnName("type_operation")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdAccountNavigation)
                    .WithMany(p => p.Operation)
                    .HasForeignKey(d => d.IdAccount)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__operation__id_ac__47DBAE45");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.IdPayment);

                entity.ToTable("payment");

                entity.Property(e => e.IdPayment).HasColumnName("id_payment");

                entity.Property(e => e.AmountPayment)
                    .HasColumnName("amount_payment")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.DatePayment)
                    .HasColumnName("date_payment")
                    .HasColumnType("date");

                entity.Property(e => e.IdCredit).HasColumnName("id_credit");

                entity.HasOne(d => d.IdCreditNavigation)
                    .WithMany(p => p.Payment)
                    .HasForeignKey(d => d.IdCredit)
                    .HasConstraintName("FK_payment_credit");
            });

            modelBuilder.Entity<PhysicalPerson>(entity =>
            {
                entity.HasKey(e => e.IdPerson);

                entity.ToTable("physical_person");

                entity.Property(e => e.IdPerson)
                    .HasColumnName("id_person")
                    .ValueGeneratedNever();

                entity.Property(e => e.IdentificationNumber)
                    .IsRequired()
                    .HasColumnName("identification_number")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PassportNumber)
                    .IsRequired()
                    .HasColumnName("passport_number")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PassportSeries)
                    .IsRequired()
                    .HasColumnName("passport_series")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.Patronymic)
                    .IsRequired()
                    .HasColumnName("patronymic")
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasColumnName("surname")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdPersonNavigation)
                    .WithOne(p => p.PhysicalPerson)
                    .HasForeignKey<PhysicalPerson>(d => d.IdPerson)
                    .HasConstraintName("FK_physical_person_client");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
