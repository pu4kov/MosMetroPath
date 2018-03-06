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
    /// Связь между двумя станциями (ж/д путь, переход)
    /// </summary>
    [DebuggerDisplay("{From?.Name} -> {To?.Name}: {Timespan}")]
    public class StationRelation: IRoute
    {
        private Station[] _stations;
        public Station From => _stations[0];
        public Station To => _stations[1];
        public int Timespan { get; protected set; }
        public int Length => 2;

        public StationRelation(Station from, Station to, int timespan)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }
            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }
            _stations = new Station[] { from, to };
            Timespan = timespan;
        }

        public Station GetOtherStation(Station station)
        {
            if (station == From)
            {
                return To;
            }
            else if (station == To)
            {
                return From;
            }

            throw new ArgumentOutOfRangeException(nameof(station));
        }

        public override int GetHashCode()
        {
            return From.Id + To.Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is StationRelation other)
            {
                return (
                    (From == other.From && To == other.To)
                    || (To == other.From && From == other.To));
            }

            return base.Equals(obj);
        }

        public IEnumerable<Line> GetLines()
        {
            yield return From.Line;

            if (To.Line != From.Line)
                yield return To.Line;
        }

        public IEnumerable<Station> GetStations(bool reverse = false)
        {
            return (reverse) ? _stations.Reverse() : _stations;
        }

        public IEnumerable<IRoute> GetRoutes(bool reverse = false)
        {
            return Enumerable.Empty<IRoute>();
        }
    }
}
