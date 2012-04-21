using System;
using System.Runtime.InteropServices;
using BananaHook.Asm;
using BananaHook.Infrastructure;
using BananaHook.Infrastructure.PInvoke;

namespace BananaHook
{
    public class Int3Hook : PatchingHook
    {
        private const uint EXCEPTION_BREAKPOINT = 0x80000003;
        private const int EXCEPTION_CONTINUE_SEARCH = 0;
        private const int EXCEPTION_CONTINUE_EXECUTION = -1;
        private readonly IntPtr _hookAddress;
        private readonly IntPtr _handler;
        private readonly VectoredHandlerDelegate _notToBeGCed;

        public unsafe Int3Hook(IMemory memory, IntPtr targetAddress, IntPtr hookAddress)
            : base(memory, targetAddress, new[] { (byte)OpCode.Int3 })
        {
            _hookAddress = hookAddress;
            _notToBeGCed = VectoredHandler;
            _handler = AddVectoredExceptionHandler(0, _notToBeGCed);
        }

        private unsafe int VectoredHandler(ExceptionPointers* exceptionInfo)
        {
            var exception = exceptionInfo->ExceptionRecord;
            var context = exceptionInfo->ContextRecord;
            if (exception->ExceptionCode == EXCEPTION_BREAKPOINT)
            {
                if (exception->ExceptionAddress == Patch.TargetAddress)
                {
                    if (IsApplied)
                    {
                        context->Eip = (uint)_hookAddress;
                        return EXCEPTION_CONTINUE_EXECUTION;
                    }
                }
            }
            return EXCEPTION_CONTINUE_SEARCH;
        }

        #region Implementation of IDisposable

        protected override void Dispose(bool disposing)
        {
            RemoveVectoredExceptionHandler(_handler);
            base.Dispose(disposing);
        }

        #endregion

        #region PInvoke

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private unsafe delegate int VectoredHandlerDelegate(ExceptionPointers* exceptionInfo);

        [DllImport("kernel32", CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr AddVectoredExceptionHandler(int firstHandler, VectoredHandlerDelegate vectoredHandler);

        [DllImport("kernel32", CallingConvention = CallingConvention.Winapi)]
        private static extern int RemoveVectoredExceptionHandler(IntPtr handle);

        #endregion
    }
}