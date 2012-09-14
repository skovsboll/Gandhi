using System;
using System.Collections.Generic;

namespace Ghandi.Love
{
	public interface INode
	{
		string Name { get; set; }
		INode Parent { get; set; }
		string Template { get; set; }
		Uri Uri { get; set; }
		ICollection<INode> Children { get; }
	}
}