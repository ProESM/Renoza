using System.Data;
using FluentMigrator;
using Renoza.DbMigration.Attributes.Company;
using Renoza.DbMigration.Enums.Company;

namespace Renoza.DbMigration.Migration
{
    [Migration(2025110904, "Добавление системы разрешений для компаний")]
    public class AddCompanyPermissionsTables : FluentMigrator.Migration
    {
        public override void Up()
        {
            #region Схема company

            // Создание схемы company (если еще не существует)
            if (!Schema.Schema("company").Exists())
            {
                Create.Schema("company");
            }

            #endregion

            #region Таблица операций

            Create.Table("Operations")
                .InSchema("company")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Name").AsString(256).NotNullable()
                .WithColumn("DisplayName").AsString(256).NotNullable();

            Create.Index("IDX_Operations_Name")
                .OnTable("Operations")
                .InSchema("company")
                .OnColumn("Name")
                .Unique();

            // Предзаполнение операций из enum
            var operations = typeof(CompanyOperation).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(CompanyOperationDetailsAttribute), false))
                .SelectMany(x => x.Cast<CompanyOperationDetailsAttribute>());

            foreach (var operation in operations)
            {
                Insert.IntoTable("Operations").InSchema("company")
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
                .InSchema("company")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Name").AsString(256).NotNullable()
                .WithColumn("DisplayName").AsString(256).NotNullable()
                .WithColumn("ParentId").AsGuid().Nullable()
                    .ForeignKey("FK_Resources_ParentId", "company", "Resources", "Id");

            Create.Index("IDX_Resources_Name")
                .OnTable("Resources")
                .InSchema("company")
                .OnColumn("Name")
                .Unique();

            Create.Index("IDX_Resources_ParentId")
                .OnTable("Resources")
                .InSchema("company")
                .OnColumn("ParentId");

            // Предзаполнение ресурсов из enum
            var resources = typeof(CompanyResource).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(CompanyResourceDetailsAttribute), false))
                .SelectMany(x => x.Cast<CompanyResourceDetailsAttribute>());

            foreach (var resource in resources)
            {
                Insert.IntoTable("Resources").InSchema("company")
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
                .InSchema("company")
                .WithColumn("ResourceId").AsGuid().NotNullable()
                    .ForeignKey("FK_ResourceOperations_ResourceId", "company", "Resources", "Id").OnDelete(Rule.Cascade)
                .WithColumn("OperationId").AsGuid().NotNullable()
                    .ForeignKey("FK_ResourceOperations_OperationId", "company", "Operations", "Id").OnDelete(Rule.Cascade);

            Create.PrimaryKey("PK_ResourceOperations")
                .OnTable("ResourceOperations")
                .WithSchema("company")
                .Columns("ResourceId", "OperationId");

            // Предзаполнение ResourceOperations (определяем какие операции возможны для каждого ресурса)
            InsertResourceOperations();

            #endregion

            #region Таблица грантов доступа

            Create.Table("AccessGrants")
                .InSchema("company")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Name").AsString(256).NotNullable()
                .WithColumn("DisplayName").AsString(256).NotNullable();

            Create.Index("IDX_AccessGrants_Name")
                .OnTable("AccessGrants")
                .InSchema("company")
                .OnColumn("Name")
                .Unique();

            // Предзаполнение грантов доступа из enum
            var accessGrants = typeof(CompanyAccessGrant).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(CompanyAccessGrantDetailsAttribute), false))
                .SelectMany(x => x.Cast<CompanyAccessGrantDetailsAttribute>());

            foreach (var grant in accessGrants)
            {
                Insert.IntoTable("AccessGrants").InSchema("company")
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
                .InSchema("company")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("ResourceId").AsGuid().NotNullable()
                    .ForeignKey("FK_Permissions_ResourceId", "company", "Resources", "Id").OnDelete(Rule.Cascade)
                .WithColumn("ResourceValue").AsString(256).Nullable()
                .WithColumn("AccessGrantId").AsGuid().Nullable()
                    .ForeignKey("FK_Permissions_AccessGrantId", "company", "AccessGrants", "Id").OnDelete(Rule.Cascade);

            Create.Index("IDX_Permissions_ResourceId")
                .OnTable("Permissions")
                .InSchema("company")
                .OnColumn("ResourceId");

            Create.Index("IDX_Permissions_AccessGrantId")
                .OnTable("Permissions")
                .InSchema("company")
                .OnColumn("AccessGrantId");

            #endregion

            #region Таблица операций для разрешений

            Create.Table("PermissionOperations")
                .InSchema("company")
                .WithColumn("PermissionId").AsGuid().NotNullable()
                    .ForeignKey("FK_PermissionOperations_PermissionId", "company", "Permissions", "Id").OnDelete(Rule.Cascade)
                .WithColumn("OperationId").AsGuid().NotNullable()
                    .ForeignKey("FK_PermissionOperations_OperationId", "company", "Operations", "Id").OnDelete(Rule.Cascade);

            Create.PrimaryKey("PK_PermissionOperations")
                .OnTable("PermissionOperations")
                .WithSchema("company")
                .Columns("PermissionId", "OperationId");

            #endregion

            #region Таблица ролей участников компании (MemberRoles)

            Create.Table("MemberRoles")
                .InSchema("company")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Code").AsString(50).NotNullable().Unique()
                .WithColumn("Name").AsString(256).NotNullable()
                .WithColumn("DisplayName").AsString(256).Nullable()
                .WithColumn("IsSystemRole").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            // Предзаполнение ролей участников из enum
            var memberRoles = typeof(CompanyMemberRole).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(CompanyMemberRoleDetailsAttribute), false))
                .SelectMany(x => x.Cast<CompanyMemberRoleDetailsAttribute>());

            foreach (var role in memberRoles)
            {
                Insert.IntoTable("MemberRoles").InSchema("company")
                    .Row(new
                    {
                        Id = role.Id,
                        Code = role.Code,
                        Name = role.Name,
                        DisplayName = role.DisplayName,
                        IsSystemRole = role.IsSystemRole,
                        IsActive = role.IsActive,
                        CreatedAt = DateTime.UtcNow
                    });
            }
            
            #endregion

            #region Предзаполнение разрешений

            InsertPermissionsAndOperations();

            #endregion
        }

        public override void Down()
        {
            #region Схема company

            // Удаляем таблицы в обратном порядке
            if (Schema.Schema("company").Table("PermissionOperations").Exists())
            {
                Delete.Table("PermissionOperations").InSchema("company");
            }

            if (Schema.Schema("company").Table("Permissions").Exists())
            {
                Delete.Index("IDX_Permissions_ResourceId").OnTable("Permissions").InSchema("company");
                Delete.Index("IDX_Permissions_AccessGrantId").OnTable("Permissions").InSchema("company");
                Delete.Table("Permissions").InSchema("company");
            }

            if (Schema.Schema("company").Table("AccessGrants").Exists())
            {
                Delete.Index("IDX_AccessGrants_Name").OnTable("AccessGrants").InSchema("company");
                Delete.Table("AccessGrants").InSchema("company");
            }

            if (Schema.Schema("company").Table("ResourceOperations").Exists())
            {
                Delete.Table("ResourceOperations").InSchema("company");
            }

            if (Schema.Schema("company").Table("Resources").Exists())
            {
                Delete.Index("IDX_Resources_Name").OnTable("Resources").InSchema("company");
                Delete.Index("IDX_Resources_ParentId").OnTable("Resources").InSchema("company");
                Delete.Table("Resources").InSchema("company");
            }

            if (Schema.Schema("company").Table("Operations").Exists())
            {
                Delete.Index("IDX_Operations_Name").OnTable("Operations").InSchema("company");
                Delete.Table("Operations").InSchema("company");
            }

            if (Schema.Schema("company").Table("MemberRoles").Exists())
            {
                Delete.Index("IDX_MemberRoles_Name").OnTable("MemberRoles").InSchema("company");
                Delete.Table("MemberRoles").InSchema("company");
            }

            // Удаляем схему
            Execute.Sql("DROP SCHEMA IF EXISTS company CASCADE");

            #endregion
        }

        /// <summary>
        /// Определяет какие операции доступны для каждого ресурса
        /// </summary>
        private void InsertResourceOperations()
        {
            var resourceAttrs = typeof(CompanyResource).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(CompanyResourceDetailsAttribute), false))
                .SelectMany(x => x.Cast<CompanyResourceDetailsAttribute>())
                .ToDictionary(x => x.Id, x => x);

            var operationAttrs = typeof(CompanyOperation).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(CompanyOperationDetailsAttribute), false))
                .SelectMany(x => x.Cast<CompanyOperationDetailsAttribute>())
                .ToDictionary(x => x.Name, x => x.Id);

            // Профиль компании - read, update
            AddResourceOperation(resourceAttrs, "company_profile.info", operationAttrs, "read");
            AddResourceOperation(resourceAttrs, "company_profile.info", operationAttrs, "update");

            // Участники компании - read, invite, remove, update (изменение ролей)
            AddResourceOperation(resourceAttrs, "company_profile.members", operationAttrs, "read");
            AddResourceOperation(resourceAttrs, "company_profile.members", operationAttrs, "invite");
            AddResourceOperation(resourceAttrs, "company_profile.members", operationAttrs, "remove");
            AddResourceOperation(resourceAttrs, "company_profile.members", operationAttrs, "update");

            // Все заказы - все операции
            AddResourceOperation(resourceAttrs, "orders.all", operationAttrs, "create");
            AddResourceOperation(resourceAttrs, "orders.all", operationAttrs, "read");
            AddResourceOperation(resourceAttrs, "orders.all", operationAttrs, "update");
            AddResourceOperation(resourceAttrs, "orders.all", operationAttrs, "delete");
            AddResourceOperation(resourceAttrs, "orders.all", operationAttrs, "assign");
            AddResourceOperation(resourceAttrs, "orders.all", operationAttrs, "execute");

            // Назначенные заказы - read, update, execute
            AddResourceOperation(resourceAttrs, "orders.assigned", operationAttrs, "read");
            AddResourceOperation(resourceAttrs, "orders.assigned", operationAttrs, "update");
            AddResourceOperation(resourceAttrs, "orders.assigned", operationAttrs, "execute");

            // Кассовые чеки - create, read
            AddResourceOperation(resourceAttrs, "finance.receipts", operationAttrs, "create");
            AddResourceOperation(resourceAttrs, "finance.receipts", operationAttrs, "read");

            // Финансовые отчеты - read, export
            AddResourceOperation(resourceAttrs, "finance.reports", operationAttrs, "read");
            AddResourceOperation(resourceAttrs, "finance.reports", operationAttrs, "export");

            // Документы - create, read, update, delete
            AddResourceOperation(resourceAttrs, "documents", operationAttrs, "create");
            AddResourceOperation(resourceAttrs, "documents", operationAttrs, "read");
            AddResourceOperation(resourceAttrs, "documents", operationAttrs, "update");
            AddResourceOperation(resourceAttrs, "documents", operationAttrs, "delete");

            // Шаблоны документов - create, read, update, delete
            AddResourceOperation(resourceAttrs, "documents.templates", operationAttrs, "create");
            AddResourceOperation(resourceAttrs, "documents.templates", operationAttrs, "read");
            AddResourceOperation(resourceAttrs, "documents.templates", operationAttrs, "update");
            AddResourceOperation(resourceAttrs, "documents.templates", operationAttrs, "delete");
        }

        private void AddResourceOperation(
            Dictionary<Guid, CompanyResourceDetailsAttribute> resources,
            string resourceName,
            Dictionary<string, Guid> operations,
            string operationName)
        {
            var resource = resources.Values.First(r => r.Name == resourceName);
            var operation = operations[operationName];

            Insert.IntoTable("ResourceOperations").InSchema("company")
                .Row(new { ResourceId = resource.Id, OperationId = operation });
        }

        /// <summary>
        /// Создает разрешения и связывает их с операциями для каждого AccessGrant
        /// </summary>
        private void InsertPermissionsAndOperations()
        {
            var grantAttrs = typeof(CompanyAccessGrant).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(CompanyAccessGrantDetailsAttribute), false))
                .SelectMany(x => x.Cast<CompanyAccessGrantDetailsAttribute>())
                .ToDictionary(x => x.Name, x => x);

            var resourceAttrs = typeof(CompanyResource).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(CompanyResourceDetailsAttribute), false))
                .SelectMany(x => x.Cast<CompanyResourceDetailsAttribute>())
                .ToDictionary(x => x.Name, x => x.Id);

            var operationAttrs = typeof(CompanyOperation).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(CompanyOperationDetailsAttribute), false))
                .SelectMany(x => x.Cast<CompanyOperationDetailsAttribute>())
                .ToDictionary(x => x.Name, x => x.Id);

            // Owner - полный доступ ко всем ресурсам
            InsertOwnerPermissions(grantAttrs["company_owner_full"].Id, resourceAttrs, operationAttrs);

            // Manager - управление заказами
            InsertManagerOrdersPermissions(grantAttrs["company_manager_orders"].Id, resourceAttrs, operationAttrs);

            // Manager - управление участниками
            InsertManagerMembersPermissions(grantAttrs["company_manager_members"].Id, resourceAttrs, operationAttrs);

            // Manager - просмотр финансов
            InsertManagerFinancePermissions(grantAttrs["company_manager_finance_view"].Id, resourceAttrs, operationAttrs);

            // Manager - управление документами
            InsertManagerDocumentsPermissions(grantAttrs["company_manager_documents"].Id, resourceAttrs, operationAttrs);

            // Employee - просмотр заказов
            InsertEmployeeOrdersViewPermissions(grantAttrs["company_employee_orders_view"].Id, resourceAttrs, operationAttrs);

            // Employee - работа с назначенными заказами
            InsertEmployeeOrdersAssignedPermissions(grantAttrs["company_employee_orders_assigned"].Id, resourceAttrs, operationAttrs);

            // Employee - просмотр и загрузка чеков
            InsertEmployeeReceiptsPermissions(grantAttrs["company_employee_receipts"].Id, resourceAttrs, operationAttrs);
        }

        private void InsertOwnerPermissions(Guid grantId, Dictionary<string, Guid> resources, Dictionary<string, Guid> operations)
        {
            // Owner имеет все операции для всех ресурсов
            var allOperations = operations.Values.ToArray();

            foreach (var resourceId in resources.Values)
            {
                var permId = Guid.NewGuid();
                Insert.IntoTable("Permissions").InSchema("company")
                    .Row(new { Id = permId, ResourceId = resourceId, ResourceValue = (string?)null, AccessGrantId = grantId });

                // Добавляем все операции, которые определены для этого ресурса в ResourceOperations
                foreach (var opId in allOperations)
                {
                    Execute.Sql($@"
                        INSERT INTO company.""PermissionOperations"" (""PermissionId"", ""OperationId"")
                        SELECT '{permId}', '{opId}'
                        WHERE EXISTS (
                            SELECT 1 FROM company.""ResourceOperations""
                            WHERE ""ResourceId"" = '{resourceId}' AND ""OperationId"" = '{opId}'
                        )");
                }
            }
        }

        private void InsertManagerOrdersPermissions(Guid grantId, Dictionary<string, Guid> resources, Dictionary<string, Guid> operations)
        {
            var permId = Guid.NewGuid();
            Insert.IntoTable("Permissions").InSchema("company")
                .Row(new { Id = permId, ResourceId = resources["orders.all"], ResourceValue = (string?)null, AccessGrantId = grantId });

            Insert.IntoTable("PermissionOperations").InSchema("company")
                .Row(new { PermissionId = permId, OperationId = operations["create"] });
            Insert.IntoTable("PermissionOperations").InSchema("company")
                .Row(new { PermissionId = permId, OperationId = operations["read"] });
            Insert.IntoTable("PermissionOperations").InSchema("company")
                .Row(new { PermissionId = permId, OperationId = operations["update"] });
            Insert.IntoTable("PermissionOperations").InSchema("company")
                .Row(new { PermissionId = permId, OperationId = operations["assign"] });
            Insert.IntoTable("PermissionOperations").InSchema("company")
                .Row(new { PermissionId = permId, OperationId = operations["execute"] });
        }

        private void InsertManagerMembersPermissions(Guid grantId, Dictionary<string, Guid> resources, Dictionary<string, Guid> operations)
        {
            var permId = Guid.NewGuid();
            Insert.IntoTable("Permissions").InSchema("company")
                .Row(new { Id = permId, ResourceId = resources["company_profile.members"], ResourceValue = (string?)null, AccessGrantId = grantId });

            Insert.IntoTable("PermissionOperations").InSchema("company")
                .Row(new { PermissionId = permId, OperationId = operations["read"] });
            Insert.IntoTable("PermissionOperations").InSchema("company")
                .Row(new { PermissionId = permId, OperationId = operations["invite"] });
        }

        private void InsertManagerFinancePermissions(Guid grantId, Dictionary<string, Guid> resources, Dictionary<string, Guid> operations)
        {
            // Кассовые чеки - read
            var receiptsPermId = Guid.NewGuid();
            Insert.IntoTable("Permissions").InSchema("company")
                .Row(new { Id = receiptsPermId, ResourceId = resources["finance.receipts"], ResourceValue = (string?)null, AccessGrantId = grantId });
            Insert.IntoTable("PermissionOperations").InSchema("company")
                .Row(new { PermissionId = receiptsPermId, OperationId = operations["read"] });

            // Финансовые отчеты - read, export
            var reportsPermId = Guid.NewGuid();
            Insert.IntoTable("Permissions").InSchema("company")
                .Row(new { Id = reportsPermId, ResourceId = resources["finance.reports"], ResourceValue = (string?)null, AccessGrantId = grantId });
            Insert.IntoTable("PermissionOperations").InSchema("company")
                .Row(new { PermissionId = reportsPermId, OperationId = operations["read"] });
            Insert.IntoTable("PermissionOperations").InSchema("company")
                .Row(new { PermissionId = reportsPermId, OperationId = operations["export"] });
        }

        private void InsertManagerDocumentsPermissions(Guid grantId, Dictionary<string, Guid> resources, Dictionary<string, Guid> operations)
        {
            var permId = Guid.NewGuid();
            Insert.IntoTable("Permissions").InSchema("company")
                .Row(new { Id = permId, ResourceId = resources["documents"], ResourceValue = (string?)null, AccessGrantId = grantId });

            Insert.IntoTable("PermissionOperations").InSchema("company")
                .Row(new { PermissionId = permId, OperationId = operations["create"] });
            Insert.IntoTable("PermissionOperations").InSchema("company")
                .Row(new { PermissionId = permId, OperationId = operations["read"] });
            Insert.IntoTable("PermissionOperations").InSchema("company")
                .Row(new { PermissionId = permId, OperationId = operations["update"] });
        }

        private void InsertEmployeeOrdersViewPermissions(Guid grantId, Dictionary<string, Guid> resources, Dictionary<string, Guid> operations)
        {
            var permId = Guid.NewGuid();
            Insert.IntoTable("Permissions").InSchema("company")
                .Row(new { Id = permId, ResourceId = resources["orders.all"], ResourceValue = (string?)null, AccessGrantId = grantId });

            Insert.IntoTable("PermissionOperations").InSchema("company")
                .Row(new { PermissionId = permId, OperationId = operations["read"] });
        }

        private void InsertEmployeeOrdersAssignedPermissions(Guid grantId, Dictionary<string, Guid> resources, Dictionary<string, Guid> operations)
        {
            var permId = Guid.NewGuid();
            Insert.IntoTable("Permissions").InSchema("company")
                .Row(new { Id = permId, ResourceId = resources["orders.assigned"], ResourceValue = (string?)null, AccessGrantId = grantId });

            Insert.IntoTable("PermissionOperations").InSchema("company")
                .Row(new { PermissionId = permId, OperationId = operations["read"] });
            Insert.IntoTable("PermissionOperations").InSchema("company")
                .Row(new { PermissionId = permId, OperationId = operations["update"] });
            Insert.IntoTable("PermissionOperations").InSchema("company")
                .Row(new { PermissionId = permId, OperationId = operations["execute"] });
        }

        private void InsertEmployeeReceiptsPermissions(Guid grantId, Dictionary<string, Guid> resources, Dictionary<string, Guid> operations)
        {
            var permId = Guid.NewGuid();
            Insert.IntoTable("Permissions").InSchema("company")
                .Row(new { Id = permId, ResourceId = resources["finance.receipts"], ResourceValue = (string?)null, AccessGrantId = grantId });

            Insert.IntoTable("PermissionOperations").InSchema("company")
                .Row(new { PermissionId = permId, OperationId = operations["create"] });
            Insert.IntoTable("PermissionOperations").InSchema("company")
                .Row(new { PermissionId = permId, OperationId = operations["read"] });
        }
    }
}
