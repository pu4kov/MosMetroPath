using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosMetroPath
{
    public class RoutesTable
    {
        private RoutesCollection<Line> _routes = new RoutesCollection<Line>(n => new TwoItemsKey<Line>(n.From.Line, n.To.Line));
        private ISet<Line> _lines = new HashSet<Line>();

        public RoutesTable()
        {
            
        }

        public void Add(IRoute route)
        {
            _routes.Add(route);
            _lines.Add(route.From.Line);
            _lines.Add(route.To.Line);
        }

        private void GenerateTable()
        {
            
        }
    }
}
