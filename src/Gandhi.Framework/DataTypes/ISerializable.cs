namespace Gandhi.Framework.DataTypes
{
	public interface ISerializable<T>
	{
		T Serialize();
		void Deserialize(T value);
	}
}