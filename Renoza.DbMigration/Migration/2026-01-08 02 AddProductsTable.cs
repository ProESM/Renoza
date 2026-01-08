using System.Data;
using FluentMigrator;

namespace Renoza.DbMigration.Migration
{
    [Migration(2026010802, "Добавление таблицы товаров и услуг")]
    public class AddProductsTable : FluentMigrator.Migration
    {
        public override void Up()
        {
            Create.Table("Products")
                .InSchema("public")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("CategoryId").AsGuid().NotNullable()
                    .ForeignKey("FK_Products_CategoryId", "public", "ProductCategories", "Id").OnDelete(Rule.None)
                .WithColumn("Name").AsString(256).NotNullable()
                .WithColumn("Description").AsString(2000).Nullable()
                .WithColumn("MeasurementUnitId").AsGuid().NotNullable()
                    .ForeignKey("FK_Products_MeasurementUnitId", "public", "MeasurementUnits", "Id").OnDelete(Rule.None)
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            Create.Index("IDX_Products_CategoryId")
                .OnTable("Products")
                .InSchema("public")
                .OnColumn("CategoryId");

            Create.Index("IDX_Products_MeasurementUnitId")
                .OnTable("Products")
                .InSchema("public")
                .OnColumn("MeasurementUnitId");

            Create.Index("IDX_Products_IsActive")
                .OnTable("Products")
                .InSchema("public")
                .OnColumn("IsActive");
        }

        public override void Down()
        {
            Delete.Table("Products").InSchema("public");
        }
    }
}
