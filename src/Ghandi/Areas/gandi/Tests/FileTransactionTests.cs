using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ghandi.Love;
using Xunit;

namespace Ghancy.Tests
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
