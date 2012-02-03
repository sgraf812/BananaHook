using System;
using System.Runtime.InteropServices;

namespace BananaHook.Specs
{
    public class SubjectToHook
    {
        private readonly IntPtr _module;
        private readonly ManagedDelegate _managed;
        private readonly SleepDelegate _unmanagedSleep;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int ManagedDelegate(double d);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate void SleepDelegate(int millis);

        public bool WasManagedCalled { get; private set; }

        // ugly hack... Marshal.GetFunctionPointerForDelegate returns a trampoline to real function, thus we have to get the delegate from that pointer.
        public ManagedDelegate Managed
        {
            get { return (ManagedDelegate) Marshal.GetDelegateForFunctionPointer(ManagedPointer, typeof(ManagedDelegate)); }
        }

        public IntPtr ManagedPointer
        {
            get { return Marshal.GetFunctionPointerForDelegate(_managed); }
        }

        public SleepDelegate UnmanagedSleep
        {
            get { return _unmanagedSleep; }
        }

        public SubjectToHook()
        {
            _module = LoadLibrary("kernel32");
            _unmanagedSleep = (SleepDelegate) Marshal.GetDelegateForFunctionPointer(
                GetProcAddress(_module, "Sleep"), typeof(SleepDelegate));
            _managed = d =>
            {
                WasManagedCalled = true;
                return (int) d;
            };
        }

        ~SubjectToHook()
        {
            FreeLibrary(_module);
        }

        #region PInvoke

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr LoadLibrary(string name);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string name);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern bool FreeLibrary(IntPtr hModule);

        #endregion
    }
}