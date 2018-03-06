using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosMetroPath
{
    public partial class Route
    {
        private class StationEnumerator : IEnumerator<Station>
        {
            private Route _route;
            private RouteNode _currentRoute;
            private IEnumerator<Station> _currentStations;

            public Station Current => _currentStations.Current;
            object IEnumerator.Current => Current;

            public bool IsReverse { get; }

            public StationEnumerator(Route route, bool reverse)
            {
                _route = route;
                IsReverse = reverse;
                Reset();
            }

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {
                while (!_currentStations.MoveNext())
                {
                    if (IsReverse)
                    {
                        if (_currentRoute.Prior == null)
                            return false;
                        else
                            _currentRoute = _currentRoute.Prior;
                    }
                    else
                    {
                        if (_currentRoute.Next == null)
                            return false;
                        else
                            _currentRoute = _currentRoute.Next;
                    }

                    _currentStations = _currentRoute.Route.GetStations(IsReverse).GetEnumerator();
                }

                return true;
            }

            public void Reset()
            {
                if (IsReverse)
                {
                    _currentRoute = _route.Last;
                }
                else
                {
                    _currentRoute = _route.First;
                }
                _currentStations = _currentRoute.Route.GetStations(IsReverse).GetEnumerator();
            }
        }
    }
}
