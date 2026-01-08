using FluentMigrator;
using Renoza.DbMigration.Attributes;
using System.Data;
using Renoza.DbMigration.Enums;

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

            #region Схема public

            #region Таблицы

            if (!Schema.Schema("public").Table("Countries").Exists())
            {
                Create.Table("Countries")
                    .InSchema("public")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                    .WithColumn("Name").AsString(256).NotNullable()
                    .WithColumn("Code").AsString(3).NotNullable().Unique()
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);
            }
            if (!Schema.Schema("public").Table("PhoneCountryCodes").Exists())
            {
                Create.Table("PhoneCountryCodes")
                    .InSchema("public")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                    .WithColumn("Code").AsString(8).NotNullable()
                    .WithColumn("PhoneFormat").AsString(50).NotNullable()
                    .WithColumn("CountryId").AsInt32().NotNullable().ForeignKey("FK_PhoneCountryCodes_CountryId", "public", "Countries", "Id").OnDelete(Rule.Cascade)
                    ;

                Create.Index("IDX_PhoneCountryCodes_CountryId")
                    .OnTable("PhoneCountryCodes")
                    .InSchema("public")
                    .OnColumn("CountryId");
            }

            // Предзаполняем данные
            InsertPrepopulatedData();

            #endregion

            #region Представления



            #endregion

            #endregion

            #endregion
        }

        public override void Down()
        {
            #region Схема public

            if (Schema.Schema("public").Table("PhoneCountryCodes").Exists())
            {
                Delete.Table("PhoneCountryCodes").InSchema("public");
            }
            if (Schema.Schema("public").Table("Countries").Exists())
            {
                Delete.Table("Countries").InSchema("public");
            }

            #endregion
        }

        /// <summary>
        /// Предзаполняет данные
        /// </summary>
        private void InsertPrepopulatedData()
        {
            if (Schema.Schema("public").Table("Countries").Exists())
            {
                var now = DateTime.UtcNow;
                var countries = typeof(Country).GetMembers()
                    .Select(x => x.GetCustomAttributes(typeof(CountryDetailsAttribute), false))
                    .SelectMany(x => x.Cast<CountryDetailsAttribute>());

                foreach (var country in countries)
                {
                    Insert.IntoTable("Countries")
                        .InSchema("public")
                        .Row(new
                        {
                            Name = country.Name,
                            Code = country.Code,
                            IsActive = country.IsActive,
                            CreatedAt = now,
                            UpdatedAt = now
                        });

                    Execute.Sql($@"INSERT INTO public.""PhoneCountryCodes""
                        (""Code"", ""PhoneFormat"", ""CountryId"")
                        SELECT '{country.PhoneCountryCode}', '{country.PhoneFormat}', ""Id"" AS ""CountryId""
                        FROM public.""Countries""
                        WHERE ""Code"" = '{country.Code}';
                    ");
                }
            }
        }
    }
}
