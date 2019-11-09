using System;

namespace Contracts
{
    public static class Constants
    {
        public const byte R = 0x0D;
        public const byte N = 0x0A;
        public const byte SPACE = 0x20;
        public static int THREADS_COUNT = Environment.ProcessorCount > 2 ? Environment.ProcessorCount : 2;
    }
}
