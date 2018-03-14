using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosMetroPath
{
    internal class ManyRoutesCollection<TKey>
        where TKey : class, IId
    {
        private Dictionary<TwoItemsKey<TKey>, ICollection<IRoute>> Routes { get; set; } = new Dictionary<TwoItemsKey<TKey>, ICollection<IRoute>>();

        public IEnumerable<IRoute> this[TKey s1, TKey s2]
        {
            get
            {
                if (TryGetRoutes(s1, s2, out var routes))
                    return routes;

                return null;
            }
        }

        public int Count
        {
            get
            {
                var counter = 0;
                foreach (var r in Routes)
                    counter += r.Value.Count;
                return counter;
            }
        }

        private Func<IRoute, TwoItemsKey<TKey>> _getKey;
        private Func<IRoute, IRoute, bool> _comparator;
        public ManyRoutesCollection(Func<IRoute, TwoItemsKey<TKey>> getKey, Func<IRoute, IRoute, bool> comparator)
        {
            _getKey = getKey;
            _comparator = comparator;
        }
        
        internal static TwoItemsKey<TKey> GetKey(TKey s1, TKey s2)
        {
            return new TwoItemsKey<TKey>(s1, s2);
        }

        public bool Add(IRoute route)
        {
            var key = _getKey(route);

            if (Routes.TryGetValue(key, out var existsSet))
            {
                existsSet.Add(route);
                return true;
            }
            else
            {
                Routes.Add(key, new HashSet<IRoute> { route });
                return true;
            }
        }

        public bool Remove(IRoute route)
        {
            var key = _getKey(route);

            return Routes.Remove(key);
        }

        public bool ContainsKey(TKey s1, TKey s2)
        {
            var key = GetKey(s1, s2);
            return Routes.ContainsKey(key);
        }

        public bool Contains(IRoute route)
        {
            var key = _getKey(route);
            if (TryGetRoutes(key.Item1, key.Item2, out var routes))
            {
                foreach (var r in routes)
                    if (_comparator(route, r))
                        return true;
            }

            return false;
        }

        public bool TryGetRoutes(TKey s1, TKey s2, out IEnumerable<IRoute> routes)
        {
            var key = GetKey(s1, s2);

            if (Routes.TryGetValue(key, out var result))
            {
                routes = result;
                return true;
            }

            routes = null;
            return false;
        }
    }
}
