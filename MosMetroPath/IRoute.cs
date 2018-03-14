using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MosMetroPath
{
    public interface IRoute: IEnumerable<Station>
    {
        Station From { get; }
        Station To { get; }
        int Timespan { get; }
        int Length { get; }
        bool HasStation(Station station);
        IEnumerable<Line> GetLines();
        IEnumerable<Station> GetStations(bool reverse);
        IEnumerable<IRoute> GetRoutes(bool reverse);
    }
}
