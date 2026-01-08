using System.Data;
using FluentMigrator;
using Renoza.DbMigration.Attributes;
using Renoza.DbMigration.Enums;

namespace Renoza.DbMigration.Migration
{
    [Migration(2025120701, "Добавление таблиц кассовых чеков и запросов на загрузку")]
    public class AddCashReceiptTables : FluentMigrator.Migration
    {
        public override void Up()
        {
            #region Схема public

            // Таблица статусов запросов на загрузку чеков
            if (!Schema.Schema("public").Table("CashReceiptJobStatuses").Exists())
            {
                Create.Table("CashReceiptJobStatuses")
                    .InSchema("public")
                    .WithColumn("Id").AsInt16().PrimaryKey()
                    .WithColumn("Code").AsString(50).NotNullable().Unique()
                    .WithColumn("Name").AsString(256).NotNullable()
                    .WithColumn("DisplayName").AsString(256).Nullable()
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                // Добавляем начальные статусы из Enum
                var now = DateTime.UtcNow;
                var statuses = typeof(CashReceiptJobStatus).GetMembers()
                    .Select(x => x.GetCustomAttributes(typeof(CashReceiptJobStatusDetailsAttribute), false))
                    .SelectMany(x => x.Cast<CashReceiptJobStatusDetailsAttribute>());

                foreach (var status in statuses)
                {
                    Insert.IntoTable("CashReceiptJobStatuses")
                        .InSchema("public")
                        .Row(new
                        {
                            Id = status.Id,
                            Code = status.Code,
                            Name = status.Name,
                            DisplayName = status.DisplayName,
                            IsActive = status.IsActive,
                            CreatedAt = now
                        });
                }
            }

            // Таблица кассовых чеков
            if (!Schema.Schema("public").Table("CashReceipts").Exists())
            {
                Create.Table("CashReceipts")
                    .InSchema("public")
                    .WithColumn("Id").AsGuid().PrimaryKey()
                    .WithColumn("QrCode").AsString(256).NotNullable()
                    .WithColumn("NormalizedQrSource").AsString(256).NotNullable()
                    .WithColumn("JsonData").AsCustom("text").Nullable()
                    .WithColumn("FileUrl").AsString(500).Nullable()
                    .WithColumn("TotalAmount").AsDecimal().Nullable()
                    .WithColumn("DocumentDateTime").AsCustom("timestamp with time zone").Nullable()
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                // UNIQUE индекс для предотвращения дубликатов чеков
                Create.Index("UQ_CashReceipts_NormalizedQrSource")
                    .OnTable("CashReceipts")
                    .InSchema("public")
                    .OnColumn("NormalizedQrSource")
                    .Ascending()
                    .WithOptions()
                    .Unique();

                Create.Index("IDX_CashReceipts_QrCode")
                    .OnTable("CashReceipts")
                    .InSchema("public")
                    .OnColumn("QrCode");

                // Индекс для аналитики и отчётов по дате создания
                Create.Index("IDX_CashReceipts_CreatedAt")
                    .OnTable("CashReceipts")
                    .InSchema("public")
                    .OnColumn("CreatedAt")
                    .Descending();
            }

            // Таблица запросов на загрузку чеков
            if (!Schema.Schema("public").Table("CashReceiptJobs").Exists())
            {
                Create.Table("CashReceiptJobs")
                    .InSchema("public")
                    .WithColumn("Id").AsGuid().PrimaryKey()
                    .WithColumn("CustomerId").AsGuid().NotNullable()
                    .WithColumn("CreatedBy").AsGuid().NotNullable()
                    .WithColumn("OrderId").AsGuid().Nullable()
                    .WithColumn("StatusId").AsInt16().NotNullable().ForeignKey("FK_CashReceiptJobs_StatusId", "public", "CashReceiptJobStatuses", "Id").OnDelete(Rule.None)
                    .WithColumn("StatusComment").AsString(2000).Nullable()
                    .WithColumn("QrSource").AsString(256).NotNullable()
                    .WithColumn("IpAddress").AsString(50).NotNullable()
                    .WithColumn("CashReceiptId").AsGuid().Nullable().ForeignKey("FK_CashReceiptJobs_CashReceiptId", "public", "CashReceipts", "Id").OnDelete(Rule.Cascade)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("CompletedAt").AsCustom("timestamp with time zone").Nullable();

                Create.Index("IDX_CashReceiptJobs_CustomerId")
                    .OnTable("CashReceiptJobs")
                    .InSchema("public")
                    .OnColumn("CustomerId");

                Create.Index("IDX_CashReceiptJobs_CreatedBy")
                    .OnTable("CashReceiptJobs")
                    .InSchema("public")
                    .OnColumn("CreatedBy");

                Create.Index("IDX_CashReceiptJobs_StatusId")
                    .OnTable("CashReceiptJobs")
                    .InSchema("public")
                    .OnColumn("StatusId");

                Create.Index("IDX_CashReceiptJobs_OrderId")
                    .OnTable("CashReceiptJobs")
                    .InSchema("public")
                    .OnColumn("OrderId");

                Create.Index("IDX_CashReceiptJobs_CashReceiptId")
                    .OnTable("CashReceiptJobs")
                    .InSchema("public")
                    .OnColumn("CashReceiptId");

                // Составной индекс для получения последних чеков клиента (КРИТИЧНЫЙ)
                Create.Index("IDX_CashReceiptJobs_CustomerId_CreatedAt")
                    .OnTable("CashReceiptJobs")
                    .InSchema("public")
                    .OnColumn("CustomerId").Ascending()
                    .OnColumn("CreatedAt").Descending();
            }

            // Таблица истории изменений статусов запросов
            if (!Schema.Schema("public").Table("CashReceiptJobHistory").Exists())
            {
                Create.Table("CashReceiptJobHistory")
                    .InSchema("public")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("JobId").AsGuid().NotNullable().ForeignKey("FK_CashReceiptJobHistory_JobId", "public", "CashReceiptJobs", "Id").OnDelete(Rule.Cascade)
                    .WithColumn("StatusId").AsInt16().NotNullable().ForeignKey("FK_CashReceiptJobHistory_StatusId", "public", "CashReceiptJobStatuses", "Id").OnDelete(Rule.None)
                    .WithColumn("Comment").AsString(2000).Nullable()
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                Create.Index("IDX_CashReceiptJobHistory_JobId")
                    .OnTable("CashReceiptJobHistory")
                    .InSchema("public")
                    .OnColumn("JobId");

                Create.Index("IDX_CashReceiptJobHistory_StatusId")
                    .OnTable("CashReceiptJobHistory")
                    .InSchema("public")
                    .OnColumn("StatusId");

                // Составной индекс для получения истории Job отсортированной по дате (ВАЖНЫЙ)
                Create.Index("IDX_CashReceiptJobHistory_JobId_CreatedAt")
                    .OnTable("CashReceiptJobHistory")
                    .InSchema("public")
                    .OnColumn("JobId").Ascending()
                    .OnColumn("CreatedAt").Descending();
            }

            // Таблица связей заказчиков и кассовых чеков
            if (!Schema.Schema("public").Table("CustomerCashReceipts").Exists())
            {
                Create.Table("CustomerCashReceipts")
                    .InSchema("public")
                    .WithColumn("CustomerId").AsGuid().PrimaryKey().ForeignKey("FK_CustomerCashReceipts_CustomerId", "public", "CustomerProfiles", "Id")
                    .WithColumn("CashReceiptId").AsGuid().PrimaryKey().ForeignKey("FK_CustomerCashReceipts_CashReceiptId", "public", "CashReceipts", "Id")
                    .WithColumn("OrderId").AsGuid().Nullable()
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                Create.Index("IDX_CustomerCashReceipts_CustomerId")
                    .OnTable("CustomerCashReceipts")
                    .InSchema("public")
                    .OnColumn("CustomerId");

                Create.Index("IDX_CustomerCashReceipts_CashReceiptId")
                    .OnTable("CustomerCashReceipts")
                    .InSchema("public")
                    .OnColumn("CashReceiptId");

                Create.Index("IDX_CustomerCashReceipts_OrderId")
                    .OnTable("CustomerCashReceipts")
                    .InSchema("public")
                    .OnColumn("OrderId");
            }

            #endregion
        }

        public override void Down()
        {
            #region Схема public

            // Удаляем таблицу связей заказчиков и кассовых чеков
            if (Schema.Schema("public").Table("CustomerCashReceipts").Exists())
            {
                if (Schema.Schema("public").Table("CustomerCashReceipts").Index("IDX_CustomerCashReceipts_CustomerId").Exists())
                {
                    Delete.Index("IDX_CustomerCashReceipts_CustomerId").OnTable("CustomerCashReceipts").InSchema("public");
                }
                if (Schema.Schema("public").Table("CustomerCashReceipts").Index("IDX_CustomerCashReceipts_CashReceiptId").Exists())
                {
                    Delete.Index("IDX_CustomerCashReceipts_CashReceiptId").OnTable("CustomerCashReceipts").InSchema("public");
                }
                if (Schema.Schema("public").Table("CustomerCashReceipts").Index("IDX_CustomerCashReceipts_OrderId").Exists())
                {
                    Delete.Index("IDX_CustomerCashReceipts_OrderId").OnTable("CustomerCashReceipts").InSchema("public");
                }
                Delete.Table("CustomerCashReceipts").InSchema("public");
            }

            // Удаляем таблицу истории изменений статусов
            if (Schema.Schema("public").Table("CashReceiptJobHistory").Exists())
            {
                if (Schema.Schema("public").Table("CashReceiptJobHistory").Index("IDX_CashReceiptJobHistory_JobId_CreatedAt").Exists())
                {
                    Delete.Index("IDX_CashReceiptJobHistory_JobId_CreatedAt").OnTable("CashReceiptJobHistory").InSchema("public");
                }
                if (Schema.Schema("public").Table("CashReceiptJobHistory").Index("IDX_CashReceiptJobHistory_JobId").Exists())
                {
                    Delete.Index("IDX_CashReceiptJobHistory_JobId").OnTable("CashReceiptJobHistory").InSchema("public");
                }
                if (Schema.Schema("public").Table("CashReceiptJobHistory").Index("IDX_CashReceiptJobHistory_StatusId").Exists())
                {
                    Delete.Index("IDX_CashReceiptJobHistory_StatusId").OnTable("CashReceiptJobHistory").InSchema("public");
                }
                Delete.Table("CashReceiptJobHistory").InSchema("public");
            }

            // Удаляем таблицу запросов на загрузку чеков
            if (Schema.Schema("public").Table("CashReceiptJobs").Exists())
            {
                if (Schema.Schema("public").Table("CashReceiptJobs").Index("IDX_CashReceiptJobs_CustomerId_CreatedAt").Exists())
                {
                    Delete.Index("IDX_CashReceiptJobs_CustomerId_CreatedAt").OnTable("CashReceiptJobs").InSchema("public");
                }
                if (Schema.Schema("public").Table("CashReceiptJobs").Index("IDX_CashReceiptJobs_CashReceiptId").Exists())
                {
                    Delete.Index("IDX_CashReceiptJobs_CashReceiptId").OnTable("CashReceiptJobs").InSchema("public");
                }
                if (Schema.Schema("public").Table("CashReceiptJobs").Index("IDX_CashReceiptJobs_CustomerId").Exists())
                {
                    Delete.Index("IDX_CashReceiptJobs_CustomerId").OnTable("CashReceiptJobs").InSchema("public");
                }
                if (Schema.Schema("public").Table("CashReceiptJobs").Index("IDX_CashReceiptJobs_CreatedBy").Exists())
                {
                    Delete.Index("IDX_CashReceiptJobs_CreatedBy").OnTable("CashReceiptJobs").InSchema("public");
                }
                if (Schema.Schema("public").Table("CashReceiptJobs").Index("IDX_CashReceiptJobs_StatusId").Exists())
                {
                    Delete.Index("IDX_CashReceiptJobs_StatusId").OnTable("CashReceiptJobs").InSchema("public");
                }
                if (Schema.Schema("public").Table("CashReceiptJobs").Index("IDX_CashReceiptJobs_OrderId").Exists())
                {
                    Delete.Index("IDX_CashReceiptJobs_OrderId").OnTable("CashReceiptJobs").InSchema("public");
                }
                Delete.Table("CashReceiptJobs").InSchema("public");
            }

            // Удаляем таблицу кассовых чеков
            if (Schema.Schema("public").Table("CashReceipts").Exists())
            {
                if (Schema.Schema("public").Table("CashReceipts").Index("IDX_CashReceipts_CreatedAt").Exists())
                {
                    Delete.Index("IDX_CashReceipts_CreatedAt").OnTable("CashReceipts").InSchema("public");
                }
                if (Schema.Schema("public").Table("CashReceipts").Index("UQ_CashReceipts_NormalizedQrSource").Exists())
                {
                    Delete.Index("UQ_CashReceipts_NormalizedQrSource").OnTable("CashReceipts").InSchema("public");
                }
                if (Schema.Schema("public").Table("CashReceipts").Index("IDX_CashReceipts_QrCode").Exists())
                {
                    Delete.Index("IDX_CashReceipts_QrCode").OnTable("CashReceipts").InSchema("public");
                }
                Delete.Table("CashReceipts").InSchema("public");
            }

            // Удаляем таблицу статусов запросов
            if (Schema.Schema("public").Table("CashReceiptJobStatuses").Exists())
            {
                Delete.Table("CashReceiptJobStatuses").InSchema("public");
            }

            #endregion
        }
    }
}
