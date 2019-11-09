using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace App.Logic
{
    internal class FilePathService
    {
        public IEnumerable<string> GetFilePaths(string folderPath, string searchPatern, CancellationToken token)
        {
            if (Directory.Exists(folderPath))
            {
                var files = Directory.GetFiles(folderPath, searchPatern, SearchOption.AllDirectories);
                var queue = new Queue<string>(files);
                while (!token.IsCancellationRequested && queue.Count > 0)
                {
                    yield return queue.Dequeue();
                }
            }
        }
    }
}
