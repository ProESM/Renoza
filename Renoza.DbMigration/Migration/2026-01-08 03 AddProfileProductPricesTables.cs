using System.Data;
using FluentMigrator;

namespace Renoza.DbMigration.Migration
{
    [Migration(2026010803, "Добавление таблиц цен профилей на товары/услуги")]
    public class AddProfileProductPricesTables : FluentMigrator.Migration
    {
        public override void Up()
        {
            #region ProfileProductPrices

            Create.Table("ProfileProductPrices")
                .InSchema("public")
                .WithColumn("ProfileId").AsGuid().NotNullable()
                    .ForeignKey("FK_ProfileProductPrices_ProfileId", "public", "Profiles", "Id").OnDelete(Rule.Cascade)
                .WithColumn("ProductId").AsGuid().NotNullable()
                    .ForeignKey("FK_ProfileProductPrices_ProductId", "public", "Products", "Id").OnDelete(Rule.Cascade)
                .WithColumn("StartDate").AsCustom("timestamp with time zone").NotNullable()
                .WithColumn("EndDate").AsCustom("timestamp with time zone").NotNullable()
                .WithColumn("CurrencyId").AsGuid().NotNullable()
                    .ForeignKey("FK_ProfileProductPrices_CurrencyId", "public", "Currencies", "Id").OnDelete(Rule.None)
                .WithColumn("Price").AsDecimal(18, 2).NotNullable();

            // Составной первичный ключ
            Create.PrimaryKey("PK_ProfileProductPrices")
                .OnTable("ProfileProductPrices")
                .WithSchema("public")
                .Columns("ProfileId", "ProductId", "StartDate");

            Create.Index("IDX_ProfileProductPrices_ProfileId")
                .OnTable("ProfileProductPrices")
                .InSchema("public")
                .OnColumn("ProfileId");

            Create.Index("IDX_ProfileProductPrices_ProductId")
                .OnTable("ProfileProductPrices")
                .InSchema("public")
                .OnColumn("ProductId");

            Create.Index("IDX_ProfileProductPrices_CurrencyId")
                .OnTable("ProfileProductPrices")
                .InSchema("public")
                .OnColumn("CurrencyId");

            #endregion

            #region ProfileProductOverridePrices

            Create.Table("ProfileProductOverridePrices")
                .InSchema("public")
                .WithColumn("ProfileId").AsGuid().NotNullable()
                .WithColumn("ProductId").AsGuid().NotNullable()
                .WithColumn("StartDate").AsCustom("timestamp with time zone").NotNullable()
                .WithColumn("CurrencyId").AsGuid().NotNullable()
                    .ForeignKey("FK_ProfileProductOverridePrices_CurrencyId", "public", "Currencies", "Id").OnDelete(Rule.None)
                .WithColumn("Price").AsDecimal(18, 2).NotNullable();

            // Составной первичный ключ
            Create.PrimaryKey("PK_ProfileProductOverridePrices")
                .OnTable("ProfileProductOverridePrices")
                .WithSchema("public")
                .Columns("ProfileId", "ProductId", "StartDate", "CurrencyId");

            // FK на базовую цену (составной ключ)
            Create.ForeignKey("FK_ProfileProductOverridePrices_BasePrice")
                .FromTable("ProfileProductOverridePrices").InSchema("public")
                .ForeignColumns("ProfileId", "ProductId", "StartDate")
                .ToTable("ProfileProductPrices").InSchema("public")
                .PrimaryColumns("ProfileId", "ProductId", "StartDate")
                .OnDelete(Rule.Cascade);

            Create.Index("IDX_ProfileProductOverridePrices_CurrencyId")
                .OnTable("ProfileProductOverridePrices")
                .InSchema("public")
                .OnColumn("CurrencyId");

            #endregion
        }

        public override void Down()
        {
            Delete.Table("ProfileProductOverridePrices").InSchema("public");
            Delete.Table("ProfileProductPrices").InSchema("public");
        }
    }
}
