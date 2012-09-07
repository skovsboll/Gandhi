using System;
using System.Runtime.Serialization;

namespace Ghandi.Areas.gandi.Love.DataTypes
{
	public class Markdown : ISerializable<string>
	{
		public string Source { get; set; }

		public string Serialize()
		{
			return Source;
		}

		public void Deserialize(string value)
		{
			Source = value;
		}
	}
}