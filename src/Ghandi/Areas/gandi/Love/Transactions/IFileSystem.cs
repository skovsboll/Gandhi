using System.IO;

namespace Ghandi.Love
{
	public interface IFileSystem
	{
		void CreateDirectory(string path);
		Stream Open(string path, FileMode mode, FileAccess access, FileShare share);
		void Delete(string path);
		bool DirectoryExists(string path);
		void RemoveDirectoryRecursive(string path);
	}
}