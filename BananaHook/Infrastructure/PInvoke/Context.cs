using System.Runtime.InteropServices;

namespace BananaHook.Infrastructure.PInvoke
{
    /// <summary>
    /// Credits to pinvoke.net
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Context
    {
        public uint ContextFlags; //set this to an appropriate value 0x00
        // Retrieved by _context_DEBUG_REGISTERS 
        public uint Dr0; // 0x04
        public uint Dr1; // 0x08
        public uint Dr2; // 0x0C
        public uint Dr3; // 0x10
        public uint Dr6; // 0x14
        public uint Dr7; // 0x18
        // Retrieved by _context_FLOATING_POINT
        private fixed byte FLOATING_SAVE_AREA[0x70];
        //publicFLOATING_SAVE_AREA FloatSave; // 0x1C size: 0x70
        // Retrieved by CONTEXT_SEGMENTS
        public uint SegGs; // 0x90
        public uint SegFs; // 0x94
        public uint SegEs; // 0x98
        public uint SegDs; // 0x9C
        // Retrieved by _context_INTEGER
        public uint Edi; // 0xA0
        public uint Esi; // 0xA4
        public uint Ebx; // 0xA8
        public uint Edx; // 0xAC
        public uint Ecx; // 0xB0
        public uint Eax; // 0xB4
        // Retrieved by _context_CONTROL
        public uint Ebp; // 0xB8
        public uint Eip; // 0xBC
        public uint SegCs; // 0xC0
        public uint EFlags; // 0xC4
        public uint Esp; // 0xC8
        public uint SegSs; // 0xCC
        // Retrieved by _context_EXTENDED_REGISTERS
        public fixed byte ExtendedRegisters[0x200];
    }
}