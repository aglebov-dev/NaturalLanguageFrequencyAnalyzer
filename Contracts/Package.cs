using System;

namespace Contracts
{
    public struct Package: IDisposable
    {
        private readonly Action _disposeAction;

        public byte[] TextLine { get; }
        public Package(byte[] textLine, Action disposeAction)
        {
            _disposeAction = disposeAction;
            TextLine = textLine ?? Array.Empty<byte>();
        }

        public void Dispose()
        {
            _disposeAction?.Invoke();
        }
    }
}
