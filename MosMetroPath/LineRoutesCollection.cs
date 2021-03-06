﻿using System;
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
    internal class LineRoutesCollection
    {
        private Dictionary<TwoItemsKey<Line>, RoutesCollection<Station>> Routes { get; set; } = new Dictionary<TwoItemsKey<Line>, RoutesCollection<Station>>();

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

            RoutesCollection<Station> routes;
            if (!Routes.TryGetValue(key, out routes))
            {
                routes = new RoutesCollection<Station>(r => new TwoItemsKey<Station>(r.From, r.To));
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
            var allRoutes = Routes[key];
            var routes = allRoutes
                .Where(r => r.From == from)
                .Union(allRoutes)
                .Where(r => r.To == from)
                .Select(r => new Route(r, true))
                .OrderByDescending(r => r.GetLines().Count())
                .ToArray();

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
