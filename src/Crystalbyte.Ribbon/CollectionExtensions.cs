using System.Collections.Generic;
using System.Windows.Controls;

namespace Crystalbyte {
    internal static class CollectionExtensions {
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source) {
            foreach (var item in source) {
                target.Add(item);
            }
        }

        public static void AddRange<T>(this ItemCollection target, IEnumerable<T> source) {
            foreach (var item in source) {
                target.Add(item);
            }
        }
    }
}
