using Lombiq.Hosting.AlgoliaSearch.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;

namespace Lombiq.Hosting.AlgoliaSearch.Migrations
{
    public class AlgoliaSearchSettingsPartMigrations : DataMigrationImpl
    {
        public int Create()
        {
            ContentDefinitionManager.AlterTypeDefinition("Site", type => type
                .WithPart(nameof(AlgoliaSearchSettingsPart)));

            return 1;
        }
    }
}