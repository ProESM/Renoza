using FluentMigrator;
using Renoza.DbMigration.Attributes;
using Renoza.DbMigration.Attributes.Company;
using Renoza.DbMigration.Enums;
using Renoza.DbMigration.Enums.Company;
using System.Data;

namespace Renoza.DbMigration.Migration
{
    [Migration(2026010801, "Добавление справочных таблиц каталога товаров и услуг")]
    public class AddProductCatalogReferenceTables : FluentMigrator.Migration
    {
        public override void Up()
        {
            #region ProductCategories

            Create.Table("ProductCategories")
                .InSchema("public")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Name").AsString(256).NotNullable()
                .WithColumn("Description").AsString(1000).Nullable()
                .WithColumn("ParentId").AsGuid().Nullable()
                    .ForeignKey("FK_ProductCategories_ParentId", "public", "ProductCategories", "Id").OnDelete(Rule.None)
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("Order").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            Create.Index("IDX_ProductCategories_ParentId")
                .OnTable("ProductCategories")
                .InSchema("public")
                .OnColumn("ParentId");

            Create.Index("IDX_ProductCategories_IsActive")
                .OnTable("ProductCategories")
                .InSchema("public")
                .OnColumn("IsActive");

            #endregion

            #region MeasurementUnitTypes

            Create.Table("MeasurementUnitTypes")
                .InSchema("public")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Code").AsString(50).NotNullable().Unique("UX_MeasurementUnitTypes_Code")
                .WithColumn("Name").AsString(256).NotNullable()
                .WithColumn("Description").AsString(500).Nullable()
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            Create.Index("IDX_MeasurementUnitTypes_IsActive")
                .OnTable("MeasurementUnitTypes")
                .InSchema("public")
                .OnColumn("IsActive");

            // Предзаполнение типов единиц измерения из enum
            var measurementUnitTypes = typeof(MeasurementUnitType).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(MeasurementUnitTypeDetailsAttribute), false))
                .SelectMany(x => x.Cast<MeasurementUnitTypeDetailsAttribute>());

            foreach (var measurementUnitType in measurementUnitTypes)
            {
                Insert.IntoTable("MeasurementUnitTypes").InSchema("public")
                    .Row(new
                    {
                        Id = measurementUnitType.Id,
                        Code = measurementUnitType.Code,
                        Name = measurementUnitType.Name,
                        Description = measurementUnitType.Description,
                        IsActive = measurementUnitType.IsActive
                    });
            }

            #endregion

            #region MeasurementUnits

            Create.Table("MeasurementUnits")
                .InSchema("public")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("TypeId").AsGuid().NotNullable().ForeignKey("FK_MeasurementUnits_TypeId", "public", "MeasurementUnitTypes", "Id").OnDelete(Rule.None)
                .WithColumn("Code").AsString(50).NotNullable().Unique("UX_MeasurementUnits_Code")
                .WithColumn("Name").AsString(256).NotNullable()
                .WithColumn("Description").AsString(500).Nullable()
                .WithColumn("Multiplier").AsDecimal(18, 8).NotNullable()
                .WithColumn("IsBase").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            Create.Index("IDX_MeasurementUnits_TypeId")
                .OnTable("MeasurementUnits")
                .InSchema("public")
                .OnColumn("TypeId");

            Create.Index("IDX_MeasurementUnits_IsActive")
                .OnTable("MeasurementUnits")
                .InSchema("public")
                .OnColumn("IsActive");

            // Предзаполнение единиц измерения из enum
            var measurementUnits = typeof(MeasurementUnit).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(MeasurementUnitDetailsAttribute), false))
                .SelectMany(x => x.Cast<MeasurementUnitDetailsAttribute>());

            foreach (var measurementUnit in measurementUnits)
            {
                Insert.IntoTable("MeasurementUnits").InSchema("public")
                    .Row(new
                    {
                        Id = measurementUnit.Id,
                        TypeId = measurementUnit.TypeId,
                        Code = measurementUnit.Code,
                        Name = measurementUnit.Name,
                        Description = measurementUnit.Description,
                        Multiplier = measurementUnit.Multiplier,
                        IsBase = measurementUnit.IsBase,
                        IsActive = measurementUnit.IsActive
                    });
            }

            #endregion

            #region Currencies

            Create.Table("Currencies")
                .InSchema("public")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Code").AsString(50).NotNullable().Unique("UX_Currencies_Code")
                .WithColumn("Name").AsString(256).NotNullable()
                .WithColumn("Symbol").AsString(10).Nullable()
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            Create.Index("IDX_Currencies_IsActive")
                .OnTable("Currencies")
                .InSchema("public")
                .OnColumn("IsActive");

            // Предзаполнение валют из enum
            var currencies = typeof(Currency).GetMembers()
                .Select(x => x.GetCustomAttributes(typeof(CurrencyDetailsAttribute), false))
                .SelectMany(x => x.Cast<CurrencyDetailsAttribute>());

            foreach (var currency in currencies)
            {
                Insert.IntoTable("Currencies").InSchema("public")
                    .Row(new
                    {
                        Id = currency.Id,
                        Code = currency.Code,
                        Name = currency.Name,
                        Symbol = currency.Symbol,
                        IsActive = currency.IsActive
                    });
            }

            #endregion
        }

        public override void Down()
        {
            Delete.Table("Currencies").InSchema("public");
            Delete.Table("MeasurementUnits").InSchema("public");
            Delete.Table("MeasurementUnitTypes").InSchema("public");
            Delete.Table("ProductCategories").InSchema("public");
        }
    }
}
