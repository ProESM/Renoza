using Renoza.Infrastructure.Entities.Renoza;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Renoza.Infrastructure.Contexts
{
    /// <summary>
    /// EF DbContext для подключения к БД
    /// </summary>
    public class RenozaContext : BaseDbContext
    {
        #region Наборы сущностей

        /// <summary>
        /// Страны
        /// </summary>
        public DbSet<CountryDao> Countries { get; set; }
        /// <summary>
        /// Международные телефонные коды
        /// </summary>
        public DbSet<PhoneCountryCodeDao> PhoneCountryCodes { get; set; }
        /// <summary>
        /// Пользователи
        /// </summary>
        public DbSet<UserDao> Users { get; set; }
        /// <summary>
        /// Пароли пользователей
        /// </summary>
        public DbSet<UserPasswordDao> UserPasswords { get; set; }
        /// <summary>
        /// История паролей пользователей
        /// </summary>
        public DbSet<UserPasswordHistoryDao> UserPasswordHistory { get; set; }
        /// <summary>
        /// Роли
        /// </summary>
        public DbSet<RoleDao> Roles { get; set; }
        /// <summary>
        /// Связи пользователей и ролей
        /// </summary>
        public DbSet<UserRoleDao> UserRoles { get; set; }
        /// <summary>
        /// Профили заказчиков
        /// </summary>
        public DbSet<CustomerProfileDao> CustomerProfiles { get; set; }
        /// <summary>
        /// Профили работников
        /// </summary>
        public DbSet<WorkerProfileDao> WorkerProfiles { get; set; }
        /// <summary>
        /// Верификация электронной почты
        /// </summary>
        public DbSet<EmailVerificationDao> EmailVerifications { get; set; }
        /// <summary>
        /// Верификация телефонных номеров
        /// </summary>
        public DbSet<PhoneVerificationDao> PhoneVerifications { get; set; }
        /// <summary>
        /// Кассовые чеки
        /// </summary>
        public DbSet<CashReceiptDao> CashReceipts { get; set; }
        /// <summary>
        /// Кассовые чеки заказчика
        /// </summary>
        public DbSet<CustomerCashReceiptDao> CustomerCashReceipts { get; set; }
        /// <summary>
        /// Запросы на загрузку чеков
        /// </summary>
        public DbSet<CashReceiptJobDao> CashReceiptJobs { get; set; }
        /// <summary>
        /// Статусы запросов на загрузку чеков
        /// </summary>
        public DbSet<CashReceiptJobStatusDao> CashReceiptJobStatuses { get; set; }
        /// <summary>
        /// История изменений статусов запросов на загрузку чеков
        /// </summary>
        public DbSet<CashReceiptJobHistoryDao> CashReceiptJobHistory { get; set; }

        #endregion

        /// <summary>
        /// EF DbContext для подключения к БД
        /// </summary>
        public RenozaContext(DbContextOptions options)
            : base(options)
        {
            //Database.Log = sql => Debug.Write(sql);
            //Database.SetInitializer<PromoLamaContext>(null);
            //Database.CommandTimeout = 60 * 60;
            //Database.SetCommandTimeout((int)TimeSpan.FromMinutes(5).TotalSeconds);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Указываем связанные с сущностями таблицы в БД

            #region auth

            modelBuilder.Entity<RoleDao>().ToTable("Roles", "auth");
            modelBuilder.Entity<UserDao>().ToTable("Users", "auth");
            modelBuilder.Entity<UserPasswordDao>().ToTable("UserPasswords", "auth");
            modelBuilder.Entity<UserPasswordHistoryDao>().ToTable("UserPasswordHistory", "auth");
            modelBuilder.Entity<UserRoleDao>().ToTable("UserRoles", "auth");
            modelBuilder.Entity<EmailVerificationDao>().ToTable("EmailVerifications", "auth");
            modelBuilder.Entity<PhoneVerificationDao>().ToTable("PhoneVerifications", "auth");

            #endregion

            #region public

            modelBuilder.Entity<CountryDao>().ToTable("Countries", "public");
            modelBuilder.Entity<CustomerProfileDao>().ToTable("CustomerProfiles", "public");
            modelBuilder.Entity<PhoneCountryCodeDao>().ToTable("PhoneCountryCodes", "public");
            modelBuilder.Entity<WorkerProfileDao>().ToTable("WorkerProfiles", "public");

            modelBuilder.Entity<CashReceiptDao>().ToTable("CashReceipts", "public");
            modelBuilder.Entity<CustomerCashReceiptDao>().ToTable("CustomerCashReceipts", "public");
            modelBuilder.Entity<CashReceiptJobDao>().ToTable("CashReceiptJobs", "public");
            modelBuilder.Entity<CashReceiptJobStatusDao>().ToTable("CashReceiptJobStatuses", "public");
            modelBuilder.Entity<CashReceiptJobHistoryDao>().ToTable("CashReceiptJobHistory", "public");

            #endregion

            #endregion

            #region Указываем первичные ключи

            #region auth

            #region RoleDao

            modelBuilder.Entity<RoleDao>()
                .HasKey(x => x.Id);

            #endregion

            #region UserDao

            modelBuilder.Entity<UserDao>()
                .HasKey(x => x.Id);

            #endregion

            #region UserPasswordDao

            modelBuilder.Entity<UserPasswordDao>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<UserPasswordDao>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd() // Configures 'Id' to be auto-generated on add
                ;

            #endregion

            #region UserPasswordHistoryDao

            modelBuilder.Entity<UserPasswordHistoryDao>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<UserPasswordHistoryDao>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd() // Configures 'Id' to be auto-generated on add
                ;

            #endregion

            #region UserRoleDao

            modelBuilder.Entity<UserRoleDao>()
                .HasKey(x => new { x.UserId, x.RoleId });

            #endregion

            #region EmailVerificationDao

            modelBuilder.Entity<EmailVerificationDao>()
                .HasKey(x => x.Id);

            #endregion

            #region PhoneVerificationDao

            modelBuilder.Entity<PhoneVerificationDao>()
                .HasKey(x => x.Id);

            #endregion

            #endregion

            #region public

            #region CountryDao

            modelBuilder.Entity<CountryDao>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<CountryDao>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd() // Configures 'Id' to be auto-generated on add
                ;

            #endregion

            #region CustomerProfileDao

            modelBuilder.Entity<CustomerProfileDao>()
                .HasKey(x => x.Id);

            #endregion

            #region PhoneCountryCodeDao

            modelBuilder.Entity<PhoneCountryCodeDao>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<PhoneCountryCodeDao>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd() // Configures 'Id' to be auto-generated on add
                ;

            #endregion

            #region WorkerProfileDao

            modelBuilder.Entity<WorkerProfileDao>()
                .HasKey(x => x.Id);

            #endregion

            #region CustomerCashReceiptDao

            modelBuilder.Entity<CustomerCashReceiptDao>()
                .HasKey(x => new { x.CustomerId, x.CashReceiptId });

            #endregion

            #region CashReceiptDao

            modelBuilder.Entity<CashReceiptDao>()
                .HasKey(x => x.Id);

            #endregion

            #region CashReceiptJobDao

            modelBuilder.Entity<CashReceiptJobDao>()
                .HasKey(x => x.Id);

            #endregion

            #region CashReceiptJobStatusDao

            modelBuilder.Entity<CashReceiptJobStatusDao>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<CashReceiptJobStatusDao>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd(); // Configures 'Id' to be auto-generated on add

            #endregion

            #region CashReceiptJobHistoryDao

            modelBuilder.Entity<CashReceiptJobHistoryDao>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<CashReceiptJobHistoryDao>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd(); // Configures 'Id' to be auto-generated on add

            #endregion

            #endregion

            #endregion

            #region Указываем внешние ключи

            #region auth

            #region UserPasswordDao

            modelBuilder.Entity<UserPasswordDao>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserPasswords)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            #endregion

            #region UserPasswordHistoryDao

            modelBuilder.Entity<UserPasswordHistoryDao>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserPasswordHistory)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            #endregion

            #region UserRoleDao

            modelBuilder.Entity<UserRoleDao>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.UserId)
                .IsRequired();
            modelBuilder.Entity<UserRoleDao>()
                .HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .IsRequired();

            #endregion

            #region EmailVerificationDao

            modelBuilder.Entity<EmailVerificationDao>()
                .HasOne(x => x.User)
                .WithMany(x => x.EmailVerifications)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            #endregion

            #region PhoneVerificationDao

            modelBuilder.Entity<PhoneVerificationDao>()
                .HasOne(x => x.User)
                .WithMany(x => x.PhoneVerifications)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            #endregion

            #endregion

            #region CustomerProfileDao

            modelBuilder.Entity<CustomerProfileDao>()
                .HasOne(x => x.User)
                .WithMany(x => x.CustomerProfiles)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            #endregion

            #region PhoneCountryCodeDao

            modelBuilder.Entity<PhoneCountryCodeDao>()
                .HasOne(x => x.Country)
                .WithMany(x => x.PhoneCountryCodes)
                .HasForeignKey(x => x.CountryId)
                .IsRequired();

            #endregion

            #region WorkerProfileDao

            modelBuilder.Entity<WorkerProfileDao>()
                .HasOne(x => x.User)
                .WithMany(x => x.WorkerProfiles)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            #endregion

            #region CashReceiptDao

            

            #endregion

            #region CashReceiptJobDao

            modelBuilder.Entity<CashReceiptJobDao>()
                .HasOne(x => x.Status)
                .WithMany(x => x.CashReceiptJobs)
                .HasForeignKey(x => x.StatusId)
                .IsRequired();

            modelBuilder.Entity<CashReceiptJobDao>()
                .HasOne(x => x.CashReceipt)
                .WithMany(x => x.CashReceiptJobs)
                .HasForeignKey(x => x.CashReceiptId)
                .IsRequired(false);

            #endregion

            #region CashReceiptJobHistoryDao

            modelBuilder.Entity<CashReceiptJobHistoryDao>()
                .HasOne(x => x.Job)
                .WithMany(x => x.History)
                .HasForeignKey(x => x.JobId)
                .IsRequired();

            modelBuilder.Entity<CashReceiptJobHistoryDao>()
                .HasOne(x => x.Status)
                .WithMany(x => x.CashReceiptJobHistory)
                .HasForeignKey(x => x.StatusId)
                .IsRequired();

            #endregion

            #region CustomerCashReceiptDao

            modelBuilder.Entity<CustomerCashReceiptDao>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.CustomerCashReceipts)
                .HasForeignKey(x => x.CustomerId)
                .IsRequired();

            modelBuilder.Entity<CustomerCashReceiptDao>()
                .HasOne(x => x.CashReceipt)
                .WithMany(x => x.CustomerCashReceipts)
                .HasForeignKey(x => x.CashReceiptId)
                .IsRequired();

            #endregion

            #endregion

            #region Описываем остальные специфичные поля

            #region ExternalCalculationJobDao

            //modelBuilder.Entity<ExternalCalculationJobDao>()
            //    .Property(x => x.CalculationJobJson)
            //    .HasColumnType("nvarchar(MAX)");

            #endregion

            #endregion

            base.OnModelCreating(modelBuilder);
        }

        public static readonly ILoggerFactory PromoLamaLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddDebug();
            //builder.AddConsole();
        });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(PromoLamaLoggerFactory);
            if (optionsBuilder.IsConfigured == false)
            {
                base.OnConfiguring(optionsBuilder);
            }
            //if (!optionsBuilder.IsConfigured)
            //{
            //    IConfigurationRoot configuration = new ConfigurationBuilder()
            //        . SetBasePath(Directory.GetCurrentDirectory())
            //        .AddJsonFile("appsettings.json")
            //        .Build();


            //    var conn = new SqlConnectionStringBuilder(configuration.GetConnectionString("DefaultConnection"))
            //    {
            //        ConnectRetryCount = 50,
            //        ConnectRetryInterval = 10,
            //        MaxPoolSize = 1200,
            //        MinPoolSize = 5
            //    };

            //    optionsBuilder.UseSqlServer(conn.ToString(),
            //        serverDbContextOptionsBuilder =>
            //        {
            //            var minutes = (int)TimeSpan.FromMinutes(600).TotalSeconds;
            //            serverDbContextOptionsBuilder.CommandTimeout(minutes);
            //            serverDbContextOptionsBuilder.EnableRetryOnFailure(20);
            //        });
            //}
        }
    }
}
