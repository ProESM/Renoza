using System.Data;
using FluentMigrator;
using Renoza.DbMigration.Attributes.Company;
using Renoza.DbMigration.Enums.Company;

namespace Renoza.DbMigration.Migration
{
    [Migration(2026010601, "Добавление таблиц для запросов на вступление в компанию")]
    public class AddCompanyJoinRequestsTables : FluentMigrator.Migration
    {
        public override void Up()
        {
            #region Таблица статусов запросов на вступление

            Create.Table("CompanyJoinRequestStatuses")
                .InSchema("public")
                .WithColumn("Id").AsInt16().PrimaryKey()
                .WithColumn("Code").AsString(50).NotNullable().Unique()
                .WithColumn("Name").AsString(256).NotNullable()
                .WithColumn("DisplayName").AsString(256).Nullable()
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            // Предзаполнение статусов из enum
            var statuses = typeof(CompanyJoinRequestStatus).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(CompanyJoinRequestStatusDetailsAttribute), false))
                .SelectMany(x => x.Cast<CompanyJoinRequestStatusDetailsAttribute>());

            foreach (var status in statuses)
            {
                Insert.IntoTable("CompanyJoinRequestStatuses").InSchema("public")
                    .Row(new
                    {
                        Id = status.Id,
                        Code = status.Code,
                        Name = status.Name,
                        DisplayName = status.DisplayName,
                        IsActive = status.IsActive
                    });
            }

            #endregion

            #region Таблица запросов на вступление в компанию

            Create.Table("CompanyJoinRequests")
                .InSchema("public")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("UserId").AsGuid().NotNullable()
                    .ForeignKey("FK_CompanyJoinRequests_UserId", "auth", "Users", "Id").OnDelete(Rule.Cascade)
                .WithColumn("CompanyProfileId").AsGuid().NotNullable()
                    .ForeignKey("FK_CompanyJoinRequests_CompanyProfileId", "public", "CompanyProfiles", "Id").OnDelete(Rule.Cascade)
                .WithColumn("StatusId").AsInt16().NotNullable()
                    .ForeignKey("FK_CompanyJoinRequests_StatusId", "public", "CompanyJoinRequestStatuses", "Id").OnDelete(Rule.None)
                .WithColumn("RequestMessage").AsString(1000).Nullable()
                .WithColumn("ResponseMessage").AsString(1000).Nullable()
                .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("ReviewerId").AsGuid().Nullable()
                    .ForeignKey("FK_CompanyJoinRequests_ReviewerId", "auth", "Users", "Id").OnDelete(Rule.SetNull)
                .WithColumn("ReviewedAt").AsCustom("timestamp with time zone").Nullable();

            // Индексы для оптимизации поиска
            Create.Index("IDX_CompanyJoinRequests_UserId")
                .OnTable("CompanyJoinRequests")
                .InSchema("public")
                .OnColumn("UserId");

            Create.Index("IDX_CompanyJoinRequests_CompanyProfileId")
                .OnTable("CompanyJoinRequests")
                .InSchema("public")
                .OnColumn("CompanyProfileId");

            Create.Index("IDX_CompanyJoinRequests_StatusId")
                .OnTable("CompanyJoinRequests")
                .InSchema("public")
                .OnColumn("StatusId");

            Create.Index("IDX_CompanyJoinRequests_ReviewerId")
                .OnTable("CompanyJoinRequests")
                .InSchema("public")
                .OnColumn("ReviewerId");

            // Уникальный индекс: один пользователь может иметь только один активный (pending) запрос на вступление в компанию
            // Примечание: фильтр для StatusId=1 (Pending) будет добавлен вручную через Execute.Sql после миграции
            Create.Index("IDX_CompanyJoinRequests_UserId_CompanyProfileId_Pending")
                .OnTable("CompanyJoinRequests")
                .InSchema("public")
                .OnColumn("UserId").Ascending()
                .OnColumn("CompanyProfileId").Ascending()
                .OnColumn("StatusId").Ascending()
                .WithOptions().Unique();

            #endregion
        }

        public override void Down()
        {
            // Удаляем индексы
            Delete.Index("IDX_CompanyJoinRequests_UserId_CompanyProfileId_Pending")
                .OnTable("CompanyJoinRequests")
                .InSchema("public");

            Delete.Index("IDX_CompanyJoinRequests_ReviewerId")
                .OnTable("CompanyJoinRequests")
                .InSchema("public");

            Delete.Index("IDX_CompanyJoinRequests_StatusId")
                .OnTable("CompanyJoinRequests")
                .InSchema("public");

            Delete.Index("IDX_CompanyJoinRequests_CompanyProfileId")
                .OnTable("CompanyJoinRequests")
                .InSchema("public");

            Delete.Index("IDX_CompanyJoinRequests_UserId")
                .OnTable("CompanyJoinRequests")
                .InSchema("public");

            // Удаляем таблицу запросов
            Delete.Table("CompanyJoinRequests")
                .InSchema("public");

            // Удаляем индексы статусов
            Delete.Index("IDX_CompanyJoinRequestStatuses_Name")
                .OnTable("CompanyJoinRequestStatuses")
                .InSchema("public");

            // Удаляем таблицу статусов
            Delete.Table("CompanyJoinRequestStatuses")
                .InSchema("public");
        }
    }
}
