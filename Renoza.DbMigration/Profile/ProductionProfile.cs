using FluentMigrator;

namespace Renoza.DbMigration.Profile
{
    [Profile("Production")]
    public class ProductionProfile : FluentMigrator.Migration
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
