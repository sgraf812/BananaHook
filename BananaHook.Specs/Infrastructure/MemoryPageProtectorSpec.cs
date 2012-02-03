using System;
using BananaHook.Infrastructure;
using BananaHook.Infrastructure.PInvoke;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace BananaHook.Specs.Infrastructure
{
    [Subject(typeof(MemoryPageProtector))]
    public class when_changing_protection_flags_for_execution : MemoryPageProtectorSpec
    {
        protected static MemoryProtectionConstraints RequestedProtection = MemoryProtectionConstraints.PAGE_EXECUTE;
        protected static MemoryProtectionConstraints OldProtection = MemoryProtectionConstraints.PAGE_WRITECOPY;

        Establish context = () =>
        {
            _protectionMock.Setup(
                x => x.VirtualProtect(_expectedAddress, _expectedLength, RequestedProtection, out OldProtection))
                .Returns(true).Verifiable("Protection flags weren't set");
            _protectionMock.Setup(
                x => x.VirtualProtect(_expectedAddress, _expectedLength, OldProtection, out RequestedProtection))
                .Returns(true).Verifiable("Protection flags weren't reset");
        };

        Because of = () =>
            _protector.ExecuteWithProtection(RequestedProtection, () => { ; });

        It should_set_and_reset_the_protection_flags_accordingly = () =>
            _protectionMock.Verify();
    }

    [Subject(typeof(MemoryPageProtector))]
    public class when_an_exception_occurs_while_executing_with_modified_protection_flags : MemoryPageProtectorSpec
    {
        protected static MemoryProtectionConstraints RequestedProtection = MemoryProtectionConstraints.PAGE_EXECUTE;
        protected static MemoryProtectionConstraints OldProtection = MemoryProtectionConstraints.PAGE_WRITECOPY;

        Establish context = () =>
        {
            _protectionMock.Setup(
                x => x.VirtualProtect(_expectedAddress, _expectedLength, RequestedProtection, out OldProtection))
                .Returns(true).Verifiable("Protection flags weren't set");
            _protectionMock.Setup(
                x => x.VirtualProtect(_expectedAddress, _expectedLength, OldProtection, out RequestedProtection))
                .Returns(true).Verifiable("Protection flags weren't reset");
        };

        Because of = () =>
        {
            try
            {
                _protector.ExecuteWithProtection(RequestedProtection, () => { throw new NotFiniteNumberException(); });
            }
            catch (NotFiniteNumberException)
            {
            }
        };

        It should_try_set_and_reset_the_protection_flags_accordingly = () =>
            _protectionMock.Verify();
    }

    [Subject(typeof(MemoryPageProtector))]
    public class MemoryPageProtectorSpec
    {
        protected static readonly IntPtr _expectedAddress = new IntPtr(0x1234);
        protected static readonly IntPtr _expectedLength = new IntPtr(0x5678);
        protected static MemoryPageProtector _protector;
        protected static Mock<IMemoryProtection> _protectionMock;

        Establish context = () =>
        {
            _protectionMock = new Mock<IMemoryProtection>();
            _protector = new MemoryPageProtector(_protectionMock.Object, _expectedAddress, _expectedLength);
        };
    }
}