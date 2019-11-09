using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using Contracts;

namespace App.Logic
{
    [DebuggerDisplay("Key: {_key}; Count: {Count}")]
    public class TreeNode : IComparable<TreeNode>
    {
        private readonly byte _key;
        private readonly TreeNode _parent;
        private readonly ConcurrentDictionary<byte, TreeNode> _children;
        private int _wordsCount;
        public int Count => _wordsCount;
        public int Length { get; }

        public static TreeNode CreateRoot() => new TreeNode(0x20, default);

        private TreeNode(byte key, TreeNode parent)
        {
            _key = key;
            _wordsCount = 0;
            Length = parent is null ? 0 : parent.Length + 1;
            _parent = parent;
            _children = new ConcurrentDictionary<byte, TreeNode>(Constants.THREADS_COUNT, 8);
        }

        public void Add(int length, byte[] word)
        {
            if (word != null)
            {
                length = Math.Min(length, word.Length);
                var node = this;
                var index = 0;

                while (index < length)
                {
                    var key = ToUpper(word[index]);
                    node = node._children.GetOrAdd(key, x => new TreeNode(x, node));
                    index++;
                }

                if (length > 0)
                {
                    Interlocked.Increment(ref node._wordsCount);
                }
            }
        }

        public IEnumerable<TreeNode> GetWords()
        {
            var queue = new Queue<TreeNode>(new[] { this });

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node._wordsCount > 0)
                {
                    yield return node;
                }
                foreach (var item in node._children)
                {
                    queue.Enqueue(item.Value);
                }
            }
        }

        public void Populate(byte[] result)
        {
            if (Length > result.Length)
            {
                throw new ArgumentException("The result array is small");
            }
            if (Length > 0)
            {
                var node = this;
                while (node.Length > 0)
                {
                    result[node.Length - 1] = node._key;
                    node = node._parent;
                }
            }
        }

        public byte[] ToBytes()
        {
            if (Length > 0 )
            {
                var node = this;
                var result = new byte[node.Length];
                while (node.Length > 0)
                {
                    result[node.Length - 1] = node._key;
                    node = node._parent;
                }

                return result;
            }

            return Array.Empty<byte>();
        }

        public int CompareTo(TreeNode other)
        {
            return _wordsCount.CompareTo(other._wordsCount);
        }

        private byte ToUpper(byte value)
        {
            if ((value > 96 && value < 123) || (value > 223))
            {
                return (byte)(value - 32);
            }
            else if (value == 184)
            {
                return (byte)(value - 16);
            }

            return value;
        }
    }
}
