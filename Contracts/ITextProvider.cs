using System.Threading;

namespace Contracts
{
    public interface ITextProvider
    {
        string SearchPatern { get; }
        IReader CreateReader(string filePath, CancellationToken token);
    }
}
