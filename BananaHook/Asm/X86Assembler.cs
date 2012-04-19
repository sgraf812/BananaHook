using System;
using System.Collections.Generic;
using System.Linq;

namespace BananaHook.Asm
{
    public class X86Assembler
    {
        private readonly IList<byte> _bytes = new List<byte>();

        public void Push(IntPtr address)
        {
            Emit(OpCode.Push);
            Emit(address);
        }

        public void Retn()
        {
            Emit(OpCode.Retn);
        }

        public void Retn(short purgeBytes)
        {
            Emit(OpCode.RetnAndPurge);
            Emit(purgeBytes);
        }

        private void Emit(OpCode op)
        {
            _bytes.Add((byte)op);
        }

        private void Emit(IntPtr address)
        {
            Emit(BitConverter.GetBytes(address.ToInt32()));
        }

        private void Emit(short word)
        {
            Emit(BitConverter.GetBytes(word));
        }

        private void Emit(IEnumerable<byte> bytes)
        {
            foreach (var b in bytes)
                _bytes.Add(b);
        }

        public byte[] GetBytes()
        {
            return _bytes.ToArray();
        }
    }
}
