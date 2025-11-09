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
        /// Пользователи
        /// </summary>
        public DbSet<UserDao> Products { get; set; }
        
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

            modelBuilder.Entity<UserDao>().ToTable("User", "auth");

            #endregion

            #region Указываем первичные ключи

            #region UserDao

            modelBuilder.Entity<UserDao>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<UserDao>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd(); // Configures 'Id' to be auto-generated on add

            #endregion

            #endregion

            #region Указываем внешние ключи

            #region CalculationJobDao

            //modelBuilder.Entity<CalculationJobDao>()
            //    .HasOne(x => x.CalculationStatus)
            //    .WithMany(x => x.CalculationJobs)
            //    .HasForeignKey(x => x.CalculationStatusId)
            //    .IsRequired();

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
