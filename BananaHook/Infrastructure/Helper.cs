using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace BananaHook.Infrastructure
{
    public static class Helper
    {
        public static void ThrowWin32ExceptionIfFalse(Func<bool> func)
        {
            if (!func())
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}