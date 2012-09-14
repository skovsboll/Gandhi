using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Gandhi.Framework
{
	public static class NodeExtensions
	{
		[Pure]
		public static IEnumerable<INode> Descendants(this INode node)
		{
			return node.Children.Union(node.Children.SelectMany(Descendants));
		}

		[Pure]
		public static IEnumerable<INode> Ancestors(this INode node)
		{
			if (node.Parent == null) return new INode[0];
			return new[] { node.Parent }.Union(node.Parent.Ancestors());
		}
	}
}