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
        private List<RouteMatrix> Completed { get; } = new List<RouteMatrix>();
        /// <summary>
        /// "Листья" дерева поиска
        /// </summary>
        private LinkedList<RouteBuilderNode> Leaves { get; } = new LinkedList<RouteBuilderNode>();
        /// <summary>
        /// Минимально возможная длина маршрута
        /// </summary>
        public int MinLength { get; private set; }
        public bool IsComplete { get; private set; }
        public int RoutesCount => Completed.Count;
        public int RoutesTimespan => (Completed.Count > 0) ? Completed[0].MinLength : int.MaxValue;

        public RouteBuilder(IEnumerable<IRoute> routes)
        {
            var _rootNode = new RouteBuilderNode(new RouteMatrix(routes));
            Leaves.AddFirst(_rootNode);
            MinLength = _rootNode.Matrix.MinLength;
        }

        private void AddCompleted(RouteBuilderNode node)
        {
            bool added = false;
            if (Completed.Count > 0
                && Completed[0].MinLength > node.Matrix.MinLength)
            {
                Completed.Clear();

                MinLength = node.Matrix.MinLength;

                added = true;
            }
            else if (Completed.Count == 0
                || Completed[0].MinLength == node.Matrix.MinLength)
            {
                added = true;
            }
            
            if (added)
            {
                Completed.Add(node.Matrix);

                var n = Leaves.Last;
                while (n != null)
                {
                    if (n.Value.Matrix.MinLength >= node.Matrix.MinLength)
                    {
                        var tmp = n.Previous;
                        Leaves.Remove(n);
                        n = tmp;
                    }
                    else
                    {
                        n = n.Previous;
                    }
                }
            }
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
                if (Leaves.First.Value.Matrix.MinLength >= node.Matrix.MinLength)
                {   // node - лучшая матрица, добавление в начало
                    Leaves.AddFirst(node);
                    if (MinLength > node.Matrix.MinLength)
                    {
                        MinLength = node.Matrix.MinLength;
                    }
                }
                else if (Leaves.Last.Value.Matrix.MinLength <= node.Matrix.MinLength)
                {   // node - худшая матрица, добавление в конец
                    Leaves.AddLast(node);
                }
                else
                { // добавление в середину
                    var n = Leaves.First;
                    while (n != null
                        && n.Value.Matrix.MinLength < node.Matrix.MinLength)
                    {
                        n = n.Next;
                    }
                    Leaves.AddBefore(n, node);
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
                    AddCompleted(node);
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
            bool next = true;
            do
            {
                var node = Pop();
                
                if (node.NextTurn())
                {
                    Add(node.ExcludeEdgeNode);
                    Add(node.IncludeEdgeNode);
                }
                else if (node.Matrix.State == RouteMatrixState.IsComplete)
                {
                    AddCompleted(node);
                    
                    next = false;
                }
                if (Leaves.Count == 0)
                    IsComplete = true;
            }
            while (!IsComplete && next);

            return !IsComplete || !next;
        }
        
        public IEnumerable<IRoute> GetRoutes()
        {
            var result = new List<IRoute>(Completed.Count);
            
            foreach (var r in Completed)
                result.Add(BuildRoute(r.PassedRoutes));

            return result;
        }

        [DebuggerDisplay("MinLength = {Key}, State = {Matrix.State}")]
        private class RouteBuilderNode
        {
            public int Key => Matrix.MinLength;
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
                        && ExcludeEdgeNode.Matrix.MinLength >= IncludeEdgeNode.Matrix.MinLength)
                        ExcludeEdgeNode = null;

                    return true;
                }

                return false;
            }
        }
    }
}
