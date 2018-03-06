using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosMetroPath
{
    public static class StationExtensions
    {
        public static Station RelationTo(this Station station, string otherName, int timespan)
        {
            var line = station.Line;
            var scheme = line.Scheme;
            var createdStation = scheme.AddStation(line, otherName);
            scheme.AddRelation(station, createdStation, timespan);
            return createdStation;
        }

        public static Station RelationTo(this Station station, string otherName, int timespan, out Station createdStation)
        {
            var line = station.Line;
            var scheme = line.Scheme;
            createdStation = scheme.AddStation(line, otherName);
            scheme.AddRelation(station, createdStation, timespan);
            return createdStation;
        }

        public static Station RelationTo(this Station station, Station other, int timespan)
        {
            station.Line.Scheme.AddRelation(station, other, timespan);
            return other;
        }
    }
}
