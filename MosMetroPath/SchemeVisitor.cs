using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
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

            var stations = scheme.GetAllLineRelationStations().ToList();    // Все пересадочные станции всех веток

            LineRoutesCollection linesRoutes = new LineRoutesCollection();  // Маршруты между линиями
            PartialRoutesCollection routes = new PartialRoutesCollection(); // Маршруты, которые могут стать решением

            // Построение маршрутов между всеми парами пересадочных станций
            for (int i = 0; i < stations.Count - 1; ++i)
            {
                var s1 = stations[i];
                for (int j = i + 1; j < stations.Count; ++j)
                {
                    var s2 = stations[j];
                    // Поиск маршрута только если станции на разных ветках
                    // и маршрут не был ранее уже найден
                    if (s1.Line != s2.Line
                        && !linesRoutes.ContainsRoute(s1, s2))
                    {
                        var newRoute = FindRoute(s1, s2);
                        if (linesRoutes.Add(newRoute))
                        {
                            routes.Add(newRoute);
                        }
                    }
                }
            }

            ResultCollection result = new ResultCollection();
            
            while (routes.TryPop(out var route))
            {
                if (route.Timespan > result.Timespan)
                    continue;

                var targetLines = scheme.GetLinesExclude(route.GetLines()); // Ветки, которые еще осталось посетить
                if (targetLines.FirstOrDefault() == null)
                {
                    result.Add(route);
                }
                else if(route.Timespan < result.Timespan)
                {   // Время текущего маршрута меньше времени уже найденных полных маршрутов
                    foreach (var targetLine in targetLines)
                    {
                        foreach (var additionalRoute in linesRoutes.GetRoutes(route.To, targetLine))
                        {
                            // Дальше обрабатываются только те маршруты, которые короче уже найденных
                            if (result.Timespan >= additionalRoute.Timespan + route.Timespan)
                            {
                                var newRoute = Route.Union(route, additionalRoute);
                                routes.Add(newRoute);
                            }
                        }
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Поиск оптимального маршрута с посещением всех веток метро с помощью метода ветвей и границ
        /// </summary>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public IEnumerable<IRoute> VisitAllLinesByBranchAndBound(Scheme scheme)
        {
            if (scheme == null)
                throw new ArgumentNullException();

            var stations = scheme.GetAllLineRelationStations();    // Все пересадочные станции всех веток


            var lines = scheme.GetLines().ToArray();    // все ветки метро
            var linesStations = new Station[lines.Length][];    // массив станций переходов для каждой ветки
            var indeces = new int[lines.Length];    // индекс станции для каждой ветки
            for (var i = 0; i < lines.Length; ++i)
            {
                linesStations[i] = lines[i].GetRelationStations().ToArray();
                indeces[i] = 0;
            }

            var lastLineInd = indeces.Length - 1;

            var cache = new RoutesCollection<Station>((r) => new TwoItemsKey<Station>(r.From, r.To));
            var visitor = new AllLinesVisitor();

            ActionBlock<IEnumerable<IRoute>> addRouteAction = new ActionBlock<IEnumerable<IRoute>>(
                (routes) =>
                {
                    visitor.Push(routes);
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 4 });

            int counter = 0;

            bool next = true;
            do
            {
                // Обработка станций
                var routes = new List<IRoute>();
                for (int fromLineId = 0; fromLineId < indeces.Length - 1; ++fromLineId)
                {
                    var fromStation = linesStations[fromLineId][indeces[fromLineId]];
                    for (int toLineId = fromLineId + 1; toLineId < indeces.Length; ++toLineId)
                    {
                        var toStation = linesStations[toLineId][indeces[toLineId]];
                        IRoute route;
                        if (!cache.TryGetRoute(fromStation, toStation, out route))
                        {
                            route = FindRoute(fromStation, toStation);
                            cache.Add(route);
                        }

                        routes.Add(route);
                    }
                }
                addRouteAction.Post(routes);
                counter++;
                
                // ограничение маршрутов для тестирования
                if (counter > 10)
                    next = false;
                
                // Переход к следующей комбинации
                var lineInd = lastLineInd;
                do
                {
                    ++indeces[lineInd];
                    if (indeces[lineInd] >= linesStations[lineInd].Length)
                    {
                        if (lineInd == 0)
                        {   // перебраны все возможные комбинации
                            next = false;
                            break;
                        }
                        else
                        {
                            indeces[lineInd] = 0;
                        }
                    }
                    else
                    {
                        break;
                    }
                    --lineInd;
                }
                while (lineInd >= 0);
            }
            while (next);

            addRouteAction.Complete();
            addRouteAction.Completion.Wait();

            visitor.Run();

            return visitor.Results;
        }

        private class TimespanCounter
        {
            public StationRelation Relation { get; set; }
            public bool IsReversed { get; set; }
            public int TotalTimespan { get; set; }
            public bool IsFixed { get; set; }

            public Station From => (IsReversed) ? Relation?.To : Relation?.From;
        }

        private class PartialRoutesCollection
        {
            private Dictionary<TwoItemsKey<Station>, ICollection<IRoute>> _routes = new Dictionary<TwoItemsKey<Station>, ICollection<IRoute>>();
            private LinkedList<IRoute> _betterRoutes = new LinkedList<IRoute>();

            private TwoItemsKey<Station> GetKey(Station s1, Station s2)
            {
                return new TwoItemsKey<Station>(s1, s2);
            }

            private void OnAddRoute(IRoute route)
            {
                if (_betterRoutes.First == null)
                {
                    _betterRoutes.AddFirst(route);
                }
                else
                {
                    var rLinesCnt = route.GetLines().Count();

                    var currentNode = _betterRoutes.First;
                    do
                    {
                        var cLinesCnt = currentNode.Value.GetLines().Count();
                        if (rLinesCnt > cLinesCnt)
                        {
                            _betterRoutes.AddBefore(currentNode, route);
                            break;
                        }
                        else if (rLinesCnt == cLinesCnt)
                        {
                            if (currentNode.Value.Timespan > route.Timespan)
                            {
                                _betterRoutes.AddBefore(currentNode, route);
                                break;
                            }
                        }
                        currentNode = currentNode.Next;
                    }
                    while (currentNode != null);
                    if (currentNode == null)
                    {
                        _betterRoutes.AddLast(route);
                    }
                }
            }

            public bool TryPop(out IRoute route)
            {
                if (_betterRoutes.First != null)
                {
                    route = _betterRoutes.First.Value;

                    _betterRoutes.RemoveFirst();
                    return true;
                }

                route = null;
                return false;
            }

            public bool Add(IRoute route)
            {
                var key = GetKey(route.From, route.To);
                var result = true;

                if (_routes.TryGetValue(key, out var routes))
                {
                    foreach (var r in routes)
                    {
                        if (route.Equals(r))
                        {
                            result = false;
                            break;
                        }
                    }
                }
                else
                {
                    routes = new List<IRoute>();
                    _routes.Add(key, routes);
                }

                if (result)
                {
                    routes.Add(route);
                    OnAddRoute(route);
                }

                return result;
            }
        }

        [DebuggerDisplay("Timespan = {Timespan}, Count = {routes.Count}")]
        private class ResultCollection : IEnumerable<IRoute>
        {
            private List<IRoute> routes = new List<IRoute>();

            public int Timespan { get; private set; } = int.MaxValue;

            public bool Add(IRoute route)
            {
                bool adding = true;
                if (route.Timespan == Timespan)
                {
                    foreach (var r in routes)
                    {
                        if (r.Length == route.Length)
                        {
                            if (r.GetRoutes(false).Except(route.GetRoutes(false)).FirstOrDefault() == null)
                            {
                                adding = false;
                                break;
                            }
                        }
                    }
                }
                else if (route.Timespan < Timespan)
                {
                    routes.Clear();
                }
                else
                {
                    adding = false;
                }

                if (adding)
                {
                    Timespan = route.Timespan;
                    routes.Add(route);
                }

                return adding;
            }

            public IEnumerator<IRoute> GetEnumerator()
            {
                return routes.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
