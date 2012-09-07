using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Ghandi.Love
{
	public class NodeBase : INode
	{
		private readonly ICollection<INode> _children = new Collection<INode>();

		public string Name { get; set; }
		public string Template { get; set; }
		public Uri Uri { get; set; }
		public INode Parent { get; set; }

		public ICollection<INode> Children
		{
			get { return _children; }
		}
	}
}