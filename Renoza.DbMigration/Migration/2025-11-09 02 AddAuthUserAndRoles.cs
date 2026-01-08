using FluentMigrator;
using Renoza.DbMigration.Attributes;
using Renoza.DbMigration.Attributes.Auth;
using Renoza.DbMigration.Enums.Auth;
using System.Data;

namespace Renoza.DbMigration.Migration
{
    [Migration(2025110902, "Добавление пользователей, ролей и таблиц для ролевого доступа")]
    public class AddAuthUserAndRoles : FluentMigrator.Migration
    {
        public override void Up()
        {
            #region Схема auth

            // Создание схемы auth (если еще не существует)
            if (!Schema.Schema("auth").Exists())
            {
                Create.Schema("auth");
            }

            #endregion

            #region Таблица пользователей

            if (!Schema.Schema("auth").Table("Users").Exists())
            {
                Create.Table("Users")
                    .InSchema("auth")
                    .WithColumn("Id").AsGuid().PrimaryKey()
                    .WithColumn("Name").AsString(256).NotNullable().Unique()
                    .WithColumn("DisplayName").AsString(256).NotNullable()
                    .WithColumn("Email").AsString(256).NotNullable().Unique()
                    .WithColumn("IsEmailVerified").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("PhoneNumber").AsString(50).NotNullable().Unique()
                    .WithColumn("PhoneCountryCode").AsString(6).NotNullable()
                    .WithColumn("IsPhoneNumberVerified").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);
            }

            #endregion

            #region Таблица паролей пользователей

            if (!Schema.Schema("auth").Table("UserPasswords").Exists())
            {
                Create.Table("UserPasswords")
                    .InSchema("auth")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("UserId").AsGuid().Nullable().ForeignKey("FK_UserPasswords_UserId", "auth", "Users", "Id").OnDelete(Rule.Cascade)
                    .WithColumn("PasswordHash").AsString(256).NotNullable()
                    .WithColumn("PasswordSalt").AsString(256).NotNullable()
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("ExpiredAt").AsCustom("timestamp with time zone").Nullable();

                Create.Index("IDX_UserPasswords_UserId")
                    .OnTable("UserPasswords")
                    .InSchema("auth")
                    .OnColumn("UserId");

                Execute.Sql(@"
                    CREATE UNIQUE INDEX ""UC_UserPasswords_UserId_IsActive""
                    ON auth.""UserPasswords"" (""UserId"", ""IsActive"")
                    WHERE ""IsActive"" = true;
                ");
            }

            #endregion

            #region Таблица истории паролей пользователей

            if (!Schema.Schema("auth").Table("UserPasswordHistory").Exists())
            {
                Create.Table("UserPasswordHistory")
                    .InSchema("auth")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("UserId").AsGuid().Nullable().ForeignKey("FK_UserPasswordHistory_UserId", "auth", "Users", "Id")
                    .OnDelete(Rule.Cascade)
                    .WithColumn("PasswordHash").AsString(256).NotNullable()
                    .WithColumn("UsedFromAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UsedToAt").AsCustom("timestamp with time zone").Nullable()
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                Create.Index("IDX_UserPasswordHistory_UserId")
                    .OnTable("UserPasswordHistory")
                    .InSchema("auth")
                    .OnColumn("UserId");
            }

            #endregion

            #region Таблица ролей

            // Таблица ролей
            if (!Schema.Schema("auth").Table("Roles").Exists())
            {
                Create.Table("Roles")
                    .InSchema("auth")
                    .WithColumn("Id").AsGuid().PrimaryKey()
                    .WithColumn("Code").AsString(50).NotNullable().Unique()
                    .WithColumn("Name").AsString(256).NotNullable()
                    .WithColumn("DisplayName").AsString(256).Nullable()
                    .WithColumn("IsSystemRole").AsBoolean().NotNullable().WithDefaultValue(true)
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                var now = DateTime.UtcNow;
                var roles = typeof(Role).GetMembers()
                    .Select(x => x.GetCustomAttributes(typeof(RoleDetailsAttribute), false))
                    .SelectMany(x => x.Cast<RoleDetailsAttribute>());

                foreach (var role in roles)
                {
                    Insert.IntoTable("Roles")
                        .InSchema("auth")
                        .Row(new
                        {
                            Id = role.Id,
                            Code = role.Code,
                            Name = role.Name,
                            DisplayName = role.DisplayName,
                            IsSystemRole = role.IsSystemRole,
                            IsActive = role.IsActive,
                            CreatedAt = now
                        });
                }
            }

            #endregion

            #region Таблица связей пользователей и ролей

            // Таблица связей пользователей и ролей (многие-ко-многим)
            if (!Schema.Schema("auth").Table("UserRoles").Exists())
            {
                Create.Table("UserRoles")
                    .InSchema("auth")
                    .WithColumn("UserId").AsGuid().NotNullable().ForeignKey("FK_UserRoles_UserId", "auth", "Users", "Id").OnDelete(Rule.Cascade)
                    .WithColumn("RoleId").AsGuid().NotNullable().ForeignKey("FK_UserRoles_RoleId", "auth", "Roles", "Id").OnDelete(Rule.Cascade)
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                // Композитный первичный ключ
                Create.PrimaryKey("PK_UserRoles")
                    .OnTable("UserRoles")
                    .WithSchema("auth")
                    .Columns("UserId", "RoleId");

                // Индексы для оптимизации запросов
                Create.Index("IDX_UserRoles_UserId")
                    .OnTable("UserRoles")
                    .InSchema("auth")
                    .OnColumn("UserId");

                Create.Index("IDX_UserRoles_RoleId")
                    .OnTable("UserRoles")
                    .InSchema("auth")
                    .OnColumn("RoleId");
            }

            #endregion

            #region Таблица операций

            Create.Table("Operations")
                .InSchema("auth")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Name").AsString(256).NotNullable()
                .WithColumn("DisplayName").AsString(256).NotNullable();

            Create.Index("IDX_Operations_Name")
                .OnTable("Operations")
                .InSchema("auth")
                .OnColumn("Name")
                .Unique();

            // Предзаполнение операций из enum
            var operations = typeof(Operation).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(OperationDetailsAttribute), false))
                .SelectMany(x => x.Cast<OperationDetailsAttribute>());

            foreach (var operation in operations)
            {
                Insert.IntoTable("Operations").InSchema("auth")
                    .Row(new
                    {
                        Id = operation.Id,
                        Name = operation.Name,
                        DisplayName = operation.DisplayName
                    });
            }

            #endregion

            #region Таблица ресурсов

            Create.Table("Resources")
                .InSchema("auth")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Name").AsString(256).NotNullable()
                .WithColumn("DisplayName").AsString(256).NotNullable()
                .WithColumn("ParentId").AsGuid().Nullable()
                    .ForeignKey("FK_Resources_ParentId", "auth", "Resources", "Id");

            Create.Index("IDX_Resources_Name")
                .OnTable("Resources")
                .InSchema("auth")
                .OnColumn("Name")
                .Unique();

            Create.Index("IDX_Resources_ParentId")
                .OnTable("Resources")
                .InSchema("auth")
                .OnColumn("ParentId");

            // Предзаполнение ресурсов из enum
            var resources = typeof(Resource).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(ResourceDetailsAttribute), false))
                .SelectMany(x => x.Cast<ResourceDetailsAttribute>());

            foreach (var resource in resources)
            {
                Insert.IntoTable("Resources").InSchema("auth")
                    .Row(new
                    {
                        Id = resource.Id,
                        Name = resource.Name,
                        DisplayName = resource.DisplayName,
                        ParentId = resource.ParentId
                    });
            }

            #endregion

            #region Таблица связи ресурсов и операций

            Create.Table("ResourceOperations")
                .InSchema("auth")
                .WithColumn("ResourceId").AsGuid().NotNullable()
                    .ForeignKey("FK_ResourceOperations_ResourceId", "auth", "Resources", "Id").OnDelete(Rule.Cascade)
                .WithColumn("OperationId").AsGuid().NotNullable()
                    .ForeignKey("FK_ResourceOperations_OperationId", "auth", "Operations", "Id").OnDelete(Rule.Cascade);

            Create.PrimaryKey("PK_ResourceOperations")
                .OnTable("ResourceOperations")
                .WithSchema("auth")
                .Columns("ResourceId", "OperationId");

            // Предзаполнение ResourceOperations (определяем какие операции возможны для каждого ресурса)
            InsertResourceOperations();

            #endregion

            #region Таблица грантов доступа

            Create.Table("AccessGrants")
                .InSchema("auth")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Name").AsString(256).NotNullable()
                .WithColumn("DisplayName").AsString(256).NotNullable();

            Create.Index("IDX_AccessGrants_Name")
                .OnTable("AccessGrants")
                .InSchema("auth")
                .OnColumn("Name")
                .Unique();

            // Предзаполнение грантов доступа из enum
            var accessGrants = typeof(AccessGrant).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(AccessGrantDetailsAttribute), false))
                .SelectMany(x => x.Cast<AccessGrantDetailsAttribute>());

            foreach (var grant in accessGrants)
            {
                Insert.IntoTable("AccessGrants").InSchema("auth")
                    .Row(new
                    {
                        Id = grant.Id,
                        Name = grant.Name,
                        DisplayName = grant.DisplayName
                    });
            }

            #endregion

            #region Таблица разрешений

            Create.Table("Permissions")
                .InSchema("auth")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("ResourceId").AsGuid().NotNullable()
                    .ForeignKey("FK_Permissions_ResourceId", "auth", "Resources", "Id").OnDelete(Rule.Cascade)
                .WithColumn("ResourceValue").AsString(256).Nullable()
                .WithColumn("AccessGrantId").AsGuid().Nullable()
                    .ForeignKey("FK_Permissions_AccessGrantId", "auth", "AccessGrants", "Id").OnDelete(Rule.Cascade);

            Create.Index("IDX_Permissions_ResourceId")
                .OnTable("Permissions")
                .InSchema("auth")
                .OnColumn("ResourceId");

            Create.Index("IDX_Permissions_AccessGrantId")
                .OnTable("Permissions")
                .InSchema("auth")
                .OnColumn("AccessGrantId");

            #endregion

            #region Таблица операций для разрешений

            Create.Table("PermissionOperations")
                .InSchema("auth")
                .WithColumn("PermissionId").AsGuid().NotNullable()
                    .ForeignKey("FK_PermissionOperations_PermissionId", "auth", "Permissions", "Id").OnDelete(Rule.Cascade)
                .WithColumn("OperationId").AsGuid().NotNullable()
                    .ForeignKey("FK_PermissionOperations_OperationId", "auth", "Operations", "Id").OnDelete(Rule.Cascade);

            Create.PrimaryKey("PK_PermissionOperations")
                .OnTable("PermissionOperations")
                .WithSchema("auth")
                .Columns("PermissionId", "OperationId");

            #endregion

            #region Таблица связи ролей и грантов доступа

            Create.Table("RoleAccessGrants")
                .InSchema("auth")
                .WithColumn("RoleId").AsGuid().NotNullable()
                    .ForeignKey("FK_RoleAccessGrants_RoleId", "auth", "Roles", "Id").OnDelete(Rule.Cascade)
                .WithColumn("AccessGrantId").AsGuid().NotNullable()
                    .ForeignKey("FK_RoleAccessGrants_AccessGrantId", "auth", "AccessGrants", "Id").OnDelete(Rule.Cascade);

            Create.PrimaryKey("PK_RoleAccessGrants")
                .OnTable("RoleAccessGrants")
                .WithSchema("auth")
                .Columns("RoleId", "AccessGrantId");

            Create.Index("IDX_RoleAccessGrants_RoleId")
                .OnTable("RoleAccessGrants")
                .InSchema("auth")
                .OnColumn("RoleId");

            Create.Index("IDX_RoleAccessGrants_AccessGrantId")
                .OnTable("RoleAccessGrants")
                .InSchema("auth")
                .OnColumn("AccessGrantId");

            #endregion

            #region Предзаполнение разрешений и связей ролей

            InsertPermissionsAndOperations();
            InsertRoleAccessGrants();

            #endregion

            #region Предзаполнение пользователем

            InsertPrepopulatedUser();

            #endregion
        }

        public override void Down()
        {
            // Удаляем таблицы в обратном порядке
            if (Schema.Schema("auth").Table("RoleAccessGrants").Exists())
            {
                Delete.Index("IDX_RoleAccessGrants_RoleId").OnTable("RoleAccessGrants").InSchema("auth");
                Delete.Index("IDX_RoleAccessGrants_AccessGrantId").OnTable("RoleAccessGrants").InSchema("auth");
                Delete.Table("RoleAccessGrants").InSchema("auth");
            }

            if (Schema.Schema("auth").Table("PermissionOperations").Exists())
            {
                Delete.Table("PermissionOperations").InSchema("auth");
            }

            if (Schema.Schema("auth").Table("Permissions").Exists())
            {
                Delete.Index("IDX_Permissions_ResourceId").OnTable("Permissions").InSchema("auth");
                Delete.Index("IDX_Permissions_AccessGrantId").OnTable("Permissions").InSchema("auth");
                Delete.Table("Permissions").InSchema("auth");
            }

            if (Schema.Schema("auth").Table("AccessGrants").Exists())
            {
                Delete.Index("IDX_AccessGrants_Name").OnTable("AccessGrants").InSchema("auth");
                Delete.Table("AccessGrants").InSchema("auth");
            }

            if (Schema.Schema("auth").Table("ResourceOperations").Exists())
            {
                Delete.Table("ResourceOperations").InSchema("auth");
            }

            if (Schema.Schema("auth").Table("Resources").Exists())
            {
                Delete.Index("IDX_Resources_Name").OnTable("Resources").InSchema("auth");
                Delete.Index("IDX_Resources_ParentId").OnTable("Resources").InSchema("auth");
                Delete.Table("Resources").InSchema("auth");
            }

            if (Schema.Schema("auth").Table("Operations").Exists())
            {
                Delete.Index("IDX_Operations_Name").OnTable("Operations").InSchema("auth");
                Delete.Table("Operations").InSchema("auth");
            }

            // Удаляем связи пользователей и ролей
            if (Schema.Schema("auth").Table("UserRoles").Exists())
            {
                if (Schema.Schema("auth").Table("UserRoles").Index("IDX_UserRoles_UserId").Exists())
                {
                    Delete.Index("IDX_UserRoles_UserId").OnTable("UserRoles").InSchema("auth");
                }
                if (Schema.Schema("auth").Table("UserRoles").Index("IDX_UserRoles_RoleId").Exists())
                {
                    Delete.Index("IDX_UserRoles_RoleId").OnTable("UserRoles").InSchema("auth");
                }
                Delete.Table("UserRoles").InSchema("auth");
            }

            // Удаляем роли
            if (Schema.Schema("auth").Table("Roles").Exists())
            {
                Delete.Table("Roles").InSchema("auth");
            }

            if (Schema.Schema("auth").Table("UserPasswordHistory").Exists())
            {
                if (Schema.Schema("auth").Table("UserPasswordHistory").Index("IDX_UserPasswordHistory_UserId").Exists())
                {
                    Delete.Index("IDX_UserPasswordHistory_UserId").OnTable("UserPasswordHistory").InSchema("auth");
                }

                Delete.Table("UserPasswordHistory").InSchema("auth");
            }

            if (Schema.Schema("auth").Table("UserPasswords").Exists())
            {
                if (Schema.Schema("auth").Table("UserPasswords").Index("IDX_UserPasswords_UserId").Exists())
                {
                    Delete.Index("IDX_UserPasswords_UserId").OnTable("UserPasswords").InSchema("auth");
                }
                if (Schema.Schema("auth").Table("UserPasswords").Index("UC_UserPasswords_UserId_IsActive").Exists())
                {
                    Delete.Index("UC_UserPasswords_UserId_IsActive").OnTable("UserPasswords").InSchema("auth");
                }

                Delete.Table("UserPasswords").InSchema("auth");
            }

            if (Schema.Schema("auth").Table("Users").Exists())
            {
                Delete.Table("Users").InSchema("auth");
            }

            if (Schema.Schema("auth").Exists())
            {
                Delete.Schema("auth");
            }
        }

        /// <summary>
        /// Определяет какие операции доступны для каждого ресурса
        /// </summary>
        private void InsertResourceOperations()
        {
            var resourceAttrs = typeof(Resource).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(ResourceDetailsAttribute), false))
                .SelectMany(x => x.Cast<ResourceDetailsAttribute>())
                .ToDictionary(x => x.Id, x => x);

            var operationAttrs = typeof(Operation).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(OperationDetailsAttribute), false))
                .SelectMany(x => x.Cast<OperationDetailsAttribute>())
                .ToDictionary(x => x.Name, x => x.Id);

            // Управление пользователями - create, read, update, delete
            AddResourceOperation(resourceAttrs, "users.manage", operationAttrs, "create");
            AddResourceOperation(resourceAttrs, "users.manage", operationAttrs, "read");
            AddResourceOperation(resourceAttrs, "users.manage", operationAttrs, "update");
            AddResourceOperation(resourceAttrs, "users.manage", operationAttrs, "delete");

            // Управление ролями - create, read, update, delete
            AddResourceOperation(resourceAttrs, "roles.manage", operationAttrs, "create");
            AddResourceOperation(resourceAttrs, "roles.manage", operationAttrs, "read");
            AddResourceOperation(resourceAttrs, "roles.manage", operationAttrs, "update");
            AddResourceOperation(resourceAttrs, "roles.manage", operationAttrs, "delete");

            // Настройки системы - read, update
            AddResourceOperation(resourceAttrs, "system.settings", operationAttrs, "read");
            AddResourceOperation(resourceAttrs, "system.settings", operationAttrs, "update");

            // Логи системы - read, export
            AddResourceOperation(resourceAttrs, "system.logs", operationAttrs, "read");
            AddResourceOperation(resourceAttrs, "system.logs", operationAttrs, "export");
        }

        /// <summary>
        /// Добавляет связь между ресурсом и операцией
        /// </summary>
        /// <param name="resources">Словарь ресурсов по идентификатору</param>
        /// <param name="resourceName">Имя ресурса</param>
        /// <param name="operations">Словарь операций по имени</param>
        /// <param name="operationName">Имя операции</param>
        private void AddResourceOperation(
            Dictionary<Guid, ResourceDetailsAttribute> resources,
            string resourceName,
            Dictionary<string, Guid> operations,
            string operationName)
        {
            var resource = resources.Values.First(r => r.Name == resourceName);
            var operation = operations[operationName];

            Insert.IntoTable("ResourceOperations").InSchema("auth")
                .Row(new { ResourceId = resource.Id, OperationId = operation });
        }

        /// <summary>
        /// Создает разрешения и связывает их с операциями для каждого AccessGrant
        /// </summary>
        private void InsertPermissionsAndOperations()
        {
            var resourceAttrs = typeof(Resource).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(ResourceDetailsAttribute), false))
                .SelectMany(x => x.Cast<ResourceDetailsAttribute>())
                .ToDictionary(x => x.Name, x => x.Id);

            var operationAttrs = typeof(Operation).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(OperationDetailsAttribute), false))
                .SelectMany(x => x.Cast<OperationDetailsAttribute>())
                .ToDictionary(x => x.Name, x => x.Id);

            // AdminFullAccess - полный доступ ко всем ресурсам
            InsertAdminFullPermissions(GetAccessGrantId(AccessGrant.AdminFullAccess), resourceAttrs, operationAttrs);

            // CustomerAccess, WorkerAccess, TechnicalSupervisorAccess - пока без разрешений
            // Они будут определены позже в зависимости от требований
        }

        /// <summary>
        /// Создает разрешения администратора с полным доступом ко всем ресурсам
        /// </summary>
        /// <param name="grantId">Идентификатор гранта доступа администратора</param>
        /// <param name="resources">Словарь ресурсов по имени</param>
        /// <param name="operations">Словарь операций по имени</param>
        private void InsertAdminFullPermissions(Guid grantId, Dictionary<string, Guid> resources, Dictionary<string, Guid> operations)
        {
            // Administrator имеет все операции для всех ресурсов
            var allOperations = operations.Values.ToArray();

            foreach (var resourceId in resources.Values)
            {
                var permId = Guid.NewGuid();
                Insert.IntoTable("Permissions").InSchema("auth")
                    .Row(new { Id = permId, ResourceId = resourceId, ResourceValue = (string?)null, AccessGrantId = grantId });

                // Добавляем все операции, которые определены для этого ресурса в ResourceOperations
                foreach (var opId in allOperations)
                {
                    Execute.Sql($@"
                        INSERT INTO auth.""PermissionOperations"" (""PermissionId"", ""OperationId"")
                        SELECT '{permId}', '{opId}'
                        WHERE EXISTS (
                            SELECT 1 FROM auth.""ResourceOperations""
                            WHERE ""ResourceId"" = '{resourceId}' AND ""OperationId"" = '{opId}'
                        )");
                }
            }
        }

        /// <summary>
        /// Связывает роли пользователей с грантами доступа
        /// </summary>
        private void InsertRoleAccessGrants()
        {
            // Administrator -> AdminFullAccess
            Insert.IntoTable("RoleAccessGrants").InSchema("auth")
                .Row(new { RoleId = GetRoleId(Role.Administrator), AccessGrantId = GetAccessGrantId(AccessGrant.AdminFullAccess) });

            // Customer -> CustomerAccess
            Insert.IntoTable("RoleAccessGrants").InSchema("auth")
                .Row(new { RoleId = GetRoleId(Role.Customer), AccessGrantId = GetAccessGrantId(AccessGrant.CustomerAccess) });

            // Worker -> WorkerAccess
            Insert.IntoTable("RoleAccessGrants").InSchema("auth")
                .Row(new { RoleId = GetRoleId(Role.Worker), AccessGrantId = GetAccessGrantId(AccessGrant.WorkerAccess) });

            // TechnicalSupervisor -> TechnicalSupervisorAccess
            Insert.IntoTable("RoleAccessGrants").InSchema("auth")
                .Row(new { RoleId = GetRoleId(Role.TechnicalSupervisor), AccessGrantId = GetAccessGrantId(AccessGrant.TechnicalSupervisorAccess) });
        }

        /// <summary>
        /// Добавляет системного пользователя
        /// </summary>
        private void InsertPrepopulatedUser()
        {
            var userId = Guid.NewGuid();
            var roleMember = Role.Administrator.GetType()
                .GetMember(Role.Administrator.ToString())
                .First();
            var roleId = ((RoleDetailsAttribute)roleMember
                .GetCustomAttributes(typeof(RoleDetailsAttribute), false)
                .First())
                .Id;
            var now = DateTime.UtcNow;

            Insert.IntoTable("Users")
                .InSchema("auth")
                .Row(new
                {
                    Id = userId,
                    Name = "test_user",
                    DisplayName = "Тестовый пользователь",
                    Email = "test_user@test.ru",
                    IsEmailVerified = true,
                    PhoneNumber = "9999999999",
                    PhoneCountryCode = "7",
                    IsPhoneNumberVerified = true,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                })
            ;

            // тут внесён пароль Qwerty123!
            Insert.IntoTable("UserPasswords")
                .InSchema("auth")
                .Row(new
                {
                    UserId = userId,
                    PasswordHash = "FkAvVqbwhy0v0vz+5sLrl6ia8p/RESG8yqPjGcm5jHW6OAA7J7nCbMJG5u7H1+ketml6fDPFzjz2Hu720m7Hbw==",
                    PasswordSalt = "ix3Zt5I+TcJZwR/3sSKubG/RlgGmj8l2gZ+ZcDgIvapzcLh6KeqYEkT7OyHUFcAioz/M6mvCXtzDTXaYkqmTIqeSJhJp5txOIXNcdm7i0mDPjqErwj3FRXCB2vLGHckb/BmXXv3tYbPs2SeMWU9eWKvLUsKJrcHzWcplXnStids=",
                    IsActive = true,
                    CreatedAt = now
                })
            ;

            Insert.IntoTable("UserPasswordHistory")
                .InSchema("auth")
                .Row(new
                {
                    UserId = userId,
                    PasswordHash = "FkAvVqbwhy0v0vz+5sLrl6ia8p/RESG8yqPjGcm5jHW6OAA7J7nCbMJG5u7H1+ketml6fDPFzjz2Hu720m7Hbw==",
                    UsedFromAt = now,
                    CreatedAt = now
                })
            ;

            // Назначаем роль администратора
            Insert.IntoTable("UserRoles")
                .InSchema("auth")
                .Row(new
                {
                    UserId = userId,
                    RoleId = roleId,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                })
            ;
        }

        /// <summary>
        /// Получить идентификатор роли по значению enum через рефлексию
        /// </summary>
        /// <param name="role">Значение enum роли</param>
        /// <returns>Идентификатор роли</returns>
        private Guid GetRoleId(Role role)
        {
            var memberInfo = typeof(Role).GetMember(role.ToString()).FirstOrDefault();
            var attribute = memberInfo?.GetCustomAttributes(typeof(RoleDetailsAttribute), false)
                .FirstOrDefault() as RoleDetailsAttribute;

            if (attribute == null)
                throw new InvalidOperationException($"Атрибут RoleDetailsAttribute не найден для роли {role}");

            return attribute.Id;
        }

        /// <summary>
        /// Получить идентификатор гранта доступа по значению enum через рефлексию
        /// </summary>
        /// <param name="accessGrant">Значение enum гранта доступа</param>
        /// <returns>Идентификатор гранта доступа</returns>
        private Guid GetAccessGrantId(AccessGrant accessGrant)
        {
            var memberInfo = typeof(AccessGrant).GetMember(accessGrant.ToString()).FirstOrDefault();
            var attribute = memberInfo?.GetCustomAttributes(typeof(AccessGrantDetailsAttribute), false)
                .FirstOrDefault() as AccessGrantDetailsAttribute;

            if (attribute == null)
                throw new InvalidOperationException($"Атрибут AccessGrantDetailsAttribute не найден для гранта доступа {accessGrant}");

            return attribute.Id;
        }
    }
}
