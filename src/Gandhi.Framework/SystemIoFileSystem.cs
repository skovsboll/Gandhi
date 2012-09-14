using System.IO;

namespace Ghandi.Love
{
	public class SystemIoFileSystem : IFileSystem
	{
		public void CreateDirectory(string path)
		{
			Directory.CreateDirectory(path);
		}

		public void Delete(string path)
		{
			File.Delete(path);
		}

		public bool DirectoryExists(string path)
		{
			return Directory.Exists(path);
		}

		public void RemoveDirectoryRecursive(string path)
		{
			Directory.Delete(path, recursive: true);
		}

		public Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
		{
			return File.Open(path, mode, access, share);
		}
	}
}