using System.Collections.Concurrent;
using Logictracker.Configuration;
using Logictracker.QuadTree.Data;

namespace Logictracker.Tracker.Application.Dispatcher.Host.Handlers
{
    internal class QtreeInstanceManager
    {
        private static readonly ConcurrentDictionary<string, Repository> Repositories = new ConcurrentDictionary<string, Repository>();

        internal static Repository Get(string key)
        {

            var repo = Repositories.GetOrAdd(key, s =>
            {
                var so = new GridStructure();
                var instance = new Repository();
                instance.Open<GeoGrillas>(Config.Qtree.QtreeDirectory, ref so);
                return instance;
            });

            return repo;
        }
    }
}