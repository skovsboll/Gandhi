using System;
using System.Diagnostics;
using System.IO;
using Xunit;

namespace Ghandi.Love
{
	public class GitShellTransaction : ITransaction
	{
		private readonly string _path;

		public GitShellTransaction(string path)
		{
			_path = path;

			//Clean();
		}

		private void Clean()
		{
			Git("reset", "--hard", "HEAD");
			Git("pull", "origin", "master");
		}

		public void Dispose()
		{
			Git("reset", "--hard", "HEAD");
		}

		public void Complete()
		{
			Git("pull", "origin", "master");
			Git("add", "-A");
			Git("commit", "-m", "Completed");
		}

		private void Git(params string[] args)
		{
			RunGit(_path, args);
		}

		internal static void RunGit(string rootPath, params string[] args)
		{
			var startInfo = new ProcessStartInfo(@"c:\program files (x86)\git\bin\git.exe", string.Join(" ", args))
			{
				CreateNoWindow = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				WorkingDirectory = rootPath
			};

			Process process = Process.Start(startInfo);
			process.WaitForExit();
			if (process.ExitCode != 0)
				throw new InvalidOperationException("git operation failed: " + process.StandardError.ReadToEnd());

			
		}
	}

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