using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace MS.CodingContest.Runner.Mef
{
    internal static class MefHelper
    {
        internal static IEnumerable<T> GetMefParticipants<T>(string[] args)
        {
            IEnumerable<T> participants = null;

            string mefAssembliesFolder = Path.Combine(Environment.CurrentDirectory, @"MefAssemblies");

            if(args != null && args.Length > 1)
            {
                mefAssembliesFolder = args[1];
            }

            using(AggregateCatalog aggregateCatalog = new AggregateCatalog())
            {
                string[] mefAssemlies = Directory.GetFiles(mefAssembliesFolder, "*.dll", SearchOption.AllDirectories);
                foreach(var mefAssembly in mefAssemlies)
                {
                    aggregateCatalog.Catalogs.Add(new AssemblyCatalog(mefAssembly));  
                }

                CatalogExportProvider exportProvider = new CatalogExportProvider(aggregateCatalog, true);
                CompositionContainer compositionContainer = new CompositionContainer(null, true, exportProvider);
                exportProvider.SourceProvider = compositionContainer;

                participants = compositionContainer.GetExportedValues<T>();
            }

            return participants;
        }
    }
}
