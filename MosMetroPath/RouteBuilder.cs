using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MosMetroPath
{
    /// <summary>
    /// Компоновщик маршрута по методу ветвей и границ
    /// </summary>
    public class RouteBuilder
    {
        private RouteMatrix _completed;
        private RouteMatrix Completed
        {
            get
            {
                return _completed;
            }
            set
            {
                if (value.State == RouteMatrixState.IsComplete)
                {
                    if (Completed == null
                        || Completed.MinTimespan > value.MinTimespan)
                    {
                        _completed = value;
                        Leaves.Clear();
                    }
                }
            }
        }
        /// <summary>
        /// "Листья" дерева поиска
        /// </summary>
        private LinkedList<RouteBuilderNode> Leaves { get; } = new LinkedList<RouteBuilderNode>();
        /// <summary>
        /// Минимально возможная длина маршрута
        /// </summary>
        public int MinTimespan
        {
            get
            {
                if (Completed != null)
                {
                    return Completed.MinTimespan;
                }
                else if (Leaves.First != null)
                {
                    return Leaves.First.Value.Matrix.MinTimespan;
                }
                else
                {
                    return int.MaxValue;
                }
            }
        }
        public bool IsComplete { get; private set; }
        public bool HasResult => Completed != null;
        public int RoutesTimespan => (Completed != null) ? Completed.MinTimespan : int.MaxValue;

        public RouteBuilder(IEnumerable<IRoute> routes)
        {
            var _rootNode = new RouteBuilderNode(new RouteMatrix(routes));
            Leaves.AddFirst(_rootNode);
        }

        private void AddLeave(RouteBuilderNode node)
        {
            if (node == null
                || node.Matrix.State == RouteMatrixState.Unreachable)
                return;

            if (Leaves.First == null)
            {
                Leaves.AddFirst(node);
            }
            else
            {
                var compareFirst = node.CompareTo(Leaves.First.Value);
                if (compareFirst <= 0)
                {
                    Leaves.AddFirst(node);
                }
                else
                {
                    var compareLast = node.CompareTo(Leaves.Last.Value);
                    if (compareLast > 0)
                    {
                        Leaves.AddLast(node);
                    }
                    else
                    {
                        var n = Leaves.First.Next;
                        while (node.CompareTo(n.Value) > 0)
                            n = n.Next;
                        Leaves.AddBefore(n, node);
                    }
                }
            }
        }

        private void Add(RouteBuilderNode node)
        {
            if (node == null)
                return;
            switch (node.Matrix.State)
            {
                case RouteMatrixState.IsComplete:
                    Completed = node.Matrix;
                    IsComplete = true;
                    break;
                case RouteMatrixState.Process:
                    AddLeave(node);
                    break;
            }
        }

        private IRoute BuildRoute(IEnumerable<IRoute> routes)
        {
            LinkedList<IRoute> lroutes = new LinkedList<IRoute>(routes);
            var firstNode = lroutes.First;
            lroutes.Remove(firstNode);
            var result = new Route(firstNode.Value);
            var r = lroutes.First;
            while (r.Value.To != firstNode.Value.From)
                r = r.Next;
            var last = r.Value;
            lroutes.Remove(r);

            r = lroutes.First;
            while (r != null)
            {
                if (r.Value.From == result.To)
                {
                    result.AddLast(r.Value);
                    lroutes.Remove(r);
                    r = lroutes.First;
                }
                else
                {
                    r = r.Next;
                }
            }

            result.AddLast(last);
            
            if (lroutes.First != null)
                throw new Exception("Ошибка компоновки маршрута");
                
            return result;
        }

        private RouteBuilderNode Pop()
        {
            var result = Leaves.First;
            Leaves.Remove(result);

            return result.Value;
        }
        
        public bool NextTurn()
        {
            if (IsComplete)
                return false;

            var node = Pop();

            var result = node.NextTurn();

            if (result)
            {
                Add(node.ExcludeEdgeNode);
                Add(node.IncludeEdgeNode);
            }
            else if (node.Matrix.State == RouteMatrixState.IsComplete)
            {
                Completed = node.Matrix;
                IsComplete = true;
            }

            return result;
        }
        
        public IRoute GetRoute()
        {
            if (Completed == null)
                throw new Exception();

            return BuildRoute(Completed.PassedRoutes);
        }

        [DebuggerDisplay("MinTimespan = {Matrix.MinTimespan}, State = {Matrix.State}, {Matrix.ColumnsCountx{Matrix.RowsCount}")]
        private class RouteBuilderNode: IComparable<RouteBuilderNode>
        {
            public RouteMatrix Matrix { get; }
            public RouteBuilderNode IncludeEdgeNode { get; private set; }
            public RouteBuilderNode ExcludeEdgeNode { get; private set; }

            public RouteBuilderNode(RouteMatrix matrix)
            {
                Matrix = matrix;
            }

            public bool NextTurn()
            {
                if (Matrix.State == RouteMatrixState.Process)
                {
                    // Определение рёбер ветвления
                    var e = Matrix.GetBranchEdges().GetEnumerator();
                    RouteMatrix.MatrixCoord edge;
                    if (e.MoveNext())
                    {
                        edge = e.Current;   // Берём первое попавшееся
                    }
                    else
                    {
                        // Не найдено ребро ветвления
                        return false;
                    }

                    // Включение ребра
                    IncludeEdgeNode = new RouteBuilderNode(Matrix.IncludeEdge(edge));

                    // Исключение ребра ветвления из матрицы
                    ExcludeEdgeNode = new RouteBuilderNode(Matrix.ExcludeEdge(edge));
                    
                    if (IncludeEdgeNode.Matrix.State == RouteMatrixState.IsComplete
                        && ExcludeEdgeNode.Matrix.MinTimespan >= IncludeEdgeNode.Matrix.MinTimespan)
                        ExcludeEdgeNode = null;

                    return true;
                }

                return false;
            }

            public int CompareTo(RouteBuilderNode other)
            {
                int result = Matrix.MinTimespan - other.Matrix.MinTimespan;
                if (result == 0)
                {
                    result = (Matrix.ColumnsCount * Matrix.RowsCount) - (other.Matrix.ColumnsCount * other.Matrix.RowsCount);
                }
                return result;
            }
        }
    }
}
