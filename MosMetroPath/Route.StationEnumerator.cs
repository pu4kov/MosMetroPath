using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosMetroPath
{
    public partial class Route
    {
        private class StationEnumerator: Enumerator<Station>
        {
            public StationEnumerator(RouteNode start, bool reverse)
                :base(start, reverse, (node) =>
                {/*
                    if (reverse)
                        return node.Route.GetStations(node.IsReversed ^ reverse).Reverse().GetEnumerator();*/
                    return node.Route.GetStations(node.IsReversed ^ reverse).GetEnumerator();
                })
            {

            }

            protected override void OnMoveNextNode()
            {
                base.OnMoveNextNode();

                CurrentEnumerator.MoveNext();   // Пропуск первой станции, т.к. она уже была в конце предыдущего маршрута
            }
        }
    }
}
