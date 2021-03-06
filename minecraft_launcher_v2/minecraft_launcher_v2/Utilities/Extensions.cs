﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace minecraft_launcher_v2.Utilities
{
    static class Extensions
    {
        public static IEnumerable<T> OrderByAlphaNumeric<T>(this IEnumerable<T> source, Func<T, string> selector)
        {
            int max = source.SelectMany(i => Regex.Matches(selector(i), @"\d+", RegexOptions.Compiled).Cast<Match>().Select(m => (int?)m.Value.Length)).Max() ?? 0;

            return source.OrderBy(i => Regex.Replace(selector(i), @"\d+", m => m.Value.PadLeft(max, '0'), RegexOptions.Compiled));
        }

    }
}
