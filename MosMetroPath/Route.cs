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
    public class Route: IRoute, IEnumerable<Station>, ICloneable
    {
        /// <summary>
        /// Первый элемент маршрута
        /// </summary>
        protected RouteNode First { get; private set; }
        /// <summary>
        /// Последний элемент маршрута
        /// </summary>
        protected RouteNode Last { get; private set; }

        /// <summary>
        /// Ветки метро, через которые проходит маршрут
        /// </summary>
        protected HashSet<Line> Lines { get; } = new HashSet<Line>();

        /// <summary>
        /// Начальная стацния маршрута
        /// </summary>
        public Station From => First.Station;

        /// <summary>
        /// Конечная станция маршрута
        /// </summary>
        public Station To => Last.Station;

        /// <summary>
        /// Полная продолжительность маршрута
        /// </summary>
        public int Timespan { get; protected set; }

        /// <summary>
        /// Количество станций, через которые проходит маршрут
        /// </summary>
        public int Length { get; private set; }

        public Route(Station from, Station to, int timespan)
        {
            Last = new RouteNode { Station = to, Timespan = 0, Next = null };
            First = new RouteNode { Station = from, Timespan = timespan, Next = Last };

            OnAddStation(from, timespan);
            OnAddStation(to, 0);
        }

        public Route(StationRelation relation)
            :this(relation.From, relation.To, relation.Timespan)
        {

        }

        protected virtual void OnAddStation(Station station, int timespan)
        {
            Timespan += timespan;
            ++Length;

            if (!Lines.Contains(station.Line))
                Lines.Add(station.Line);
        }

        public RouteNode AddFirst(Station station, int timespan)
        {
            if (timespan < 0)
                throw new ArgumentOutOfRangeException(nameof(timespan));

            var result = new RouteNode
            {
                Station = station,
                Timespan = timespan,
                Next = First
            };

            First = result;

            OnAddStation(station, timespan);

            return result;
        }

        public RouteNode AddLast(Station station, int timespan)
        {
            if (timespan < 0)
                throw new ArgumentOutOfRangeException(nameof(timespan));

            var result = new RouteNode
            {
                Station = station,
                Timespan = 0,
                Next = null
            };

            Last.Timespan = timespan;
            Last.Next = result;
            Last = result;

            OnAddStation(station, timespan);
            
            return result;
        }

        public RouteNode Add(StationRelation relation)
        {
            RouteNode result;
            if (relation.HasStation(From))
            {
                result = AddFirst(relation.GetOtherStation(From), relation.Timespan);
            }
            else if (relation.HasStation(To))
            {
                result = AddLast(relation.GetOtherStation(To), relation.Timespan);
            }
            else
            {
                throw new ArgumentException();
            }

            OnAddStation(result.Station, relation.Timespan);

            return result;
        }

        public IEnumerable<Line> GetLines() => Lines;

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

        #region IEnumerable

        public IEnumerator<Station> GetEnumerator()
        {
            return new Enumerator(First);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ICloneable

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

        #endregion

        /// <summary>
        /// Элемент маршрута
        /// </summary>
        public class RouteNode
        {
            /// <summary>
            /// Текущая станция
            /// </summary>
            public Station Station { get; set; }
            /// <summary>
            /// Времени до следующей станции
            /// </summary>
            public int Timespan { get; set; }
            /// <summary>
            /// Следующий элемент маршрута
            /// </summary>
            public RouteNode Next { get; set; }
        }

        private class Enumerator : IEnumerator<Station>
        {
            private readonly RouteNode _start;
            private RouteNode _current;
            public Station Current => _current.Station;

            object IEnumerator.Current => Current;

            public Enumerator(RouteNode node)
            {
                if (node == null)
                    throw new ArgumentNullException(nameof(node));

                _start = new RouteNode
                {
                    Station = null,
                    Timespan = 0,
                    Next = node
                };
                _current = _start;
            }

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {
                if (_current.Next != null)
                {
                    _current = _current.Next;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _current = _start;
            }
        }
    }
}
