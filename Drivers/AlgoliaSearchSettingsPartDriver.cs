using Lombiq.Hosting.AlgoliaSearch.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lombiq.Hosting.AlgoliaSearch.Drivers
{
    public class AlgoliaSearchSettingsPartDriver : ContentPartDriver<AlgoliaSearchSettingsPart>
    {
        protected override DriverResult Editor(AlgoliaSearchSettingsPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_AlgoliaSearchSettings_Edit", () => shapeHelper.EditorTemplate(
                 TemplateName: "Parts.AlgoliaSearchSettings",
                 Model: part,
                 Prefix: Prefix));
        }

        protected override DriverResult Editor(AlgoliaSearchSettingsPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}