using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MosMetroPath
{
    /// <summary>
    /// Коллекция маршрутов между линиями
    /// </summary>
    public class LineRoutesCollection
    {
        private Dictionary<TwoItemsKey<Line>, RoutesCollection> Routes { get; set; } = new Dictionary<TwoItemsKey<Line>, RoutesCollection>();

        public IEnumerable<IRoute> this[Line l1, Line l2]
        {
            get
            {
                if (TryGetRoutes(l1, l2, out var routes))
                    return routes;

                return null;
            }
        }

        private TwoItemsKey<Line> GetKey(IRoute route)
        {
            return new TwoItemsKey<Line>(route.From.Line, route.To.Line);
        }

        private TwoItemsKey<Line> GetKey(Line l1, Line l2)
        {
            return new TwoItemsKey<Line>(l1, l2);
        }

        public bool Add(IRoute route)
        {
            var key = GetKey(route);
            
            RoutesCollection routes;
            if (!Routes.TryGetValue(key, out routes))
            {
                routes = new RoutesCollection();
                Routes.Add(key, routes);
            }

            return routes.Add(route);
        }

        public bool ContainsKey(Line l1, Line l2)
        {
            var key = GetKey(l1, l2);
            return Routes.ContainsKey(key);
        }

        public bool ContainsRoute(Station s1, Station s2)
        {
            var key = GetKey(s1.Line, s2.Line);
            if (Routes.TryGetValue(key, out var collection))
            {
                return collection.ContainsKey(s1, s2);
            }

            return false;
        }

        public IRoute GetRoute(Station s1, Station s2)
        {
            var key = GetKey(s1.Line, s2.Line);
            if (Routes.TryGetValue(key, out var collection))
            {
                return collection[s1, s2];
            }

            return null;
        }

        public IEnumerable<IRoute> GetRoutes(Station from, Line to)
        {
            var key = GetKey(from.Line, to);
            var routes = Routes[key].Where(r => r.From == from || r.To == from).ToArray();

            return routes;
        }

        public bool TryGetRoutes(Line l1, Line l2, out IEnumerable<IRoute> routes)
        {
            var key = GetKey(l1, l2);

            if (Routes.TryGetValue(key, out var r))
            {
                routes = r;
                return true;
            }

            routes = null;
            return false;
        }
    }
}
