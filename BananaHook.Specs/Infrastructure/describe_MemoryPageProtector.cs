using System;
using BananaHook.Infrastructure;
using BananaHook.Infrastructure.PInvoke;
using NSpec;

namespace BananaHook.Specs.Infrastructure
{
    class describe_MemoryPageProtector : nspec
    {
        const MemoryProtectionConstraints RequestedProtection = MemoryProtectionConstraints.PAGE_EXECUTE;
        readonly IntPtr ExpectedAddress = new IntPtr(0x1234);
        readonly IntPtr ExpectedLength = new IntPtr(0x5678);
        StubMemoryProtection _protection;
        MemoryPageProtector _protector;

        void before_each()
        {
            _protection = new StubMemoryProtection();
            _protector = new MemoryPageProtector(_protection, ExpectedAddress, ExpectedLength);
        }

        void when_changing_protection_flags_for_execution()
        {
            act = () => _protector.ExecuteWithProtection(RequestedProtection, () => _protection.CurrentProtectection.should_be(RequestedProtection));

            it["should have set and reset the protection flags"] = () => _protection.HasChanged.should_be_true();
        }

        void when_an_exception_occurs_while_executing_with_modified_protection_flags()
        {
            act = expect<NotFiniteNumberException>(() =>
                _protector.ExecuteWithProtection(RequestedProtection, () => { throw new NotFiniteNumberException(); }));

            it["should have set and reset the protection flags"] = () => _protection.HasChanged.should_be_true();
        }
    }
}