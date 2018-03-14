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
    internal class RoutesCollection<TKey>: IEnumerable<IRoute>
        where TKey: class, IId
    {
        private Dictionary<TwoItemsKey<TKey>, IRoute> Routes { get; } = new Dictionary<TwoItemsKey<TKey>, IRoute>();

        public IRoute this[TKey s1, TKey s2]
        {
            get
            {
                if (TryGetRoute(s1, s2, out var route))
                    return route;

                return null;
            }
        }

        public int Count => Routes.Count;

        private Func<IRoute, TwoItemsKey<TKey>> _getKey;
        public RoutesCollection(Func<IRoute, TwoItemsKey<TKey>> getKey)
        {
            Routes = new Dictionary<TwoItemsKey<TKey>, IRoute>();
            _getKey = getKey;
        }

        public RoutesCollection(RoutesCollection<TKey> other)
        {
            Routes = new Dictionary<TwoItemsKey<TKey>, IRoute>(other.Routes);
            _getKey = other._getKey;
        }

        internal static TwoItemsKey<TKey> GetKey(TKey s1, TKey s2)
        {
            return new TwoItemsKey<TKey>(s1, s2);
        }

        public bool Add(IRoute route)
        {
            var key = _getKey(route);

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
            var key = _getKey(route);

            return Routes.Remove(key);
        }

        public bool ContainsKey(TKey s1, TKey s2)
        {
            var key = GetKey(s1, s2);
            return Routes.ContainsKey(key);
        }

        public bool TryGetRoute(TKey s1, TKey s2, out IRoute route)
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
