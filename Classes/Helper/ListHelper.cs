using System;
using System.Collections.Generic;
using System.Text;

namespace WebServerDetector.Classes.Helper
{
    public static class ListHelper
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            HashSet<TResult> set = new HashSet<TResult>();

            foreach (var item in source)
            {
                var selectedValue = selector(item);

                if (set.Add(selectedValue))
                    yield return item;
            }
        }
    }
}
