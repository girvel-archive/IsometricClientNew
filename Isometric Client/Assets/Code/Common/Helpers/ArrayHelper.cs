using System;

namespace Assets.Code.Common.Helpers
{
    public static class ArrayHelper
    {
        public static TResult[,] TwoDimSelect<TSource, TResult>(this TSource[,] array, Func<TSource, TResult> selector)
        {
            var result = new TResult[array.GetLength(0), array.GetLength(1)];

            for (var x = 0; x < array.GetLength(0); x++)
            {
                for (var y = 0; y < array.GetLength(1); y++)
                {
                    result[x, y] = selector(array[x, y]);
                }
            }

            return result;
        }
    }
}