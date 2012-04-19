using System.Runtime.InteropServices;

namespace BananaHook.Infrastructure.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ExceptionPointers
    {
        public ExceptionRecord* ExceptionRecord;
        public Context* ContextRecord;
    }
}