using System.Diagnostics.Contracts;
using System.IO;

namespace Gandhi.Framework
{
	[ContractClassFor(typeof(INodeSerializer))]
	public abstract class NodeSerializerContracts : INodeSerializer
	{
		public void Serialize(string rootPath, INode node, bool recursive)
		{
			Contract.Requires(Path.IsPathRooted(rootPath));
			Contract.Requires(node != null);
		}

		public INode Deserialize(string rootPath)
		{
			Contract.Ensures(Contract.Result<INode>() != null);
			return null;
		}
	}
}