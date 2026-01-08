using System.Data;
using FluentMigrator;

namespace Renoza.DbMigration.Migration
{
    [Migration(2026010804, "Добавление таблицы корзины покупок")]
    public class AddCartItemsTable : FluentMigrator.Migration
    {
        public override void Up()
        {
            Create.Table("CartItems")
                .InSchema("public")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("UserId").AsGuid().NotNullable()
                    .ForeignKey("FK_CartItems_UserId", "auth", "Users", "Id").OnDelete(Rule.Cascade)
                .WithColumn("ProfileId").AsGuid().NotNullable()
                    .ForeignKey("FK_CartItems_ProfileId", "public", "Profiles", "Id").OnDelete(Rule.Cascade)
                .WithColumn("ProductId").AsGuid().NotNullable()
                    .ForeignKey("FK_CartItems_ProductId", "public", "Products", "Id").OnDelete(Rule.Cascade)
                .WithColumn("Quantity").AsDecimal(18, 3).NotNullable().WithDefaultValue(1)
                .WithColumn("Notes").AsString(1000).Nullable()
                .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            Create.Index("IDX_CartItems_UserId")
                .OnTable("CartItems")
                .InSchema("public")
                .OnColumn("UserId");

            Create.Index("IDX_CartItems_ProfileId")
                .OnTable("CartItems")
                .InSchema("public")
                .OnColumn("ProfileId");

            Create.Index("IDX_CartItems_ProductId")
                .OnTable("CartItems")
                .InSchema("public")
                .OnColumn("ProductId");

            // Уникальный индекс: один пользователь не может добавить один и тот же товар от одного исполнителя дважды
            Create.Index("UX_CartItems_UserId_ProfileId_ProductId")
                .OnTable("CartItems")
                .InSchema("public")
                .OnColumn("UserId").Ascending()
                .OnColumn("ProfileId").Ascending()
                .OnColumn("ProductId").Ascending()
                .WithOptions().Unique();
        }

        public override void Down()
        {
            Delete.Table("CartItems").InSchema("public");
        }
    }
}
