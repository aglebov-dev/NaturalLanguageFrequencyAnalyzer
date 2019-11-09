using System;
using System.Collections.Generic;

namespace Contracts
{
    public interface IReader: IDisposable
    {
        IEnumerable<Package> Read();
    }
}
