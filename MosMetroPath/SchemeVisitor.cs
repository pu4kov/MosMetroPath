using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;

namespace MosMetroPath
{
    public class SchemeVisitor
    {
        /// <summary>
        /// Поиск самого быстрого пути между двумя станциями
        /// </summary>
        /// <param name="from">Станция отправления</param>
        /// <param name="to">Станция назначения</param>
        /// <returns>Найденный путь</returns>
        public IRoute FindRoute(Station from, Station to)
        {
            var scheme = from.Line.Scheme;
            if (scheme != to.Line.Scheme)
            {
                throw new ArgumentException();
            }
            // Счётчики для реализации алгоритма Дейкстры
            var counters = new Dictionary<Station, TimespanCounter>();

            var currentFrom = from;
            var currentTimespan = 0;
            // Добавляем начало маршрута в счётчики
            counters.Add(from, new TimespanCounter
            {
                Relation = null,
                IsReversed = false,
                IsFixed = true,
                TotalTimespan = 0
            });

            do
            {
                var rels = scheme.GetStationRelations(currentFrom);
                foreach (var relation in rels)      // Перебор всех соседних станций
                {
                    Station currentTo;
                    bool reverse;
                    
                    if (relation.From == currentFrom)
                    {
                        currentTo = relation.To;
                        reverse = false;
                    }
                    else
                    {
                        currentTo = relation.From;
                        reverse = true;
                    }

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
                            {   // Найден лучший путь, чем тот, что записан в "счётчиках"
                                counter.Relation = relation;
                                counter.IsReversed = reverse;
                                counter.TotalTimespan = relTotalTimespan;
                            }
                        }
                    }
                    else
                    {
                        counters.Add(currentTo,
                            new TimespanCounter
                            {
                                Relation = relation,
                                IsReversed = reverse,
                                TotalTimespan = relTotalTimespan,
                                IsFixed = false
                            });
                    }
                }
                // Поиск маршрута, который можно зафиксировать как оптимальный
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
                    throw new Exception($"Невозможно построить маршрут от станции {from.Name} до станции {to.Name}");
                }

                var newFixedCounter = counters[newFixed];
                newFixedCounter.IsFixed = true;

                currentFrom = newFixed;
                currentTimespan = newFixedCounter.TotalTimespan;
            }
            while (currentFrom != to);

            var lastStation = to;                   // Последняя станция в маршруте
            var penultStation = counters[to].From;  // Предпоследняя станция

            // Сборка итогового маршрута движения
            var result = new Route(new StationRelation(penultStation, lastStation, counters[to].Relation.Timespan));

            currentFrom = counters[penultStation].From;
            var currentCounter = counters[penultStation];
            while (currentFrom != null)
            {
                result.AddFirst(currentCounter.Relation);
                currentCounter = counters[currentFrom];
                currentFrom = currentCounter.From;
            }

            return result;
        }
        
        /// <summary>
        /// Поиск оптимальных маршрутов движения для посещения всех веток метро
        /// </summary>
        /// <param name="scheme">Схема метро</param>
        /// <returns>Наиболее быстрые маршруты</returns>
        public IEnumerable<IRoute> VisitAllLines(Scheme scheme)
        {
            if (scheme == null)
                throw new ArgumentNullException();

            var stations = scheme.GetAllLineRelationStations();    // Все пересадочные станции всех веток

            LineRoutesCollection linesRoutes = new LineRoutesCollection();  // Маршруты между линиями

            Stack<IRoute> routes = new Stack<IRoute>(); // Маршруты, которые могут стать решением

            int cnt = 0;
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
                        if (linesRoutes.Add(newRoute))
                            routes.Push(newRoute);
                        else
                            cnt++;
                    }
                }
            }

            List<IRoute> result = new List<IRoute>();
            int resultTimespan = int.MaxValue;

            while (routes.Count > 0)
            {
                /*
                Debug.WriteLine("====================================");
                Debug.WriteLine("routes");
                foreach (var r in routes)
                    Debug.WriteLine($"{r.From.Name} -> {r.To.Name} (l:{r.Length}, t:{r.Timespan})");
                Debug.WriteLine("results");
                foreach (var r in result)
                    Debug.WriteLine($"{r.From.Name} -> {r.To.Name} (l:{r.Length}, t:{r.Timespan})");

                Debug.WriteLine("====================================");
                */

                var route = routes.Pop();
                var routeTimespan = route.Timespan;
                var targetLines = scheme.GetLinesExclude(route.GetLines()); // Ветки, которые еще осталось посетить
                if (targetLines.FirstOrDefault() == null)
                {   // Все ветки охвачены маршрутом
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
                else if(routeTimespan < resultTimespan)
                {   // Время текущего маршрута меньше времени уже найденных полных маршрутов
                    foreach (var targetLine in targetLines)
                    {
                        foreach (var additionalRoute in linesRoutes.GetRoutes(route.To, targetLine))
                        {
                            // Дальше обрабатываются только те маршруты, которые короче уже найденных
                            if (resultTimespan >= additionalRoute.Timespan + routeTimespan)
                            {
                                var newRoute = Route.Union(route, additionalRoute);
                                routes.Push(newRoute);
                            }
                        }
                    }
                }
            }
            

            return result;
        }

        private enum RouteCompareResult
        {
            Equals, // маршруты одинаковы
            Better, // Первый маршрут лучше
            Worse,  // Второй маршрут лучше
            Unknown // Нельзя однозначно сравнить маршруты
        }

        /// <summary>
        /// Сравнение двух маршрутов
        /// </summary>
        /// <param name="r1">Первый маршрут</param>
        /// <param name="r2">Второй маршрут</param>
        /// <returns>Результат сравнения</returns>
        private RouteCompareResult CompareRoutes(IRoute r1, IRoute r2)
        {
            if ((r1.From == r2.From && r1.To == r2.To)
                || (r1.To == r2.From && r1.From == r2.To))
            {
                var l1 = new HashSet<Line>(r1.GetLines());
                var l2 = new HashSet<Line>(r2.GetLines());
                var le1 = l1.Except(l2);
                var le2 = l2.Except(l1);

                if (le1.FirstOrDefault() == null && le2.FirstOrDefault() == null)
                {
                    if (r1.Timespan < r2.Timespan)
                    {
                        return RouteCompareResult.Better;
                    }
                    else if (r1.Timespan == r2.Timespan)
                    {
                        return RouteCompareResult.Equals;
                    }
                    else
                    {
                        return RouteCompareResult.Worse;
                    }
                }
                else if (le1.FirstOrDefault() == null)
                {
                    if (r2.Timespan <= r1.Timespan)
                        return RouteCompareResult.Worse;
                }
                else if (le2.FirstOrDefault() == null)
                {
                    if (r1.Timespan < r2.Timespan)
                        return RouteCompareResult.Better;
                }
            }

            return RouteCompareResult.Unknown;
        }

        private class TimespanCounter
        {
            public StationRelation Relation { get; set; }
            public bool IsReversed { get; set; }
            public int TotalTimespan { get; set; }
            public bool IsFixed { get; set; }

            public Station From => (IsReversed) ? Relation?.To : Relation?.From;
        }
    }
}
