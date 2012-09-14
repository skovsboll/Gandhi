using System.Diagnostics.Contracts;

namespace Gandhi.Framework
{
	[ContractClass(typeof(NodeSerializerContracts))]
	public interface INodeSerializer
	{
		void Serialize(string rootPath, INode node, bool recursive);

		INode Deserialize(string rootPath);
	}
}