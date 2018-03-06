using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MosMetroPath
{
    /// <summary>
    /// Составной маршрут
    /// </summary>
    [DebuggerDisplay("{Length} станций, {Timespan} секунд")]
    public class CompositeRoute: IRoute, IEnumerable<Station>, ICloneable
    {
        private CompositeRouteNode _firstNode;
        private CompositeRouteNode _lastNode;

        public Station From
        {
            get
            {
                return _firstNode.From;
            }
        }

        public Station To
        {
            get
            {
                return _lastNode.To;
            }
        }

        public int Timespan
        {
            get
            {
                int result = 0;

                for (var current = _firstNode; current != null; current = current.Next)
                {
                    result += current.Route.Timespan;
                    
                }

                return result;
            }
        }

        public int Length
        {
            get
            {
                int result = 0;

                for (var current = _firstNode; current != null; current = current.Next)
                {
                    result += current.Route.Length;
                }

                return result;
            }
        }

        public IEnumerable<Line> GetLines()
        {
            HashSet<Line> result = new HashSet<Line>();

            for (var current = _firstNode; current != null; current = current.Next)
            {
                result.UnionWith(current.Route.GetLines());
                
            }

            return result;
        }

        public CompositeRoute(IRoute route)
        {
            _firstNode = new CompositeRouteNode(route);
            _lastNode = _firstNode;
        }

        public CompositeRoute(CompositeRoute route)
        {
            _firstNode = new CompositeRouteNode(route._firstNode);
            var current = _firstNode;
            while (current.Next != null)
                current = current.Next;
            _lastNode = current;
        }

        public void AddFirst(IRoute route)
        {
            bool reverse;
            if (From == route.To)
                reverse = false;
            else if (From == route.From)
                reverse = true;
            else
            {
                throw new ArgumentException("Ошибка добавления маршрута. Начало имеющегося маршрута не является началом/концом добавляемого");
            }

            var newNode = new CompositeRouteNode(route, reverse)
            {
                Next = _firstNode
            };
            _firstNode = newNode;
        }

        public void AddLast(IRoute route)
        {
            bool reverse;
            if (To == route.From)
                reverse = false;
            else if (To == route.To)
                reverse = true;
            else
            {
                throw new ArgumentException("Ошибка добавления маршрута. Конец имеющегося маршрута не является началом/концом добавляемого");
            }
            var newNode = new CompositeRouteNode(route, reverse);
            _lastNode.Next = newNode;
            _lastNode = newNode;
        }

        public void UnionWith(IRoute route)
        {
            bool retry;

            // Сначала пробуем добавить в конец
            try
            {
                AddLast(route);
                retry = false;
            }
            catch
            {
                retry = true;
            }

            if (retry)
            {
                // Добавляем в начало
                try
                {
                    AddFirst(route);
                    retry = false;
                }
                catch
                {
                    retry = true;
                }
            }

            if (retry)
                throw new ArgumentException("Ошибка добавления маршрута. Маршрут не может быть объединен с имеющимся");
        }

        public IEnumerator<Station> GetEnumerator()
        {
            return new Enumerator(_firstNode);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public object Clone()
        {
            return new CompositeRoute(this);
        }

        private class CompositeRouteNode
        {
            public IRoute Route { get; }

            public bool IsReverse { get; }

            public CompositeRouteNode Next { get; set; }

            public CompositeRouteNode(IRoute route, bool isReverse = false)
            {
                Route = route;
                IsReverse = isReverse;
                Next = null;
            }

            public CompositeRouteNode(CompositeRouteNode node)
            {
                Route = node.Route;
                IsReverse = node.IsReverse;
                if (node.Next == null)
                    Next = null;
                else
                    Next = new CompositeRouteNode(node.Next);
            }

            public virtual Station From => (IsReverse) ? Route.To : Route.From;
            public virtual Station To => (IsReverse) ? Route.From : Route.To;
        }

        private class Enumerator : IEnumerator<Station>
        {
            private CompositeRouteNode _start;
            private CompositeRouteNode _currentNode;
            private IEnumerator<Station> _currentRoute;

            public Station Current => _currentRoute.Current;

            object IEnumerator.Current => Current;

            public Enumerator(CompositeRouteNode node)
            {
                _start = node;
                _currentNode = node;
                _currentRoute = node.Route.GetEnumerator();
            }

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {
                while (!_currentRoute.MoveNext())
                {
                    if (_currentNode.Next == null)
                        return false;

                    _currentNode = _currentNode.Next;

                    if (_currentNode.IsReverse)
                        _currentRoute = _currentNode.Route.Reverse().GetEnumerator();
                    else
                        _currentRoute = _currentNode.Route.GetEnumerator();
                }

                return true;
            }

            public void Reset()
            {
                _currentNode = _start;
                _currentRoute = null;
            }
        }
    }
}
