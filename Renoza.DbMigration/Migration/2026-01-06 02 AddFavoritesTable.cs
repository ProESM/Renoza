using FluentMigrator;
using System.Data;

namespace Renoza.DbMigration.Migration
{
    [Migration(2026010602, "Добавление таблицы избранного (Favorites)")]
    public class AddFavoritesTable : FluentMigrator.Migration
    {
        public override void Up()
        {
            Create.Table("Favorites")
                .InSchema("public")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("UserId").AsGuid().NotNullable()
                    .ForeignKey("FK_Favorites_UserId", "auth", "Users", "Id").OnDelete(Rule.Cascade)
                .WithColumn("ProfileId").AsGuid().NotNullable()
                    .ForeignKey("FK_Favorites_ProfileId", "public", "Profiles", "Id").OnDelete(Rule.Cascade)
                .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            // Уникальный индекс - один пользователь не может добавить один и тот же профиль в избранное дважды
            Create.Index("UX_Favorites_UserId_ProfileId")
                .OnTable("Favorites")
                .InSchema("public")
                .OnColumn("UserId").Ascending()
                .OnColumn("ProfileId").Ascending()
                .WithOptions().Unique();

            // Индекс для быстрого поиска избранного пользователя
            Create.Index("IX_Favorites_UserId")
                .OnTable("Favorites")
                .InSchema("public")
                .OnColumn("UserId").Ascending();
        }

        public override void Down()
        {
            Delete.Index("IX_Favorites_UserId").OnTable("Favorites").InSchema("public");
            Delete.Index("UX_Favorites_UserId_ProfileId").OnTable("Favorites").InSchema("public");
            Delete.Table("Favorites").InSchema("public");
        }
    }
}
