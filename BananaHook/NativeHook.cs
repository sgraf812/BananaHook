using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using BananaHook.Infrastructure;
using BananaHook.Infrastructure.PInvoke;

namespace BananaHook
{
    public class NativeHook : IHook
    {
        private const int OpCodePush = 0x68;
        private const int OpCodeRet = 0xC3;
        private readonly IMemory _memory;
        private readonly MemoryPageProtector _protector;
        private readonly IntPtr _targetAddress;
        private readonly List<byte> _redirectingOpCodes = new List<byte>(6);
        private readonly List<byte> _originalOpCodes = new List<byte>(6);

        public NativeHook(IMemory memory, IntPtr targetAddress, IntPtr hookAddress)
        {
            _memory = memory;
            _targetAddress = targetAddress;
            _protector = new MemoryPageProtector(new Win32Implementation(), targetAddress, new IntPtr(6));

            StoreOriginalOpCodes();
            GenerateDetourOpCodes(hookAddress);
        }

        private void StoreOriginalOpCodes()
        {
            _originalOpCodes.AddRange(_memory.ReadBytes(_targetAddress, 6));
        }

        private void GenerateDetourOpCodes(IntPtr hookAddress)
        {
            // RET sets EIP to the the top of the stack -> hook function is being called.
            // After hook function terminates, the ret instruction pops the EIP and returns to the source function.
            _redirectingOpCodes.Add(OpCodePush);
            byte[] address = BitConverter.GetBytes(hookAddress.ToInt32());
            _redirectingOpCodes.AddRange(address);
            _redirectingOpCodes.Add(OpCodeRet);
        }

        #region Implementation of IDisposable

        ~NativeHook()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (IsApplied)
            {
                Remove();
            }
        }

        #endregion

        #region Implementation of IHook

        public bool IsApplied { get; private set; }

        public void Apply()
        {
            _protector.ExecuteWithProtection(MemoryProtectionConstraints.PAGE_EXECUTE_READWRITE, () =>
                _memory.WriteBytes(_targetAddress, _redirectingOpCodes.ToArray()));
            IsApplied = true;
        }

        public void Remove()
        {
            _protector.ExecuteWithProtection(MemoryProtectionConstraints.PAGE_EXECUTE_READWRITE, () =>
                _memory.WriteBytes(_targetAddress, _originalOpCodes.ToArray()));
            IsApplied = false;
        }

        public IDetour CreateDetour(Type delegateType)
        {
            var targetDelegate = Marshal.GetDelegateForFunctionPointer(_targetAddress, delegateType);
            return new Detour(this, targetDelegate);
        }

        #endregion
    }
}