using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosMetroPath
{
    public partial class Route
    {
        /// <summary>
        /// Элемент маршрута
        /// </summary>
        private class RouteNode
        {
            /// <summary>
            /// Текущая станция
            /// </summary>
            public IRoute Route { get; set; }
            /*
            /// <summary>
            /// Времени до следующей станции
            /// </summary>
            public int Timespan { get; set; }
            */
            /// <summary>
            /// Если true - текущий маршрут нужно развернуть
            /// </summary>
            public bool IsReversed { get; set; }

            public Station From => (IsReversed) ? Route.To : Route.From;

            public Station To => (IsReversed) ? Route.From : Route.To;
            /// <summary>
            /// Следующий элемент маршрута
            /// </summary>
            public RouteNode Next { get; set; }
            /// <summary>
            /// Предыдущий элемент маршрута
            /// </summary>
            public RouteNode Prior { get; set; }
        }
    }
}
