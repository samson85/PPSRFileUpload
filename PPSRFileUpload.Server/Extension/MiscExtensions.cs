using System.Collections.Generic;

namespace System.Collections
{
    public static class MiscExtensions
    {
        public static void Each<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list) {
                action(item);
            }
        }


    }
}
