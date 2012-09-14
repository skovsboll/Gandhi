using Gandhi.Framework.Transactions;
using Xunit;

namespace Gandhi.Framework.Tests
{
	public class FileTransactionTests
	{


		[Fact]
		public void EmptyTransactionRunsWihtoutError()
		{
			var sut = new GitTransactionFactory("c:\temp\test");

			using (var trans = sut.OpenSession())
			{
				

				trans.Complete();
			}
		}


	}
}
