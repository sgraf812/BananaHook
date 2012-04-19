using System;
using System.Runtime.InteropServices;

namespace BananaHook.Infrastructure.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ExceptionRecord
    {
        public uint ExceptionCode;
        public uint ExceptionFlags;
        public ExceptionRecord* pExceptionRecord;
        public IntPtr ExceptionAddress;
        public int NumberParameters;
        //public fixed IntPtr ExceptionInformation[15];
    }
}