namespace Ghandi.Areas.gandi.Love.DataTypes
{
	public interface ISerializable<T>
	{
		T Serialize();
		void Deserialize(T value);
	}
}