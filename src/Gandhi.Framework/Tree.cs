using System.Diagnostics.Contracts;
using System.Linq;

namespace Gandhi.Framework
{
	public class Tree
	{
		public readonly INode Root;

		public Tree(INode root)
		{
			Root = root;
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			Contract.Invariant(Root.Descendants().Select(d => d.Uri).Duplicates().None());

		}
	}
}