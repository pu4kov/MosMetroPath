using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace MosMetroPath
{
    public class SchemeVisitor
    {
        public IRoute FindRoute(Station from, Station to)
        {
            var scheme = from.Line.Scheme;
            if (scheme != to.Line.Scheme)
            {
                throw new ArgumentException();
            }

            var counters = new Dictionary<Station, TimespanCounter>();

            var currentFrom = from;
            var currentTimespan = 0;

            counters.Add(from, new TimespanCounter { From = null, IsFixed = true, TotalTimespan = 0, CurrentTimespan = 0 });

            do
            {
                var rels = scheme.GetStationRelations(currentFrom);
                foreach (var relation in rels)
                {
                    var currentTo = relation.GetOtherStation(currentFrom);
                    var relTotalTimespan = currentTimespan + relation.Timespan;
                    if (counters.TryGetValue(currentTo, out var counter))
                    {
                        if (counter.IsFixed)
                        {
                            continue;
                        }
                        else
                        {
                            if (counter.TotalTimespan > relTotalTimespan)
                            {
                                counter.From = currentFrom;
                                counter.TotalTimespan = relTotalTimespan;
                            }
                        }
                    }
                    else
                    {
                        counters.Add(currentTo, new TimespanCounter { From = currentFrom, CurrentTimespan = relation.Timespan, TotalTimespan = relTotalTimespan, IsFixed = false });
                    }
                }

                var minTimespan = int.MaxValue;
                Station newFixed = null;

                foreach (var c in counters)
                {
                    if (!c.Value.IsFixed
                        && c.Value.TotalTimespan < minTimespan)
                    {
                        minTimespan = c.Value.TotalTimespan;
                        newFixed = c.Key;
                    }
                }

                if (newFixed == null)
                {
                    throw new Exception();
                }

                var newFixedCounter = counters[newFixed];
                newFixedCounter.IsFixed = true;

                currentFrom = newFixed;
                currentTimespan = newFixedCounter.TotalTimespan;
            }
            while (currentFrom != to);

            var lastStation = to;                   // Последняя станция в маршруте
            var penultStation = counters[to].From;  // Предпоследняя станция

            var result = new Route(penultStation, lastStation, counters[to].CurrentTimespan);

            currentFrom = counters[penultStation].From;
            var currentCounter = counters[penultStation];
            while (currentFrom != null)
            {
                result.AddFirst(currentFrom, currentCounter.CurrentTimespan);
                currentCounter = counters[currentFrom];
                currentFrom = currentCounter.From;
            }

            return result;
        }
        
        public IEnumerable<CompositeRoute> VisitAllLines(Scheme scheme)
        {
            if (scheme == null)
                throw new ArgumentNullException();

            var stations = scheme.GetAllLineRelationStations();    // Все пересадочные станции всех веток

            LineRoutesCollection linesRoutes = new LineRoutesCollection();  // Маршруты между линиями

            Stack<CompositeRoute> routes = new Stack<CompositeRoute>(); // Маршруты, которые могут стать решением

            // Построение маршрутов между всеми парами пересадочных станций
            foreach (var s1 in stations)
            {
                foreach (var s2 in stations)
                {
                    // Поиск маршрута только если станции на разных ветках
                    // и маршрут не был ранее уже найден
                    if (s1.Line != s2.Line
                        && !linesRoutes.ContainsRoute(s1, s2))
                    {
                        var newRoute = FindRoute(s1, s2);
                        //baseRoutes.Add(newRoute);
                        linesRoutes.Add(newRoute);
                        routes.Push(new CompositeRoute(newRoute));
                    }
                }
            }

            List<CompositeRoute> result = new List<CompositeRoute>();
            int resultTimespan = int.MaxValue;

            while (routes.Count > 0)
            {
                var route = routes.Pop();
                var routeTimespan = route.Timespan;
                var targetLines = scheme.GetLinesExclude(route.GetLines());
                if (targetLines.FirstOrDefault() == null)
                {
                    if (resultTimespan > routeTimespan)
                    {   // Маршрут короче тех, что уже были найдены
                        result.Clear();
                        resultTimespan = routeTimespan;
                        result.Add(route);
                    }
                    else if (resultTimespan == routeTimespan)
                    {
                        result.Add(route);
                    }
                }
                else if (routeTimespan >= resultTimespan)
                {

                }
                else
                {
                    foreach (var targetLine in targetLines)
                    {
                        foreach (var additionalRoute in linesRoutes.GetRoutes(route.To, targetLine))
                        {
                            // Проверяем, что уже найденные маршруты длиннее текущего
                            if (resultTimespan >= additionalRoute.Timespan + routeTimespan)
                            {
                                var newRoute = route.Union(additionalRoute);
                                routes.Push(newRoute);
                            }
                        }
                    }
                }
            }
            

            return result;

            /*
            var startStations = scheme.GetAllLineRelationStations();

            var routes = new Queue<Route>();

            foreach (var start in startStations)
            {
                var otherLines = scheme.GetLinesExclude(new Line[] { start.Line });
                var otherStations = scheme.GetLinesRelationStations(otherLines);
                
                foreach (var station in otherStations)
                {
                    var route = FindRoute(start, station);
                    routes.Enqueue(route);
                }
            }

            while (routes.Count > 0)
            {
                var route = routes.Dequeue();

                var otherLines = scheme.GetLinesExclude(route.Lines);
                if (otherLines.Count == 0)
                    yield return route;

                var otherStations = scheme.GetLinesRelationStations(otherLines);

                foreach (var station in otherStations)
                {
                    if (station == route.To)
                        continue;
                    var newRoute = FindRoute(route.To, station);
                    //routes.Enqueue(route.Concat(newRoute));
                }
            }*/
        }

        private class TimespanCounter
        {
            public Station From { get; set; }
            public int CurrentTimespan { get; set; }
            public int TotalTimespan { get; set; }
            public bool IsFixed { get; set; }
        }

        private class RouteComparer : IEqualityComparer<IRoute>
        {
            public bool Equals(IRoute x, IRoute y)
            {
                throw new NotImplementedException();
            }

            public int GetHashCode(IRoute obj)
            {
                return obj.From.GetHashCode() + obj.To.GetHashCode();
            }
        }
    }
}
