using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;

namespace MosMetroPath
{
    /// <summary>
    /// Коллекция маршрутов с уникальным значением пары ключей: пункт отправления, пункт назначения (маршрут a -> b == b -> a)
    /// </summary>
    public class RoutesCollection: IEnumerable<IRoute>
    {
        private Dictionary<TwoItemsKey<Station>, IRoute> Routes { get; set; } = new Dictionary<TwoItemsKey<Station>, IRoute>();

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

        private TwoItemsKey<Station> GetKey(IRoute route)
        {
            return new TwoItemsKey<Station>(route.From, route.To);
        }

        private TwoItemsKey<Station> GetKey(Station s1, Station s2)
        {
            return new TwoItemsKey<Station>(s1, s2);
        }

        public bool Add(IRoute route)
        {
            var key = GetKey(route);

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
            var key = GetKey(route);

            return Routes.Remove(key);
        }

        public bool ContainsKey(Station s1, Station s2)
        {
            var key = GetKey(s1, s2);
            return Routes.ContainsKey(key);
        }

        public bool TryGetRoute(Station s1, Station s2, out IRoute route)
        {
            var key = GetKey(s1, s2);

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
    }
}
