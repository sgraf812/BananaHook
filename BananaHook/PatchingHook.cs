using System;
using System.Runtime.InteropServices;
using BananaHook.Infrastructure;

namespace BananaHook
{
    public class PatchingHook : IHook
    {
        public PatchingHook(IMemory memory, IntPtr targetAddress, byte[] redirection)
        {
            Patch = new Patch(memory, targetAddress, redirection);
        }

        protected Patch Patch { get; private set; }

        #region Implementation of IDisposable

        public void Dispose()
        {
            Dispose(true);
        }

        ~PatchingHook()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Patch.Dispose();
                GC.SuppressFinalize(this);
            }
        }
        
        #endregion

        #region Implementation of IHook

        public bool IsApplied { get { return Patch.IsApplied; } }

        public void Apply()
        {
            Patch.Apply();
        }

        public void Remove()
        {
            Patch.Remove();
        }

        public Detour CreateDetour(Type delegateType)
        {
            var targetDelegate = Marshal.GetDelegateForFunctionPointer(Patch.TargetAddress, delegateType);
            return new Detour(this, targetDelegate);
        }

        #endregion
    }
}