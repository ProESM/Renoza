using FluentMigrator;

namespace Renoza.DbMigration.Migration
{
    [Migration(2025110901, "Инициализация базы данных")]
    public class InitDb : FluentMigrator.Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("public").Exists())
            {
                Create.Schema("public");
            }

            #region Таблицы и представления

            #region Схема auth

            #region Таблицы

            if (!Schema.Schema("auth").Exists())
            {
                Create.Schema("auth");
            }

            if (!Schema.Schema("auth").Table("User").Exists())
            {
                Create.Table("User")
                    .InSchema("auth")
                    .WithColumn("Id").AsInt16().PrimaryKey().Identity()
                    .WithColumn("Name").AsString(256).NotNullable();
            }

            #endregion

            #region Представления



            #endregion

            #endregion

            #region Схема public

            #region Таблицы

            if (!Schema.Schema("public").Table("AttributeType").Exists())
            {
                Create.Table("AttributeType")
                    .InSchema("public")
                    .WithColumn("Id").AsInt16().PrimaryKey().Identity()
                    .WithColumn("ExtId").AsString(128).NotNullable().Unique()
                    .WithColumn("Name").AsString(256).NotNullable()
                    .WithColumn("Code").AsString(128).NotNullable().Unique()
                    .WithColumn("Description").AsString().Nullable()
                    .WithColumn("EditedBy").AsString(50).Nullable()
                    .WithColumn("EditedDate").AsDateTime().Nullable()
                    .WithColumn("DeletedBy").AsString(50).Nullable()
                    .WithColumn("DeletedDate").AsDateTime().Nullable()
                    .WithColumn("IsDeleted").AsBoolean().NotNullable().WithDefaultValue(false);
            }

            #endregion

            #region Представления

            

            #endregion

            #endregion

            #endregion
        }

        public override void Down()
        {
            #region Схема public

            if (Schema.Schema("public").Table("AttributeType").Exists())
            {
                Delete.Table("AttributeType").InSchema("public");
            }

            #endregion

            #region Схема auth

            if (Schema.Schema("auth").Table("User").Exists())
            {
                Delete.Table("User").InSchema("auth");
            }

            if (Schema.Schema("auth").Exists())
            {
                Delete.Schema("auth");
            }

            #endregion
        }
    }
}
