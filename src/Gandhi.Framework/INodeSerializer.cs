using System.Diagnostics.Contracts;
using System.IO;

namespace Ghandi.Love
{
	[ContractClass(typeof(NodeSerializerContracts))]
	public interface INodeSerializer
	{
		void Serialize(string rootPath, INode node, bool recursive);

		INode Deserialize(string rootPath);
	}

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