using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosMetroPath
{
    public interface IRoute: IEnumerable<Station>
    {
        Station From { get; }
        Station To { get; }
        int Timespan { get; }
        int Length { get; }
        IEnumerable<Line> GetLines();
    }
}
