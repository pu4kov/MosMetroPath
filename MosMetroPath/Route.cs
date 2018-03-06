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
    [DebuggerDisplay("[{Length}] {From.Name} -> {To.Name} ({Timespan} секунд)")]
    public partial class Route: IRoute//, ICloneable, IEnumerable<IRoute>, IEnumerable<Station>
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
        /*
        public Route(Station from, Station to, int timespan)
        {
            Last = new RouteNode { Station = to, Next = null };
            First = new RouteNode { Station = from, timespan, Next = Last };

            OnAddStation(from, timespan);
            OnAddStation(to, 0);
        }

        public Route(StationRelation relation)
            :this(relation.From, relation.To, relation.Timespan)
        {

        }
        */
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
        }

        protected virtual void OnAddRoute(IRoute route)
        {
            Timespan += route.Timespan;
            Length += route.Length;
            Lines.UnionWith(route.GetLines());
        }

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
        /*
        public RouteNode Add(IRoute route)
        {
            RouteNode result;
            if (route.HasStation(From))
            {
                result = AddFirst(route.GetOtherStation(From), route.Timespan);
            }
            else if (route.HasStation(To))
            {
                result = AddLast(route.GetOtherStation(To), route.Timespan);
            }
            else
            {
                throw new ArgumentException();
            }

            OnAddStation(result.Station, route.Timespan);

            return result;
        }*/

        public IEnumerable<Line> GetLines() => Lines;
        /*
        public IEnumerable<StationRelation> GetLinesRelations()
        {
            if (First == Last)
                throw new NotImplementedException();

            var prior = First;
            var current = First.Next;
            while (current != null)
            {
                if (current.Station.Line != prior.Station.Line)
                {
                    yield return new StationRelation(prior.Station, current.Station, current.Timespan);
                }
                prior = current;
                current = current.Next;
            }
        }
        */
        /*
        public object Clone()
        {
            var first = First;
            var second = First.Next;
            var result = new Route(first.Station, second.Station, second.Timespan);

            var current = First;
            while (current != null)
            {
                result.AddLast(current.Station, current.Timespan);
                current = current.Next;
            }

            return result;
        }
        */

        public IEnumerable<Station> GetStations(bool reverse = false)
        {
            return new Enumerable<Station>(() => new StationEnumerator(this, reverse));
        }

        public IEnumerable<IRoute> GetRoutes(bool reverse = false)
        {
            return new Enumerable<IRoute>(() => new RouteEnumerator(this, reverse));
        }
        /*
        IEnumerator<IRoute> IEnumerable<IRoute>.GetEnumerator()
        {
            return GetRoutes().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetRoutes().GetEnumerator();
        }

        IEnumerator<Station> IEnumerable<Station>.GetEnumerator()
        {
            return GetStations().GetEnumerator();
        }
        */
    }
}
