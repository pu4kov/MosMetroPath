using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MosMetroPath
{
    /// <summary>
    /// Состояние матрицы
    /// </summary>
    public enum RouteMatrixState
    {
        Process,    // Промежуточная матрица (маршрут в процессе поиска)
        IsComplete, // Финальная матрица (маршрут найден)
        Unreachable // Маршрут недостижим
    }

    [DebuggerDisplay("MinTimespan = {MinTimespan} [{ColumnsCount}x{RowsCount}]")]
    public partial class RouteMatrix
    {
        private MatrixValue[,] Items { get; }
        private MatrixHeaders Headers { get; }

        private List<IRoute> _passedRoutes = new List<IRoute>();
        public IEnumerable<IRoute> PassedRoutes => _passedRoutes;
        private int BaseMinTimespan { get; set; }
        private int CurrentMinTimespan { get; set; }
        /// <summary>
        /// Минимально возможная длина маршрута (Нижняя граница)
        /// </summary>
        public int MinTimespan  => BaseMinTimespan + CurrentMinTimespan;
        public RouteMatrixState State { get; private set; } = RouteMatrixState.Process;

        public int ColumnsCount => Headers.ColumnsCount;
        public int RowsCount => Headers.RowsCount;

        public RouteMatrix(IEnumerable<IRoute> routes)
        {
            State = RouteMatrixState.Process;
            Headers = new MatrixHeaders(routes);
            if (Headers.GetLines().Count() <= 2)
                throw new ArgumentOutOfRangeException(nameof(routes));

            Items = new MatrixValue[Headers.ColumnsCount + 1, Headers.RowsCount + 1];
            foreach (var r in routes)
            {
                var from = r.From.Line;
                var to = r.To.Line;
                var coord = Headers.GetCoord(from, to);
                Items[coord.Col, coord.Row] = new MatrixValue(r);
                coord = Headers.GetCoord(to, from);
                Items[coord.Col, coord.Row] = new MatrixValue(r);
            }

            // Добавление бесконечности в пути, соединяющие одну и ту же ветку
            foreach (var line in Headers.GetLines())
            {
                if (Headers.TryGetCoord(line, line, out var coord))
                {
                    Items[coord.Col, coord.Row] = MatrixValue.Infinity;
                }
                else
                {
                    throw new ArgumentException("Набор маршрутов покрывает не все возможные варианты движения");
                }
            }

            Reduction();
        }

        public RouteMatrix(RouteMatrix matrix, MatrixCoord excluded)
        {
            BaseMinTimespan = matrix.MinTimespan;
            _passedRoutes.AddRange(matrix.PassedRoutes);
            State = RouteMatrixState.Process;
            Headers = matrix.Headers;
            Items = new MatrixValue[Headers.ColumnsCount + 1, Headers.RowsCount + 1];
            for (int col = 0; col < ColumnsCount; ++col)
            {
                for (int row = 0; row < RowsCount; ++row)
                {
                    Items[col, row] = matrix.Items[col, row];
                }
            }

            Items[excluded.Col, excluded.Row].SetInfinity();
        }

        private RouteMatrix(MatrixHeaders headers, ICollection<IRoute> passedRoutes, int baseMinTimespan, RouteMatrixState state = RouteMatrixState.Process)
        {   
            if (baseMinTimespan <= 0)
                throw new ArgumentOutOfRangeException(nameof(baseMinTimespan));
            if (passedRoutes != null)
                _passedRoutes.AddRange(passedRoutes);
            BaseMinTimespan = baseMinTimespan;
            State = state;
            if (State == RouteMatrixState.IsComplete)
            {
                var len = 0;
                foreach (var r in PassedRoutes)
                    len += r.Timespan;
                CurrentMinTimespan = len - BaseMinTimespan;
            }
            else if (State == RouteMatrixState.Process)
            {
                Headers = headers;
                Items = new MatrixValue[Headers.ColumnsCount + 1, Headers.RowsCount + 1];
            }
            else if (State == RouteMatrixState.Unreachable)
            {

            }
        }

        public IEnumerable<Line> GetLines() => Headers.GetLines();


        private void FillMinValuesByCols()
        {
            for (int col = 0; col < ColumnsCount; ++col)
            {
                Items[col, RowsCount] = Items[col, 0];
                for (int row = 1; row < RowsCount; ++row)
                    Items[col, RowsCount] = MatrixValue.Min(Items[col, row], Items[col, RowsCount]);

                if (Items[col, RowsCount].IsInfinity)
                {
                    Items[col, RowsCount].IsInfinity = false;
                    Items[col, RowsCount].Value = 0;
                }
            }
        }

        private void FillMinValuesByRows()
        {
            for (int row = 0; row < RowsCount; ++row)
            {
                Items[ColumnsCount, row] = Items[0, row];
                for (int col = 1; col < ColumnsCount; ++col)
                    Items[ColumnsCount, row] = MatrixValue.Min(Items[col, row], Items[ColumnsCount, row]);

                if (Items[ColumnsCount, row].IsInfinity)
                {
                    Items[ColumnsCount, row].IsInfinity = false;
                    Items[ColumnsCount, row].Value = 0;
                }
            }
        }

        /// <summary>
        /// Вычесть из значений минимальное число в колонке
        /// </summary>
        private void SubtractMinValuesByCols()
        {
            FillMinValuesByCols();
            for (int col = 0; col < ColumnsCount; ++col)
            {
                if (Items[col, RowsCount].Value > 0)
                {
                    for (int row = 0; row < RowsCount; ++row)
                        Items[col, row].Subtract(Items[col, RowsCount]);
                }
            }
        }

        /// <summary>
        /// Вычесть из значений минимальное число в строке
        /// </summary>
        private void SubtractMinValuesByRows()
        {
            FillMinValuesByRows();

            for (int row = 0; row < RowsCount; ++row)
            {
                if (Items[ColumnsCount, row].Value > 0)
                {
                    for (int col = 0; col < ColumnsCount; ++col)
                        Items[col, row].Subtract(Items[ColumnsCount, row]);
                }
            }
        }

        
        /// <summary>
        /// Подсчёт суммы констант для элемента [col, row], выставленного в Infinity
        /// </summary>
        /// <param name="col">Столбец, в котором находится элемент матрицы</param>
        /// <param name="row">Строка, в которой находится элемент матрицы</param>
        /// <returns>Сумма констант</returns>
        private int CalcZeroConstSum(int col, int row)
        {
            var minCol = Items[0, row];
            
            for (int i = 1; i < ColumnsCount; ++i)
                if (i != col)
                    minCol = MatrixValue.Min(Items[i, row], minCol);
            
            var minRow = Items[col, 0];
            for (int j = 1; j < RowsCount; ++j)
                if (j != row)
                    minRow = MatrixValue.Min(Items[col, j], minRow);

            Items[col, RowsCount].Value = (minRow.IsInfinity) ? 0 : minRow.Value;
            Items[ColumnsCount, row].Value = (minCol.IsInfinity) ? 0 : minCol.Value;

            return Items[col, RowsCount].Value + Items[ColumnsCount, row].Value;
        }

        /// <summary>
        /// Исключение ребра
        /// </summary>
        /// <param name="coord">Координаты ребра</param>
        /// <returns>Новая матрица, с исключенным ребром [coord.Col, coord.Row]</returns>
        public RouteMatrix ExcludeEdge(MatrixCoord coord)
        {
            var result = new RouteMatrix(this, coord);
            result.Reduction();

            return result;
        }

        /// <summary>
        /// Включение ребра
        /// </summary>
        /// <param name="coord">Координаты ребра</param>
        /// <returns>Новая матрица, с включенным ребром [coord.Col, coord.Row]</returns>
        public RouteMatrix IncludeEdge(MatrixCoord coord)
        {
            var result = CopyWithoutColAndRow(coord);

            return result;
        }

        /// <summary>
        /// Определение рёбер ветвления
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MatrixCoord> GetBranchEdges()
        {
            int max = -1;
            var result = new List<MatrixCoord>();

            Reduction();

            if (State == RouteMatrixState.Process)
            {
                for (int col = 0; col < ColumnsCount; ++col)
                {
                    for (int row = 0; row < RowsCount; ++row)
                    {
                        if (!Items[col, row].IsInfinity
                            && Items[col, row].Value == 0)
                        {
                            var n = CalcZeroConstSum(col, row);

                            if (max < n)
                            {
                                max = n;
                                result.Clear();
                                result.Add(new MatrixCoord(col, row));
                            }
                            else if (max == n)
                            {
                                result.Add(new MatrixCoord(col, row));
                            }
                        }
                    }
                }
            }            

            return result;
        }

        /// <summary>
        /// Оценка нижней границы
        /// </summary>
        private void CalcCurrentMinTimespan()
        {
            //CurrentMinLength = 0;
            for (int col = 0; col < ColumnsCount; ++col)
                CurrentMinTimespan += Items[col, RowsCount].Value;
            for (int row = 0; row < RowsCount; ++row)
                CurrentMinTimespan += Items[ColumnsCount, row].Value;
        }

        /// <summary>
        /// Редукция матрицы
        /// </summary>
        private void Reduction()
        {
            SubtractMinValuesByRows();
            SubtractMinValuesByCols();

            CalcCurrentMinTimespan();

            if (BaseMinTimespan == 0)
            {
                BaseMinTimespan = CurrentMinTimespan;
                CurrentMinTimespan = 0;
            }
        }

        private IRoute GetRoute(MatrixCoord coord)
        {
            return GetRoute(coord.Col, coord.Row);
        }

        private IRoute GetRoute(int col, int row)
        {
            var toLine = Headers.GetLineByCol(col);
            var fromLine = Headers.GetLineByRow(row);
            if (toLine == Items[col, row].Route.To.Line
                && fromLine == Items[col, row].Route.From.Line)
            {
                return Items[col, row].Route;
            }
            else if (fromLine == Items[col, row].Route.To.Line
                && toLine == Items[col, row].Route.From.Line)
            {
                return new Route(Items[col, row].Route, true);
            }

            throw new Exception();
        }

        private bool CanAddPassedRoute(IEnumerable<IRoute> routes, IRoute addedRoute)
        {
            bool from = false;
            bool to = false;
            foreach (var r in routes)
            {
                if (r.From == addedRoute.To)
                    to = true;
                else if (r.To == addedRoute.From)
                    from = true;
            }

            return !from || !to;
        }

        /// <summary>
        /// Создать копию матрицы, удалив из неё столбец и строку
        /// </summary>
        /// <param name="coord">Удаляемые строка и столбец</param>
        /// <returns>Копия матрицы</returns>
        public RouteMatrix CopyWithoutColAndRow(MatrixCoord coord)
        {
            // Настройка заголовков строк и столбцов создаваемой матрицы
            MatrixHeaders newHeaders = null;
            RouteMatrixState newState = RouteMatrixState.Process;
            List<IRoute> routes = null;

            var addedRoute = GetRoute(coord);

            if (CanAddPassedRoute(PassedRoutes, addedRoute))    // Контроль за образованием цикла
            {
                routes = new List<IRoute>(PassedRoutes)
                {
                    addedRoute
                };
                newHeaders = Headers.CopyWithoutColRow(coord.Col, coord.Row);

                if (newHeaders.ColumnsCount == 1
                    && newHeaders.RowsCount == 1)
                {
                    var newLinesCount = newHeaders.GetLines().Count();
                    if (newLinesCount == 2)
                    {
                        var fromLine = newHeaders.GetLineByRow(0);
                        var toLine = newHeaders.GetLineByCol(0);
                        var curCoord = Headers.GetCoord(toLine, fromLine);
                        // Оставшийся последним маршрут достижим
                        if (!Items[curCoord.Col, curCoord.Row].IsInfinity)
                        {
                            var lastRoute = GetRoute(curCoord);
                            routes.Add(lastRoute);

                            newState = RouteMatrixState.IsComplete;
                        }
                        else
                        {
                            newState = RouteMatrixState.Unreachable;
                        }
                    }
                    else
                    {
                        newState = RouteMatrixState.Unreachable;
                    }
                    newHeaders = null;
                }
            }
            else
            {
                newState = RouteMatrixState.Unreachable;
            }
            

            RouteMatrix result = new RouteMatrix(newHeaders, routes, MinTimespan, newState);

            if (result.State == RouteMatrixState.Process)
            {
                for (int col = 0; col < ColumnsCount; ++col)
                {
                    if (col != coord.Col)
                    {
                        var newCol = result.Headers.GetColByLine(Headers.GetLineByCol(col));

                        for (int row = 0; row < RowsCount; ++row)
                        {
                            if (row != coord.Row)
                            {        
                                var newRow = result.Headers.GetRowByLine(Headers.GetLineByRow(row));
                                result.Items[newCol, newRow] = Items[col, row];
                            }
                        }
                    }
                }
                // Устанавливаем элемент [row, col] в бесконечность
                var fromLine = Headers.GetLineByRow(coord.Row);
                var toLine = Headers.GetLineByCol(coord.Col);
                var infCol = result.Headers.GetColByLine(fromLine);
                var infRow = result.Headers.GetRowByLine(toLine);
                if (infCol >= 0 && infRow >= 0)
                    result.Items[infCol, infRow].SetInfinity();

                result.FillMinValuesByRows();
                result.FillMinValuesByCols();
                result.CalcCurrentMinTimespan();
            }         

            return result;
        }
    }
}
