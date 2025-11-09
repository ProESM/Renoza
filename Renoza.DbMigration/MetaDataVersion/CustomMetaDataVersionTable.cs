using FluentMigrator.Runner.VersionTableInfo;

namespace Renoza.DbMigration.MetaDataVersion
{
    [VersionTableMetaData]
    public class CustomMetaDataVersionTable : IVersionTableMetaData
    {
        public virtual string SchemaName => "migration";
        public virtual string TableName => "MigrationLog";
        public virtual string ColumnName => "Version";
        public virtual string UniqueIndexName => "UC_MigrationLog_Version";
        public virtual string AppliedOnColumnName => "AppliedOn";
        public virtual bool CreateWithPrimaryKey { get; }
        public virtual string DescriptionColumnName => "Description";
        public virtual bool OwnsSchema => true;
    }
}
