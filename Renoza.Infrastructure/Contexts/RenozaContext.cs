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

            #endregion

            #region public

            modelBuilder.Entity<CountryDao>().ToTable("Countries", "public");
            modelBuilder.Entity<CustomerProfileDao>().ToTable("CustomerProfiles", "public");
            modelBuilder.Entity<PhoneCountryCodeDao>().ToTable("PhoneCountryCodes", "public");
            modelBuilder.Entity<WorkerProfileDao>().ToTable("WorkerProfiles", "public");

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
