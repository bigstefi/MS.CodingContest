using MS.CodingContest.Interfaces.Fence;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace MS.CodingContest.Helpers.Fence
{
    public class ForestProviderContainer
    {
        private List<IForestProvider> _forestProviders = new List<IForestProvider>();

        public IEnumerable<IForestProvider> ForestProviders 
        { 
            get 
            { 
                return _forestProviders; 
            } 
        }

        public ForestProviderContainer(string forestFilesFolderPath)
        {
            if(!Directory.Exists(forestFilesFolderPath))
            {
                throw new DirectoryNotFoundException($"Could not find folder {forestFilesFolderPath}");
            }

            string[] forestFiles = Directory.GetFiles(forestFilesFolderPath, "*.Fence.txt", SearchOption.TopDirectoryOnly);
            IEnumerable<string> forestFilesSorted = forestFiles.ToImmutableSortedSet();

            foreach(string forestFile in forestFilesSorted)
            {
                _forestProviders.Add(new ForestProvider(forestFile));
            }
        }
    }
}