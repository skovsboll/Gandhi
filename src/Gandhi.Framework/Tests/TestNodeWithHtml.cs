using System.Xml.Linq;

namespace Gandhi.Framework.Tests
{
	public class TestNodeWithHtml : NodeToFolderSerializerTests.TestNode
	{
		public XDocument Content { get; set; }
	}
}