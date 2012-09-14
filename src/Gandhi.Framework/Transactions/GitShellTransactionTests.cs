using System;
using System.IO;
using Xunit;

namespace Gandhi.Framework.Transactions
{
	public class GitShellTransactionTests
	{
		[Fact]
		public void CanCommit()
		{
			// Initialize main repo
			string originalPath = Path.GetFullPath("original");
			Directory.CreateDirectory(originalPath);
			GitShellTransaction.RunGit(originalPath, "init");

			// Path to clone
			string rootPath = Path.GetDirectoryName(originalPath);
			string clonePath = Path.Combine(rootPath, "clone");

			try
			{
				if (Directory.Exists(clonePath))
					Directory.Delete(clonePath, true);
			}
			catch (UnauthorizedAccessException) { }

			//git clone -l -s -n . ../copy
			GitShellTransaction.RunGit(originalPath, "clone", "-l", "-s", ".", clonePath);

			using (var sut = new GitShellTransaction(clonePath))
			{

				File.WriteAllText(Path.Combine(clonePath, "somefile.txt"), "I'm in hee-eere!");
				
				sut.Complete();
			}
		}

	}
}