using Lombiq.Hosting.AlgoliaSearch.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment;
using Orchard.Localization;
using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Lombiq.Hosting.AlgoliaSearch.Handlers
{
    public class AlgoliaSearchSettingsPartHandler : ContentHandler
    {
        public Localizer T { get; set; }


        public AlgoliaSearchSettingsPartHandler(Work<IEncryptionService> encryptionService)
        {
            T = NullLocalizer.Instance;

            Filters.Add(new ActivatingFilter<AlgoliaSearchSettingsPart>("Site"));

            OnActivated<AlgoliaSearchSettingsPart>((context, part) =>
            {
                part.AdminApiKeyField.Loader(() =>
                {
                    return string.IsNullOrEmpty(part.EncodedAdminApiKey)
                        ? ""
                        : Encoding.UTF8.GetString(encryptionService.Value.Decode(Convert.FromBase64String(part.EncodedAdminApiKey)));
                });

                part.AdminApiKeyField.Setter((value) =>
                {
                    part.EncodedAdminApiKey = string.IsNullOrEmpty(value)
                        ? ""
                        : Convert.ToBase64String(encryptionService.Value.Encode(Encoding.UTF8.GetBytes(value)));

                    return value;
                });
            });
        }


        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            if (context.ContentItem.ContentType != "Site") return;

            base.GetItemMetadata(context);

            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("Algolia Search Settings")) { Id = "AlgoliaSearchSettings" });
        }
    }
}