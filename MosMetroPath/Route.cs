using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MosMetroPath
{
    /// <summary>
    /// Маршрут
    /// </summary>
    [DebuggerDisplay("[{Length} станций] {From.Name} -> {To.Name} ({Timespan} сек.)")]
    public partial class Route: IRoute
    {
        /// <summary>
        /// Первый элемент маршрута
        /// </summary>
        private RouteNode First { get; set; }
        /// <summary>
        /// Последний элемент маршрута
        /// </summary>
        private RouteNode Last { get; set; }

        /// <summary>
        /// Ветки метро, через которые проходит маршрут
        /// </summary>
        protected HashSet<Line> Lines { get; } = new HashSet<Line>();

        /// <summary>
        /// Начальная стацния маршрута
        /// </summary>
        public Station From => First.From;

        /// <summary>
        /// Конечная станция маршрута
        /// </summary>
        public Station To => Last.To;

        /// <summary>
        /// Полная продолжительность маршрута
        /// </summary>
        public int Timespan { get; protected set; } = 0;

        /// <summary>
        /// Количество станций, через которые проходит маршрут
        /// </summary>
        public int Length { get; private set; } = 0;

        public Route(IRoute route, bool reverse = false)
        {
            First = new RouteNode
            {
                Route = route,
                IsReversed = reverse,
                Next = null,
                Prior = null
            };
            Last = First;
            OnAddRoute(route);
            ++Length;
        }

        protected virtual void OnAddRoute(IRoute route)
        {
            Timespan += route.Timespan;
            Length += route.Length - 1;
            Lines.UnionWith(route.GetLines());
        }

        /// <summary>
        /// Добавление маршрута в начало текущего (с автоповоротом добавляемого маршрута)
        /// </summary>
        /// <param name="route">Добавляемый маршрут</param>
        public void AddFirst(IRoute route)
        {
            // Определение необходимости разворота маршрута
            bool reverse;
            if (route.To == From)
                reverse = false;
            else if (route.From == From)
                reverse = true;
            else
                throw new ArgumentOutOfRangeException(nameof(route));

            var newNode = new RouteNode
            {
                Route = route,
                IsReversed = reverse,
                Next = First,
                Prior = null
            };

            First.Prior = newNode;
            First = newNode;

            OnAddRoute(route);
        }

        /// <summary>
        /// Добавление маршрута в конец текущего (с автоповоротом добавляемого маршрута)
        /// </summary>
        /// <param name="route">Добавляемый маршрут</param>
        public void AddLast(IRoute route)
        {
            // Определение необходимости разворота маршрута
            bool reverse;
            if (route.To == To)
                reverse = true;
            else if (route.From == To)
                reverse = false;
            else
                throw new ArgumentOutOfRangeException(nameof(route));

            if (route.Length == 2)
            {
                route = new StationRelation(To, (reverse) ? route.From : route.To, route.Timespan);
                reverse = false;
            }

            var newNode = new RouteNode
            {
                Route = route,
                IsReversed = reverse,
                Next = null,
                Prior = Last
            };

            Last.Next = newNode;
            Last = newNode;

            OnAddRoute(route);
        }

        public IEnumerable<Line> GetLines() => Lines;

        /// <summary>
        /// Создание нового маршрута, путём добавления маршрута справа к исходному маршруту (при необходимости, с поворотом добавляемого маршрута)
        /// </summary>
        /// <param name="a">Исходный маршрут</param>
        /// <param name="b">Добавляемый маршрут</param>
        /// <returns>Полученный маршрут</returns>
        public static Route Union(IRoute a, IRoute b)
        {
            var result = new Route(a, false);
            result.AddLast(b);

            return result;
        }

        public IEnumerable<Station> GetStations(bool reverse)
        {
            if (First == Last)
                return First.Route.GetStations(First.IsReversed ^ reverse);

            var startNode = (reverse) ? Last : First;
            
            return new Enumerable<Station>(
                () =>
                {
                    return new StationEnumerator(startNode, reverse);
                });
        }

        public IEnumerable<IRoute> GetRoutes(bool reverse)
        {
            if (First == Last)
                return First.Route.GetRoutes(First.IsReversed ^ reverse);

            var startNode = (reverse) ? Last : First;
            
            return new Enumerable<IRoute>(
                () =>
                {
                    return new Enumerator<IRoute>(startNode, reverse,
                        (node) =>
                        {
                            return node.Route.GetRoutes(node.IsReversed ^ reverse).GetEnumerator();
                        });
                });
        }

        public override int GetHashCode()
        {
            return From.Id + To.Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is IRoute other)
            {
                if (((From == other.From && To == other.To)
                    || (To == other.From && From == other.To))
                    && Length == other.Length
                    && Timespan == other.Timespan)
                {
                    var thisRoutes = GetRoutes(false);
                    var otherRoutes = (From == other.From) ? other.GetRoutes(false) : other.GetRoutes(true);

                    return (thisRoutes.Count() == otherRoutes.Count()) && (thisRoutes.Intersect(otherRoutes).FirstOrDefault() == null);
                }
                return false;
            }
            return base.Equals(obj);
        }

        public IEnumerator<Station> GetEnumerator()
        {
            return GetStations(false).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool HasStation(Station station)
        {
            return GetStations(false).FirstOrDefault(s => s.Equals(station)) != null;
        }
    }
}
