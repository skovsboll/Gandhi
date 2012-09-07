using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Ghandi.Areas.gandi.Love.DataTypes;

namespace Ghandi.Love
{
	public class NodeToFolderSerializer : INodeSerializer
	{
		private readonly IFileSystem _fileSystem;

		public NodeToFolderSerializer(IFileSystem fileSystem)
		{
			_fileSystem = fileSystem;
		}


		// XDocument / XElement --> tinyMCE
		// IEnumerable<INode> --> multiSelect
		// INode descendant --> node selector
		// string --> singleLine
		// Markdown
		// [MultiLine] string --> multiLine

		class MetaModel
		{
			public MetaModel(INode node, string nodePath)
			{
				_props =
					(from p in node.GetType().GetProperties()
					 select new PropertyDescriptor(p, p.Name, Path.Combine(nodePath, p.Name), p.GetValue(node, new object[0])))
						.ToArray();

			}

			private static readonly string[] ExcludedProperties = new[] { "Children", "Name", "Uri", "Parent", "Template", "Publish", "Unpublish" };
			private readonly PropertyDescriptor[] _props;

			public IEnumerable<PropertyDescriptor> Collections
			{
				get
				{
					return (from p in _props where p.Property.PropertyType.IsSubclassOf(typeof(ICollection<>)) select p).ToArray();
				}
			}
			public IEnumerable<PropertyDescriptor> XmlProperties
			{
				get
				{
					return (from p in _props where typeof(XNode).IsAssignableFrom(p.Property.PropertyType) select p).ToArray();
				}
			}

			public IEnumerable<PropertyDescriptor> SerializableOfStringProperties
			{
				get
				{
					return (from p in _props where typeof(ISerializable<string>).IsAssignableFrom(p.Property.PropertyType) select p).ToArray();
				}
			}

			public IEnumerable<PropertyDescriptor> Properties
			{
				get
				{
					return _props
						.Except(Collections)
						.Except(XmlProperties)
						.Except(SerializableOfStringProperties)
						.Where(p => !ExcludedProperties.Contains(p.Name));
				}
			}

		}

		public class PropertyDescriptor
		{
			private readonly PropertyInfo _property;
			private readonly string _name;
			private readonly string _path;
			private readonly object _value;

			public PropertyInfo Property
			{
				get { return _property; }
			}

			public string Name
			{
				get { return _name; }
			}

			public string Path
			{
				get { return _path; }
			}

			public object Value
			{
				get { return _value; }
			}

			public PropertyDescriptor(PropertyInfo property, string name, string path, object value)
			{
				_property = property;
				_name = name;
				_path = path;
				_value = value;
			}
		}

		public void Serialize(string rootPath, INode node, bool recursive = false)
		{
			string nodePath = Path.Combine(rootPath, node.Name.SanitizeUrl());

			if (!_fileSystem.DirectoryExists(nodePath))
				_fileSystem.CreateDirectory(nodePath);

			var metaModel = new MetaModel(node, nodePath);

			using (var fs = _fileSystem.Open(Path.Combine(nodePath, "Properties.txt"), FileMode.Create, FileAccess.Write, FileShare.None))
			using (var writer = new StreamWriter(fs))
			{
				writer.WriteLine("Type: {0}", node.GetType().FullName);
				writer.WriteLine("Urls: {0}", node.Uri);
				writer.WriteLine("Template: {0}", node.Template);
				writer.WriteLine("Publish: {0}", "Never");
				writer.WriteLine("Unpublish: {0}", "Never");
			}

			foreach (var s in metaModel.Properties)
			{
				string path = Path.Combine(nodePath, s.Name.SanitizeUrl() + ".txt");
				using (var fs = _fileSystem.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
				using (var writer = new StreamWriter(fs))
				{
					writer.Write(s.Value.ToString());
				}
			}

			foreach (var prop in metaModel.XmlProperties)
			{
				string path = Path.Combine(nodePath, prop.Name.SanitizeUrl() + ".xml");
				using (var fs = _fileSystem.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
				using (var writer = new StreamWriter(fs))
				{
					writer.Write(prop.Value.ToString());
				}
			}

			foreach (var s in metaModel.SerializableOfStringProperties)
			{
				string path = Path.Combine(nodePath, s.Name.SanitizeUrl() + "." + s.Property.PropertyType.Name);
				using (var fs = _fileSystem.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
				using (var writer = new StreamWriter(fs))
				{
					writer.Write(((ISerializable<string>)s.Value).Serialize());
				}
			}

			//  TODO
			//foreach (var collection in collections.Where(c => !ExcludedProperties.Contains(c.Name)))
			//{

			//}

			if (recursive)
				foreach (var child in node.Children)
					Serialize(rootPath: nodePath, node: child, recursive: true);
		}

		public INode Deserialize(string nodePath)
		{
			string nodeName = Path.GetFileName(nodePath);

			INode node;
			using (var fs = _fileSystem.Open(Path.Combine(nodePath, "Properties.txt"), FileMode.Open, FileAccess.Read, FileShare.Read))
			using (var reader = new StreamReader(fs))
			{
				string type = reader.ReadLine().Split(':')[1].Trim();
				node = (INode)Activator.CreateInstance("Ghandi", type).Unwrap();
				node.Name = nodeName;

				while (!reader.EndOfStream)
				{
					Parse(node, reader.ReadLine());
				}
			}

			var metaModel = new MetaModel(node, nodePath);

			foreach (var s in metaModel.Properties)
			{
				string path = Path.Combine(nodePath, s.Name.SanitizeUrl() + ".txt");
				using (var fs = _fileSystem.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
				using (var reader = new StreamReader(fs))
				{
					s.Property.SetValue(node, Convert.ChangeType(reader.ReadToEnd(), s.Property.PropertyType), new object[0]);
				}
			}

			foreach (var s in metaModel.XmlProperties)
			{
				string path = Path.Combine(nodePath, s.Name.SanitizeUrl() + ".xml");
				s.Property.SetValue(node, XDocument.Load(path), new object[0]);
			}

			foreach (var s in metaModel.SerializableOfStringProperties)
			{
				string path = Path.Combine(nodePath, s.Name.SanitizeUrl() + "." + s.Property.PropertyType.Name);
				using (var fs = _fileSystem.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
				using (var reader = new StreamReader(fs))
				{
					string stringValue = reader.ReadToEnd();
					ISerializable<string> propertyValue = (ISerializable<string>)Activator.CreateInstance(s.Property.PropertyType);
					propertyValue.Deserialize(stringValue);
					s.Property.SetValue(node, propertyValue, new object[0]);
				}
			}

			return node;
		}

		private void Parse(INode node, string line)
		{
			//writer.WriteLine("Urls: {0}", node.Uri);
			//writer.WriteLine("Template: {0}", node.Template);
			//writer.WriteLine("Publish: {0}", "Never");
			//writer.WriteLine("Unpublish: {0}", "Never");

			var parts = line.Split(':').Select(s => s.Trim()).ToArray();
			switch (parts[0])
			{
				case "Urls":
					node.Uri = new Uri(string.Join(":", parts.Skip(1).ToArray()));
					break;
				case "Template":
					node.Template = parts[1];
					break;

			}
		}
	}
}