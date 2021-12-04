using System.Linq;
using System.Collections.Generic;
using System;

namespace Common
{
    public static class Extensions
    {
        public static IEnumerable<List<TElem>> SplitWhen<TElem>(this IEnumerable<TElem> seq,
            Func<TElem, TElem, bool> splitPredicate)
        {
            List<TElem> prefix = new List<TElem>();
            foreach (TElem next in seq)
            {
                if (prefix.Any())
                {
                    if (splitPredicate(prefix.Last(), next))
                    {
                        yield return prefix;
                        prefix = new List<TElem>();
                    }
                }
                prefix.Add(next);
            }
            if (prefix.Any())
            {
                yield return prefix;
            }
            yield break;
        }

        public static IEnumerable<List<TElem>> SplitBy<TElem>(this IEnumerable<TElem> seq,
            Func<TElem, bool> separatorPredicate)
        {
            List<TElem> prefix = new List<TElem>();
            foreach (TElem next in seq)
            {
                if (separatorPredicate(next))
                {
                    if (prefix.Any())
                    {
                        yield return prefix;
                        prefix = new List<TElem>();
                    }
                }
                else
                {
                    prefix.Add(next);
                }
            }
            if (prefix.Any())
            {
                yield return prefix;
            }
            yield break;
        }

        public static IEnumerable<List<string>> Paragraphs(this IEnumerable<string> lines)
        {
            return lines.SplitBy(s => s.Trim().Length == 0);
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
        {
            return keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public static TVal GetOrElse<TKey, TVal>(this Dictionary<TKey, TVal> dict, TKey key, TVal ifNotFound)
        {
            if (dict.TryGetValue(key, out TVal result))
            {
                return result;
            }
            else
            {
                return ifNotFound;
            }
        }

        public static IEnumerable<KeyValuePair<int, T>> ZipWithIndex<T>(this IEnumerable<T> src)
        {
            int i = 0;
            foreach (T e in src)
            {
                yield return KeyValuePair.Create(i++, e);
            }
            yield break;
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> src)
        {
            return src.SelectMany(part => part);
        }

    }
}
