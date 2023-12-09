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

        public static IEnumerable<TResult> ProcessAdjacent<TElem, TResult>(this IEnumerable<TElem> sequence, Func<TElem, TElem, TResult> selector)
        {
            IEnumerator<TElem> iterator = sequence.GetEnumerator();
            if (!iterator.MoveNext())
            {
                yield break;
            }
            TElem prev = iterator.Current;
            while (iterator.MoveNext())
            {
                TElem current = iterator.Current;
                yield return selector(prev, current);
                prev = current;
            }
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

        public static void AddOrSet<TKey, TVal>(this Dictionary<TKey, TVal> dict, TKey key, TVal val)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = val;
            }
            else
            {
                dict.Add(key, val);
            }
        }

        public static void AddToCount<T>(this Dictionary<T, int> dict, T key, int count)
        {
            dict.AccumulateForKey((a, b) => a + b, 0, key, count);
        }

        public static void AddToCount<T>(this Dictionary<T, long> dict, T key, long count)
        {
            dict.AccumulateForKey((a, b) => a + b, 0, key, count);
        }

        public static void AccumulateForKey<TKey, TVal>(this Dictionary<TKey, TVal> dict, Func<TVal, TVal, TVal> Combine, TVal initialVal, TKey key, TVal value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = Combine(dict[key], value);
            }
            else
            {
                dict.Add(key, Combine(initialVal, value));
            }
        }


    }

}
