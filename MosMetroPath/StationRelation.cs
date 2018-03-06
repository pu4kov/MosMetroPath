using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;

namespace MosMetroPath
{
    [DebuggerDisplay("{From?.Name} -> {To?.Name}: {Timespan}")]
    public class StationRelation: IRoute
    {
        public virtual Station From { get; }
        public virtual Station To { get; }
        public int Timespan { get; protected set; }

        public int Length => 2;

        public StationRelation(Station from, Station to, int timespan = 0)
        {
            From = from ?? throw new ArgumentNullException(nameof(from));
            To = to ?? throw new ArgumentNullException(nameof(to));
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

        public bool HasStation(Station station)
        {
            return From == station
                || To == station;
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

        public IEnumerator<Station> GetEnumerator()
        {
            return (new List<Station> { From, To }).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
