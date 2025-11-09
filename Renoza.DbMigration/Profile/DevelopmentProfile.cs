using FluentMigrator;

namespace Renoza.DbMigration.Profile
{
    [Profile("Development")]
    public class DevelopmentProfile : FluentMigrator.Migration
    {
        public override void Up()
        {
            //empty, not using
        }

        public override void Down()
        {
            //empty, not using
        }
    }
}
