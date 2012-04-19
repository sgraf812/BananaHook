using System;
using System.Runtime.InteropServices;
using BananaHook.Infrastructure;

namespace BananaHook
{
    public class PatchingHook : IHook
    {
        private readonly Patch _patch;
        private readonly IntPtr _targetAddress;

        public PatchingHook(IMemory memory, IntPtr targetAddress, byte[] redirection)
        {
            _targetAddress = targetAddress;
            _patch = new Patch(memory, targetAddress, redirection);
        }

        protected IntPtr TargetAddress
        {
            get { return _targetAddress; }
        }

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
                _patch.Dispose();
                GC.SuppressFinalize(this);
            }
        }
        
        #endregion

        #region Implementation of IHook

        public bool IsApplied { get { return _patch.IsApplied; } }

        public void Apply()
        {
            _patch.Apply();
        }

        public void Remove()
        {
            _patch.Remove();
        }

        public Detour CreateDetour(Type delegateType)
        {
            var targetDelegate = Marshal.GetDelegateForFunctionPointer(TargetAddress, delegateType);
            return new Detour(this, targetDelegate);
        }

        #endregion
    }
}