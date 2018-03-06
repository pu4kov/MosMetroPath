using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosMetroPath
{
    public static class CompositeRouteExtensions
    {
        public static CompositeRoute Union(this CompositeRoute composite, IRoute route)
        {
            var result = new CompositeRoute(composite);
            result.UnionWith(route);

            return result;
        }
    }
}
