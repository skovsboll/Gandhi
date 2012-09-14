using System;
using System.IO;
using NGit.Api;

namespace Gandhi.Framework.Transactions
{
	public class GitTransactionFactory
	{
		private readonly string _remoteRepository;
		private readonly Pool<DirectoryInfo> _workingFolders = new Pool<DirectoryInfo>(GetNewTempDir);

		private static DirectoryInfo GetNewTempDir()
		{
			string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			string tempName = ShortGuid.NewGuid();
			return new DirectoryInfo(Path.Combine(appData, tempName));
		}

		public GitTransactionFactory(string remoteRepository)
		{
			_remoteRepository = remoteRepository;

			Git.Init().SetDirectory(_remoteRepository).Call();
		}

		public ITransaction OpenSession()
		{
			var lease = _workingFolders.GetLease();

			var command = Git.CloneRepository();
			command.SetDirectory(lease.Value.FullName);
			command.SetURI(_remoteRepository);
			var git = command.Call();
			var repository = git.GetRepository();

			return new GitShellTransaction(repository.Directory.ToString());
		}

		internal void Release(Pool<DirectoryInfo>.Lease lease)
		{
			_workingFolders.Release(lease);
		}
	}
}