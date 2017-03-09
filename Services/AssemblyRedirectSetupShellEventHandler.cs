using Orchard.Environment;
using System;
using System.Reflection;

namespace Lombiq.Hosting.AlgoliaSearch.Services
{
    public class AssemblyRedirectSetupShellEventHandler : IOrchardShellEvents
    {
        public void Activated()
        {
            // Trying to remove first so no duplicate event registration can occur.
            AppDomain.CurrentDomain.AssemblyResolve -= SystemNetHttpPrimitives;
            AppDomain.CurrentDomain.AssemblyResolve += SystemNetHttpPrimitives;
        }

        public void Terminating()
        {
        }


        private Assembly SystemNetHttpPrimitives(object sender, ResolveEventArgs args)
        {
            // These are here instead of adding assembly redirects to the Web.config. 
            switch (args.Name.Split(',')[0])
            {
                case "System.Net.Http.Primitives":
                    return Assembly.Load("System.Net.Http.Primitives");
                default:
                    return null;
            }
        }
    }
}