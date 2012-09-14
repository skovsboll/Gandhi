using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Gandhi.Framework.DataTypes;
using Xunit;
using FluentAssertions;

namespace Gandhi.Framework.Tests
{
	public class NodeToFolderSerializerTests
	{
		public class TestNode : NodeBase
		{
			public string HeadLine { get; set; }
			public DateTime BirthDay { get; set; }
		}

		[Fact]
		public void CanSerializeNodesRecursively()
		{
			var sut = new NodeToFolderSerializer(new SystemIoFileSystem());

			string rootPath = Path.GetFullPath("temp");
			var node = new NodeBase {Name = "Root", Uri = new Uri("http://root")};
			node.Children.Add(new TestNode { Name = "Barn", Uri = new Uri("http://root/barn"), HeadLine = "Mjallo", BirthDay = DateTime.Today});
			sut.Serialize(rootPath, node, recursive: true);

			string nodePath = Path.Combine(rootPath, "Root");
			File.Exists(Path.Combine(nodePath, "Properties.txt")).Should().BeTrue();
			Directory.EnumerateFiles(nodePath).Count().Should().Be(1);
			Directory.EnumerateDirectories(nodePath).Count().Should().Be(1);
			string barnPath = Path.Combine(nodePath, "Barn");
			Directory.EnumerateFiles(barnPath).Count().Should().Be(3);
		}

		[Fact]
		public void CanSerializeHtmlFields()
		{
			var sut = new NodeToFolderSerializer(new SystemIoFileSystem());

			string rootPath = Path.GetFullPath("temp2");
			var node = new TestNodeWithHtml { Name = "Root", Uri = new Uri("http://root/barn"), HeadLine = "Mjallo", BirthDay = DateTime.Today, Content = XDocument.Parse("<html><body>Hejsa</body></html>")};
			sut.Serialize(rootPath, node, recursive: false);

			string nodePath = Path.Combine(rootPath, "Root");
			File.Exists(Path.Combine(nodePath, "Properties.txt")).Should().BeTrue();
			Directory.EnumerateFiles(nodePath).Count().Should().Be(4);
			Directory.EnumerateDirectories(nodePath).Count().Should().Be(0);
			XDocument.Load(Path.Combine(nodePath, "Content.xml"));
		}

		[Fact]
		public void CanSerializeAndDeserializeMarkdown()
		{
			var sut = new NodeToFolderSerializer(new SystemIoFileSystem());

			string rootPath = Path.GetFullPath("temp2");
			var node = new TestNodeWithMarkdown{ Name = "Root", Uri = new Uri("http://root/barn"), HeadLine = "Mjallo", BirthDay = DateTime.Today, Content = new Markdown { Source =  "Mjallo"} };
			sut.Serialize(rootPath, node, recursive: false);

			string nodePath = Path.Combine(rootPath, "Root");
			File.Exists(Path.Combine(nodePath, "Content.markdown")).Should().BeTrue();

			var reloaded = (TestNodeWithMarkdown)sut.Deserialize(nodePath);
			reloaded.Content.Source.Should().Be("Mjallo");
		}


		[Fact]
		public void CanDeserializeSimpleNode()
		{
			// Arrange
			var sut = new NodeToFolderSerializer(new SystemIoFileSystem());

			string rootPath = Path.GetFullPath("temp2");
			var node = new TestNodeWithHtml { Name = "Root", Uri = new Uri("http://root/barn"), HeadLine = "Mjallo", BirthDay = DateTime.Today, Content = XDocument.Parse("<html><body>Hejsa</body></html>") };
			sut.Serialize(rootPath, node, recursive: false);

			// Act
			var newNode = (TestNodeWithHtml)sut.Deserialize(Path.Combine(rootPath, "Root"));

			// Assert
			newNode.Uri.ToString().Should().BeEquivalentTo("http://root/barn");
			newNode.HeadLine.Should().BeEquivalentTo("Mjallo");
			newNode.Content.Should().HaveRoot("html");
			newNode.Content.Should().HaveElement("body");
			newNode.BirthDay.Should().Be(DateTime.Today);
		}

	}
}