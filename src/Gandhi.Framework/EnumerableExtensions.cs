using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Gandhi.Framework
{
	public static class EnumerableExtensions
	{
		[Pure]
		public static bool None<T>(this IEnumerable<T> items, Func<T, bool> predicate)
		{
			return !items.Any(predicate);
		}

		[Pure]
		public static bool None<T>(this IEnumerable<T> items)
		{
			return !items.Any();
		}

		[Pure]
		public static IEnumerable<T> Duplicates<T>(this IEnumerable<T> list)
		{
			var duplicates =
				from t1 in list.Select((t, i) => new { i, t })
				from t2 in list.Select((t, i) => new { i, t })
				where t1.t.Equals(t2.t) && t1.i != t2.i
				select t1.t;

			return duplicates.Distinct();
		}
	}
}