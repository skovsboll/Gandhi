using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Ghandi.Love;

namespace Ghandi.Areas.gandi.Love
{
	public static class NodeExtensions
	{
		public static IEnumerable<INode> Descendants(this INode node)
		{
			return node.Children.Union(node.Children.SelectMany(c => c.Descendants()));
		}

		public static IEnumerable<INode> Ancestors(this INode node)
		{
			if (node.Parent == null) return new INode[0];
			return new[] {node.Parent}.Union(node.Parent.Ancestors());
		}
	}

	public  static class EnumerableExtensions
	{
		public static bool None<T>(this IEnumerable<T> items, Func<T, bool> predicate)
		{
			return !items.Any(predicate);
		}

		public static bool None<T>(this IEnumerable<T> items)
		{
			return !items.Any();
		}  
		public static IEnumerable<T> Duplicates<T>(this IEnumerable<T> list)
		{
			var x = from t1 in list.Select((t, i) => new {i, t})
			        from t2 in list.Select((t, i) => new {i, t})
			        where t1.t.Equals(t2.t) && t1.i != t2.i
			        select t1.t;
		}

	}

	public class Tree
	{
		private readonly INode _root;

		public Tree(INode root)
		{
			_root = root;
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			Contract.Invariant(_root.Descendants().Select(d => d.Uri).Duplicates().None());	

		}
	}
}