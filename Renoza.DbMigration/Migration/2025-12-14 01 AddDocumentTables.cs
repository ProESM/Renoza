using FluentMigrator;
using Renoza.DbMigration.Attributes;
using Renoza.DbMigration.Enums;

namespace Renoza.DbMigration.Migration
{
    [Migration(2025121401, "Добавление таблиц для документов и шаблонов")]
    public class AddDocumentTables : FluentMigrator.Migration
    {
        public override void Up()
        {
            #region Схема public

            // Таблица типов шаблонов документов (справочник)
            if (!Schema.Schema("public").Table("DocumentTemplateTypes").Exists())
            {
                Create.Table("DocumentTemplateTypes")
                    .InSchema("public")
                    .WithColumn("Id").AsInt16().PrimaryKey()
                    .WithColumn("Code").AsString(50).NotNullable().Unique()
                    .WithColumn("Name").AsString(256).NotNullable()
                    .WithColumn("DisplayName").AsString(256).Nullable()
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                // Добавляем начальные типы шаблонов из Enum
                var now = DateTime.UtcNow;
                var templateTypes = typeof(DocumentTemplateType).GetMembers()
                    .Select(x => x.GetCustomAttributes(typeof(DocumentTemplateTypeDetailsAttribute), false))
                    .SelectMany(x => x.Cast<DocumentTemplateTypeDetailsAttribute>());

                foreach (var templateType in templateTypes)
                {
                    Insert.IntoTable("DocumentTemplateTypes")
                        .InSchema("public")
                        .Row(new
                        {
                            Id = templateType.Id,
                            Code = templateType.Code,
                            Name = templateType.Name,
                            DisplayName = templateType.DisplayName,
                            IsActive = templateType.IsActive,
                            CreatedAt = now
                        });
                }
            }

            // Таблица форматов документов (справочник)
            if (!Schema.Schema("public").Table("DocumentFormats").Exists())
            {
                Create.Table("DocumentFormats")
                    .InSchema("public")
                    .WithColumn("Id").AsInt16().PrimaryKey()
                    .WithColumn("Code").AsString(50).NotNullable().Unique()
                    .WithColumn("Name").AsString(256).NotNullable()
                    .WithColumn("DisplayName").AsString(256).Nullable()
                    .WithColumn("FileExtension").AsString(10).NotNullable()
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                // Добавляем форматы из Enum
                var now = DateTime.UtcNow;
                var formats = typeof(DocumentFormat).GetMembers()
                    .Select(x => x.GetCustomAttributes(typeof(DocumentFormatDetailsAttribute), false))
                    .SelectMany(x => x.Cast<DocumentFormatDetailsAttribute>());

                foreach (var format in formats)
                {
                    Insert.IntoTable("DocumentFormats")
                        .InSchema("public")
                        .Row(new
                        {
                            Id = format.Id,
                            Code = format.Code,
                            Name = format.Name,
                            DisplayName = format.DisplayName,
                            FileExtension = format.FileExtension,
                            IsActive = format.IsActive,
                            CreatedAt = now
                        });
                }
            }

            // Таблица статусов документов (справочник)
            if (!Schema.Schema("public").Table("DocumentStatuses").Exists())
            {
                Create.Table("DocumentStatuses")
                    .InSchema("public")
                    .WithColumn("Id").AsInt16().PrimaryKey()
                    .WithColumn("Code").AsString(50).NotNullable().Unique()
                    .WithColumn("Name").AsString(256).NotNullable()
                    .WithColumn("DisplayName").AsString(256).Nullable()
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                // Добавляем статусы из Enum
                var now = DateTime.UtcNow;
                var statuses = typeof(DocumentStatus).GetMembers()
                    .Select(x => x.GetCustomAttributes(typeof(DocumentStatusDetailsAttribute), false))
                    .SelectMany(x => x.Cast<DocumentStatusDetailsAttribute>());

                foreach (var status in statuses)
                {
                    Insert.IntoTable("DocumentStatuses")
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

            // Таблица шаблонов документов
            if (!Schema.Schema("public").Table("DocumentTemplates").Exists())
            {
                Create.Table("DocumentTemplates")
                    .InSchema("public")
                    .WithColumn("Id").AsGuid().PrimaryKey()
                    .WithColumn("TemplateTypeId").AsInt16().NotNullable()
                        .ForeignKey("FK_DocumentTemplates_TemplateTypeId", "public", "DocumentTemplateTypes", "Id")
                    .WithColumn("Name").AsString(500).NotNullable()
                    .WithColumn("Description").AsString(2000).Nullable()
                    .WithColumn("FileUrl").AsString(500).NotNullable()
                    .WithColumn("FileSize").AsInt64().NotNullable()
                    .WithColumn("AvailablePlaceholders").AsCustom("text").Nullable()
                    .WithColumn("CreatedBy").AsGuid().NotNullable()
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                Create.Index("IDX_DocumentTemplates_TemplateTypeId")
                    .OnTable("DocumentTemplates")
                    .InSchema("public")
                    .OnColumn("TemplateTypeId");

                Create.Index("IDX_DocumentTemplates_CreatedBy")
                    .OnTable("DocumentTemplates")
                    .InSchema("public")
                    .OnColumn("CreatedBy");

                Create.Index("IDX_DocumentTemplates_IsActive")
                    .OnTable("DocumentTemplates")
                    .InSchema("public")
                    .OnColumn("IsActive");

                Create.Index("IDX_DocumentTemplates_CreatedAt")
                    .OnTable("DocumentTemplates")
                    .InSchema("public")
                    .OnColumn("CreatedAt")
                    .Descending();
            }

            // Таблица сгенерированных документов
            if (!Schema.Schema("public").Table("Documents").Exists())
            {
                Create.Table("Documents")
                    .InSchema("public")
                    .WithColumn("Id").AsGuid().PrimaryKey()
                    .WithColumn("TemplateId").AsGuid().NotNullable()
                        .ForeignKey("FK_Documents_TemplateId", "public", "DocumentTemplates", "Id")
                    .WithColumn("FormatId").AsInt16().NotNullable()
                        .ForeignKey("FK_Documents_FormatId", "public", "DocumentFormats", "Id")
                    .WithColumn("StatusId").AsInt16().NotNullable()
                        .ForeignKey("FK_Documents_StatusId", "public", "DocumentStatuses", "Id")
                    .WithColumn("Name").AsString(500).NotNullable()
                    .WithColumn("FileUrl").AsString(500).NotNullable()
                    .WithColumn("FileSize").AsInt64().NotNullable()
                    .WithColumn("PlaceholderData").AsCustom("text").Nullable()
                    .WithColumn("OrderId").AsGuid().Nullable()
                    .WithColumn("CustomerId").AsGuid().Nullable()
                    .WithColumn("CreatedBy").AsGuid().NotNullable()
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("FirstDownloadedAt").AsCustom("timestamp with time zone").Nullable()
                    .WithColumn("LastDownloadedAt").AsCustom("timestamp with time zone").Nullable()
                    .WithColumn("DownloadCount").AsInt32().NotNullable().WithDefaultValue(0);

                Create.Index("IDX_Documents_TemplateId")
                    .OnTable("Documents")
                    .InSchema("public")
                    .OnColumn("TemplateId");

                Create.Index("IDX_Documents_FormatId")
                    .OnTable("Documents")
                    .InSchema("public")
                    .OnColumn("FormatId");

                Create.Index("IDX_Documents_StatusId")
                    .OnTable("Documents")
                    .InSchema("public")
                    .OnColumn("StatusId");

                Create.Index("IDX_Documents_OrderId")
                    .OnTable("Documents")
                    .InSchema("public")
                    .OnColumn("OrderId");

                Create.Index("IDX_Documents_CustomerId")
                    .OnTable("Documents")
                    .InSchema("public")
                    .OnColumn("CustomerId");

                Create.Index("IDX_Documents_CreatedBy")
                    .OnTable("Documents")
                    .InSchema("public")
                    .OnColumn("CreatedBy");

                Create.Index("IDX_Documents_CreatedAt")
                    .OnTable("Documents")
                    .InSchema("public")
                    .OnColumn("CreatedAt")
                    .Descending();

                // Составной индекс для получения документов заказчика
                Create.Index("IDX_Documents_CustomerId_CreatedAt")
                    .OnTable("Documents")
                    .InSchema("public")
                    .OnColumn("CustomerId").Ascending()
                    .OnColumn("CreatedAt").Descending();

                // Составной индекс для получения документов заказа
                Create.Index("IDX_Documents_OrderId_CreatedAt")
                    .OnTable("Documents")
                    .InSchema("public")
                    .OnColumn("OrderId").Ascending()
                    .OnColumn("CreatedAt").Descending();
            }

            #endregion
        }

        public override void Down()
        {
            #region Схема public

            // Удаляем таблицу документов
            if (Schema.Schema("public").Table("Documents").Exists())
            {
                if (Schema.Schema("public").Table("Documents").Index("IDX_Documents_OrderId_CreatedAt").Exists())
                    Delete.Index("IDX_Documents_OrderId_CreatedAt").OnTable("Documents").InSchema("public");
                if (Schema.Schema("public").Table("Documents").Index("IDX_Documents_CustomerId_CreatedAt").Exists())
                    Delete.Index("IDX_Documents_CustomerId_CreatedAt").OnTable("Documents").InSchema("public");
                if (Schema.Schema("public").Table("Documents").Index("IDX_Documents_CreatedAt").Exists())
                    Delete.Index("IDX_Documents_CreatedAt").OnTable("Documents").InSchema("public");
                if (Schema.Schema("public").Table("Documents").Index("IDX_Documents_CreatedBy").Exists())
                    Delete.Index("IDX_Documents_CreatedBy").OnTable("Documents").InSchema("public");
                if (Schema.Schema("public").Table("Documents").Index("IDX_Documents_CustomerId").Exists())
                    Delete.Index("IDX_Documents_CustomerId").OnTable("Documents").InSchema("public");
                if (Schema.Schema("public").Table("Documents").Index("IDX_Documents_OrderId").Exists())
                    Delete.Index("IDX_Documents_OrderId").OnTable("Documents").InSchema("public");
                if (Schema.Schema("public").Table("Documents").Index("IDX_Documents_StatusId").Exists())
                    Delete.Index("IDX_Documents_StatusId").OnTable("Documents").InSchema("public");
                if (Schema.Schema("public").Table("Documents").Index("IDX_Documents_FormatId").Exists())
                    Delete.Index("IDX_Documents_FormatId").OnTable("Documents").InSchema("public");
                if (Schema.Schema("public").Table("Documents").Index("IDX_Documents_TemplateId").Exists())
                    Delete.Index("IDX_Documents_TemplateId").OnTable("Documents").InSchema("public");

                Delete.Table("Documents").InSchema("public");
            }

            // Удаляем таблицу шаблонов
            if (Schema.Schema("public").Table("DocumentTemplates").Exists())
            {
                if (Schema.Schema("public").Table("DocumentTemplates").Index("IDX_DocumentTemplates_CreatedAt").Exists())
                    Delete.Index("IDX_DocumentTemplates_CreatedAt").OnTable("DocumentTemplates").InSchema("public");
                if (Schema.Schema("public").Table("DocumentTemplates").Index("IDX_DocumentTemplates_IsActive").Exists())
                    Delete.Index("IDX_DocumentTemplates_IsActive").OnTable("DocumentTemplates").InSchema("public");
                if (Schema.Schema("public").Table("DocumentTemplates").Index("IDX_DocumentTemplates_CreatedBy").Exists())
                    Delete.Index("IDX_DocumentTemplates_CreatedBy").OnTable("DocumentTemplates").InSchema("public");
                if (Schema.Schema("public").Table("DocumentTemplates").Index("IDX_DocumentTemplates_TemplateTypeId").Exists())
                    Delete.Index("IDX_DocumentTemplates_TemplateTypeId").OnTable("DocumentTemplates").InSchema("public");

                Delete.Table("DocumentTemplates").InSchema("public");
            }

            // Удаляем справочники
            if (Schema.Schema("public").Table("DocumentStatuses").Exists())
                Delete.Table("DocumentStatuses").InSchema("public");
            if (Schema.Schema("public").Table("DocumentFormats").Exists())
                Delete.Table("DocumentFormats").InSchema("public");
            if (Schema.Schema("public").Table("DocumentTemplateTypes").Exists())
                Delete.Table("DocumentTemplateTypes").InSchema("public");

            #endregion
        }
    }
}
