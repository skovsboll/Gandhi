using System.Linq;
using NGit;
using NGit.Api;
using NGit.Merge;
using NGit.Storage.File;

namespace Ghandi.Love
{
	public class NGitTransaction : ITransaction
	{
		private readonly string _path;
		private readonly Git _git;

		internal NGitTransaction(string path)
		{
			_path = path;
			_git = Git.Open(path);

			Clean();
		}

		internal NGitTransaction(Repository repository)
		{
			_path = repository.WorkTree;
			_git = new Git(repository);

			Clean();
		}

		public string Path
		{
			get { return _path; }
		}

		private void Clean()
		{
			_git.Reset().SetMode(ResetCommand.ResetType.HARD).SetRef("~").Call();
			_git.Fetch().Call();
			_git.Merge().SetStrategy(MergeStrategy.THEIRS).Call();
		}

		public void Dispose()
		{
			_git.Reset().SetMode(ResetCommand.ResetType.HARD).SetRef("~").Call();
		}


		public void Complete()
		{
			_git.Add().SetUpdate(true).Call();
			_git.Fetch().Call();
			_git.Merge().SetStrategy(MergeStrategy.OURS).Call();
			_git.Commit().SetMessage("Transaction completed").Call();
		}
	}
}