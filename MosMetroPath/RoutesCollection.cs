using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;

namespace MosMetroPath
{
    public class RoutesCollection: IEnumerable<IRoute>
    {
        private Dictionary<RouteKey, IRoute> Routes { get; set; } = new Dictionary<RouteKey, IRoute>();

        public IRoute this[Station s1, Station s2]
        {
            get
            {
                if (TryGetRoute(s1, s2, out var route))
                    return route;

                return null;
            }
        }

        public int Count => Routes.Count;

        public bool Add(IRoute route)
        {
            var key = RouteKey.FromRoute(route);

            if (Routes.TryGetValue(key, out var existsRoute))
            {
                return false;
            }
            else
            {
                Routes.Add(key, route);
                return true;
            }
        }

        public bool Remove(IRoute route)
        {
            var key = RouteKey.FromRoute(route);

            return Routes.Remove(key);
        }

        public bool ContainsKey(Station s1, Station s2)
        {
            var key = new RouteKey(s1, s2);
            return Routes.ContainsKey(key);
        }

        public bool TryGetRoute(Station s1, Station s2, out IRoute route)
        {
            var key = new RouteKey(s1, s2);

            return Routes.TryGetValue(key, out route);
        }

        public IEnumerator<IRoute> GetEnumerator()
        {
            return Routes.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [DebuggerDisplay("{Station1.Name}-{Station2.Name}")]
        private class RouteKey : IEquatable<RouteKey>
        {
            public Station Station1 { get; }
            public Station Station2 { get; }

            public RouteKey(Station s1, Station s2)
            {
                Station1 = s1;
                Station2 = s2;
            }

            public static RouteKey FromRoute(IRoute route)
            {
                return new RouteKey(route.From, route.To);
            }

            public bool Equals(RouteKey other)
            {
                return
                    ((Station1 == other.Station1 && Station2 == other.Station2)
                    || (Station2 == other.Station1 && Station1 == other.Station2));
            }

            public override int GetHashCode()
            {
                return Station1.Id + Station2.Id;
            }

            public override bool Equals(object obj)
            {
                if (obj is RouteKey other)
                    return Equals(other);

                return false;
            }
        }
    }
}
