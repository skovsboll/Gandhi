using System;

namespace Ghandi.Love
{
	public interface ITransaction : IDisposable
	{
		void Complete();
	}
}