using System;

namespace Gandhi.Framework.Transactions
{
	public interface ITransaction : IDisposable
	{
		void Complete();
	}
}