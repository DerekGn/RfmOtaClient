
using System.Collections.Generic;

namespace RfmOta.Ota
{
    internal class FlashWrites
    {
        private readonly List<IReadOnlyList<byte>> _writes;

        public FlashWrites()
        {
            _writes = new List<IReadOnlyList<byte>>();
        }

        public IReadOnlyList<IReadOnlyList<byte>> Writes => _writes.AsReadOnly();

        public void AddWrite(IReadOnlyList<byte> write)
        {
            _writes.Add(write);
        }

        public IReadOnlyList<byte> GetWritesBytes()
        {
            List<byte> writes = new List<byte>();
            foreach (var write in _writes)
            {
                writes.AddRange(write);
            }

            return writes;
        }
    }
}
