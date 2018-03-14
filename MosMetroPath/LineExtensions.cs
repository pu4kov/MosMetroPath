using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosMetroPath
{
    public static class LineExtensions
    {
        public static Station AddStation(this Line line, string station)
        {
            return line.Scheme.AddStation(line, station);
        }

        public static Station AddStation(this Line line, string station, out Station createdStation)
        {
            createdStation = line.Scheme.AddStation(line, station);
            return createdStation;
        }

        public static IEnumerable<Station> GetRelationStations(this Line line)
        {
            return line.Scheme.GetLineRelationStations(line);
        }
    }
}
