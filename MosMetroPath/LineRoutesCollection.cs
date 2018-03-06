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
        private Dictionary<LineRouteKey, RoutesCollection> Routes { get; set; } = new Dictionary<LineRouteKey, RoutesCollection>();

        public IEnumerable<IRoute> this[Line l1, Line l2]
        {
            get
            {
                if (TryGetRoutes(l1, l2, out var routes))
                    return routes;

                return null;
            }
        }

        public bool Add(IRoute route)
        {
            var key = LineRouteKey.FromRoute(route);
            
            RoutesCollection routes;
            if (!Routes.TryGetValue(key, out routes))
            {
                routes = new RoutesCollection();
                Routes.Add(key, routes);
            }

            return routes.Add(route);
        }
        /*
        public bool Remove(Route route)
        {
            var key = LineRouteKey.FromRoute(route);

            return Routes.Remove(key);
        }*/

        public bool ContainsKey(Line l1, Line l2)
        {
            var key = new LineRouteKey(l1, l2);
            return Routes.ContainsKey(key);
        }

        public bool ContainsRoute(Station s1, Station s2)
        {
            var key = new LineRouteKey(s1.Line, s2.Line);
            if (Routes.TryGetValue(key, out var collection))
            {
                return collection.ContainsKey(s1, s2);
            }

            return false;
        }

        public IRoute GetRoute(Station s1, Station s2)
        {
            var key = new LineRouteKey(s1.Line, s2.Line);
            if (Routes.TryGetValue(key, out var collection))
            {
                return collection[s1, s2];
            }

            return null;
        }

        public IEnumerable<IRoute> GetRoutes(Station from, Line to)
        {
            var key = new LineRouteKey(from.Line, to);
            var routes = Routes[key].Where(r => r.From == from || r.To == from).ToArray();

            return routes;
        }

        public bool TryGetRoutes(Line l1, Line l2, out IEnumerable<IRoute> routes)
        {
            var key = new LineRouteKey(l1, l2);

            if (Routes.TryGetValue(key, out var r))
            {
                routes = r;
                return true;
            }

            routes = null;
            return false;
        }

        [DebuggerDisplay("{Line1.Name}-{Line2.Name}")]
        private class LineRouteKey : IEquatable<LineRouteKey>
        {
            public Line Line1 { get; }
            public Line Line2 { get; }

            public LineRouteKey(Line l1, Line l2)
            {
                Line1 = l1;
                Line2 = l2;
            }

            public static LineRouteKey FromRoute(IRoute route)
            {
                return new LineRouteKey(route.From.Line, route.To.Line);
            }

            public bool Equals(LineRouteKey other)
            {
                return
                    ((Line1 == other.Line1 && Line2 == other.Line2)
                    || (Line2 == other.Line1 && Line1 == other.Line2));
            }

            public override int GetHashCode()
            {
                return Line1.Id + Line2.Id;
            }

            public override bool Equals(object obj)
            {
                if (obj is LineRouteKey other)
                    return Equals(other);

                return false;
            }
        }
    }
}
