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
        /// <summary>
        /// Типы компаний
        /// </summary>
        public DbSet<CompanyTypeDao> CompanyTypes { get; set; }
        /// <summary>
        /// Профили компаний
        /// </summary>
        public DbSet<CompanyProfileDao> CompanyProfiles { get; set; }
        /// <summary>
        /// Результаты верификации компаний
        /// </summary>
        public DbSet<CompanyVerificationDao> CompanyVerifications { get; set; }
        /// <summary>
        /// Статусы заданий на верификацию компаний
        /// </summary>
        public DbSet<CompanyVerificationJobStatusDao> CompanyVerificationJobStatuses { get; set; }
        /// <summary>
        /// Задания на верификацию компаний
        /// </summary>
        public DbSet<CompanyVerificationJobDao> CompanyVerificationJobs { get; set; }
        /// <summary>
        /// История изменений статусов заданий на верификацию компаний
        /// </summary>
        public DbSet<CompanyVerificationJobHistoryDao> CompanyVerificationJobHistory { get; set; }
        /// <summary>
        /// Профили технического надзора
        /// </summary>
        public DbSet<TechnicalSupervisorProfileDao> TechnicalSupervisorProfiles { get; set; }
        /// <summary>
        /// Роли участников компании
        /// </summary>
        public DbSet<MemberRoleDao> MemberRoles { get; set; }
        /// <summary>
        /// Участники компаний
        /// </summary>
        public DbSet<CompanyMemberDao> CompanyMembers { get; set; }
        /// <summary>
        /// Статусы запросов на вступление в компанию
        /// </summary>
        public DbSet<CompanyJoinRequestStatusDao> CompanyJoinRequestStatuses { get; set; }
        /// <summary>
        /// Запросы на вступление в компанию
        /// </summary>
        public DbSet<CompanyJoinRequestDao> CompanyJoinRequests { get; set; }

        /// <summary>
        /// Избранное
        /// </summary>
        public DbSet<FavoriteDao> Favorites { get; set; }

        /// <summary>
        /// Категории товаров и услуг
        /// </summary>
        public DbSet<ProductCategoryDao> ProductCategories { get; set; }

        /// <summary>
        /// Типы единиц измерения
        /// </summary>
        public DbSet<MeasurementUnitTypeDao> MeasurementUnitTypes { get; set; }

        /// <summary>
        /// Единицы измерения
        /// </summary>
        public DbSet<MeasurementUnitDao> MeasurementUnits { get; set; }

        /// <summary>
        /// Валюты
        /// </summary>
        public DbSet<CurrencyDao> Currencies { get; set; }

        /// <summary>
        /// Товары и услуги
        /// </summary>
        public DbSet<ProductDao> Products { get; set; }

        /// <summary>
        /// Базовые цены профилей на товары/услуги
        /// </summary>
        public DbSet<ProfileProductPriceDao> ProfileProductPrices { get; set; }

        /// <summary>
        /// Переопределенные цены в альтернативных валютах
        /// </summary>
        public DbSet<ProfileProductOverridePriceDao> ProfileProductOverridePrices { get; set; }

        /// <summary>
        /// Корзина покупок
        /// </summary>
        public DbSet<CartItemDao> CartItems { get; set; }

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

            modelBuilder.Entity<CompanyTypeDao>().ToTable("CompanyTypes", "public");
            modelBuilder.Entity<CompanyProfileDao>().ToTable("CompanyProfiles", "public");
            modelBuilder.Entity<CompanyVerificationDao>().ToTable("CompanyVerifications", "public");
            modelBuilder.Entity<CompanyVerificationJobStatusDao>().ToTable("CompanyVerificationJobStatuses", "public");
            modelBuilder.Entity<CompanyVerificationJobDao>().ToTable("CompanyVerificationJobs", "public");
            modelBuilder.Entity<CompanyVerificationJobHistoryDao>().ToTable("CompanyVerificationJobHistory", "public");
            modelBuilder.Entity<TechnicalSupervisorProfileDao>().ToTable("TechnicalSupervisorProfiles", "public");
            modelBuilder.Entity<CompanyMemberDao>().ToTable("CompanyMembers", "public");
            modelBuilder.Entity<CompanyJoinRequestStatusDao>().ToTable("CompanyJoinRequestStatuses", "public");
            modelBuilder.Entity<CompanyJoinRequestDao>().ToTable("CompanyJoinRequests", "public");
            modelBuilder.Entity<FavoriteDao>().ToTable("Favorites", "public");

            modelBuilder.Entity<ProductCategoryDao>().ToTable("ProductCategories", "public");
            modelBuilder.Entity<MeasurementUnitTypeDao>().ToTable("MeasurementUnitTypes", "public");
            modelBuilder.Entity<MeasurementUnitDao>().ToTable("MeasurementUnits", "public");
            modelBuilder.Entity<CurrencyDao>().ToTable("Currencies", "public");
            modelBuilder.Entity<ProductDao>().ToTable("Products", "public");
            modelBuilder.Entity<ProfileProductPriceDao>().ToTable("ProfileProductPrices", "public");
            modelBuilder.Entity<ProfileProductOverridePriceDao>().ToTable("ProfileProductOverridePrices", "public");
            modelBuilder.Entity<CartItemDao>().ToTable("CartItems", "public");

            #endregion

            #region company

            modelBuilder.Entity<MemberRoleDao>().ToTable("MemberRoles", "company");

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

            #region CompanyTypeDao

            modelBuilder.Entity<CompanyTypeDao>()
                .HasKey(x => x.Id);

            #endregion

            #region CompanyProfileDao

            modelBuilder.Entity<CompanyProfileDao>()
                .HasKey(x => x.Id);

            // Связь CompanyProfile -> CompanyType (many-to-one)
            modelBuilder.Entity<CompanyProfileDao>()
                .HasOne(cp => cp.CompanyType)
                .WithMany(ct => ct.CompanyProfiles)
                .HasForeignKey(cp => cp.CompanyTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region CompanyVerificationDao

            modelBuilder.Entity<CompanyVerificationDao>()
                .HasKey(x => x.Id);

            #endregion

            #region CompanyVerificationJobStatusDao

            modelBuilder.Entity<CompanyVerificationJobStatusDao>()
                .HasKey(x => x.Id);

            #endregion

            #region CompanyVerificationJobDao

            modelBuilder.Entity<CompanyVerificationJobDao>()
                .HasKey(x => x.Id);

            #endregion

            #region CompanyVerificationJobHistoryDao

            modelBuilder.Entity<CompanyVerificationJobHistoryDao>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<CompanyVerificationJobHistoryDao>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd(); // Configures 'Id' to be auto-generated on add

            #endregion

            #region TechnicalSupervisorProfileDao

            modelBuilder.Entity<TechnicalSupervisorProfileDao>()
                .HasKey(x => x.Id);

            #endregion

            #region CompanyMemberDao

            modelBuilder.Entity<CompanyMemberDao>()
                .HasKey(x => x.Id);

            #endregion

            #region CompanyJoinRequestStatusDao

            modelBuilder.Entity<CompanyJoinRequestStatusDao>()
                .HasKey(x => x.Id);

            #endregion

            #region CompanyJoinRequestDao

            modelBuilder.Entity<CompanyJoinRequestDao>()
                .HasKey(x => x.Id);

            #endregion

            #region FavoriteDao

            modelBuilder.Entity<FavoriteDao>()
                .HasKey(x => x.Id);

            #endregion

            #region ProductCategoryDao

            modelBuilder.Entity<ProductCategoryDao>()
                .HasKey(x => x.Id);

            #endregion

            #region MeasurementUnitTypeDao

            modelBuilder.Entity<MeasurementUnitTypeDao>()
                .HasKey(x => x.Id);

            #endregion

            #region MeasurementUnitDao

            modelBuilder.Entity<MeasurementUnitDao>()
                .HasKey(x => x.Id);

            #endregion

            #region CurrencyDao

            modelBuilder.Entity<CurrencyDao>()
                .HasKey(x => x.Id);

            #endregion

            #region ProductDao

            modelBuilder.Entity<ProductDao>()
                .HasKey(x => x.Id);

            #endregion

            #region ProfileProductPriceDao

            // Составной первичный ключ
            modelBuilder.Entity<ProfileProductPriceDao>()
                .HasKey(x => new { x.ProfileId, x.ProductId, x.StartDate });

            #endregion

            #region ProfileProductOverridePriceDao

            // Составной первичный ключ
            modelBuilder.Entity<ProfileProductOverridePriceDao>()
                .HasKey(x => new { x.ProfileId, x.ProductId, x.StartDate, x.CurrencyId });

            #endregion

            #region CartItemDao

            modelBuilder.Entity<CartItemDao>()
                .HasKey(x => x.Id);

            #endregion

            #endregion

            #region company

            #region MemberRoleDao

            modelBuilder.Entity<MemberRoleDao>()
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

            #region CompanyProfileDao

            // Связь CompanyProfile -> CompanyVerifications (one-to-many)
            // Настраивается через CompanyVerificationDao.CompanyProfile

            #endregion

            #region CompanyVerificationDao

            modelBuilder.Entity<CompanyVerificationDao>()
                .HasOne(x => x.CompanyProfile)
                .WithMany(x => x.CompanyVerifications)
                .HasForeignKey(x => x.CompanyProfileId)
                .IsRequired();

            #endregion

            #region CompanyVerificationJobDao

            modelBuilder.Entity<CompanyVerificationJobDao>()
                .HasOne(x => x.CompanyProfile)
                .WithMany(x => x.CompanyVerificationJobs)
                .HasForeignKey(x => x.CompanyProfileId)
                .IsRequired();

            modelBuilder.Entity<CompanyVerificationJobDao>()
                .HasOne(x => x.Status)
                .WithMany(x => x.Jobs)
                .HasForeignKey(x => x.StatusId)
                .IsRequired();

            modelBuilder.Entity<CompanyVerificationJobDao>()
                .HasOne(x => x.CompanyVerification)
                .WithMany()
                .HasForeignKey(x => x.CompanyVerificationId)
                .IsRequired(false);

            #endregion

            #region CompanyVerificationJobHistoryDao

            modelBuilder.Entity<CompanyVerificationJobHistoryDao>()
                .HasOne(x => x.Job)
                .WithMany(x => x.History)
                .HasForeignKey(x => x.JobId)
                .IsRequired();

            modelBuilder.Entity<CompanyVerificationJobHistoryDao>()
                .HasOne(x => x.Status)
                .WithMany()
                .HasForeignKey(x => x.StatusId)
                .IsRequired();

            #endregion

            #region TechnicalSupervisorProfileDao

            modelBuilder.Entity<TechnicalSupervisorProfileDao>()
                .HasOne(x => x.User)
                .WithMany(x => x.TechnicalSupervisorProfiles)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            #endregion

            #region CompanyMemberDao

            modelBuilder.Entity<CompanyMemberDao>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<CompanyMemberDao>()
                .HasOne(x => x.CompanyProfile)
                .WithMany()
                .HasForeignKey(x => x.CompanyProfileId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<CompanyMemberDao>()
                .HasOne(x => x.MemberRole)
                .WithMany(x => x.CompanyMembers)
                .HasForeignKey(x => x.MemberRoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            #endregion

            #region CompanyJoinRequestDao

            modelBuilder.Entity<CompanyJoinRequestDao>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<CompanyJoinRequestDao>()
                .HasOne(x => x.CompanyProfile)
                .WithMany()
                .HasForeignKey(x => x.CompanyProfileId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<CompanyJoinRequestDao>()
                .HasOne(x => x.Status)
                .WithMany(x => x.CompanyJoinRequests)
                .HasForeignKey(x => x.StatusId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<CompanyJoinRequestDao>()
                .HasOne(x => x.Reviewer)
                .WithMany()
                .HasForeignKey(x => x.ReviewerId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            #endregion

            #region FavoriteDao

            modelBuilder.Entity<FavoriteDao>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            #endregion

            #region ProductCategoryDao

            // Самореференция: родительская категория
            modelBuilder.Entity<ProductCategoryDao>()
                .HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            #endregion

            #region MeasurementUnitDao

            modelBuilder.Entity<MeasurementUnitDao>()
                .HasOne(x => x.Type)
                .WithMany(x => x.MeasurementUnits)
                .HasForeignKey(x => x.TypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            #endregion

            #region ProductDao

            modelBuilder.Entity<ProductDao>()
                .HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<ProductDao>()
                .HasOne(x => x.MeasurementUnit)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.MeasurementUnitId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            #endregion

            #region ProfileProductPriceDao

            modelBuilder.Entity<ProfileProductPriceDao>()
                .HasOne(x => x.Product)
                .WithMany(x => x.ProfileProductPrices)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<ProfileProductPriceDao>()
                .HasOne(x => x.Currency)
                .WithMany(x => x.ProfileProductPrices)
                .HasForeignKey(x => x.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            #endregion

            #region ProfileProductOverridePriceDao

            // FK на базовую цену (составной ключ)
            modelBuilder.Entity<ProfileProductOverridePriceDao>()
                .HasOne(x => x.ProfileProductPrice)
                .WithMany(x => x.OverridePrices)
                .HasForeignKey(x => new { x.ProfileId, x.ProductId, x.StartDate })
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<ProfileProductOverridePriceDao>()
                .HasOne(x => x.Currency)
                .WithMany(x => x.ProfileProductOverridePrices)
                .HasForeignKey(x => x.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            #endregion

            #region CartItemDao

            modelBuilder.Entity<CartItemDao>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<CartItemDao>()
                .HasOne(x => x.Product)
                .WithMany(x => x.CartItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
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
