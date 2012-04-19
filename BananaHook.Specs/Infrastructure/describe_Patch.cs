using System;
using System.Runtime.InteropServices;
using BananaHook.Infrastructure;
using BananaHook.Infrastructure.PInvoke;
using NSpec;

namespace BananaHook.Specs.Infrastructure
{
    class describe_Patch : nspec
    {
        IntPtr buffer;
        byte[] replaceWith;
        IMemory memory;
        Patch patch;

        void before_each()
        {
            buffer = Marshal.AllocHGlobal(4);
            replaceWith = new byte[] { 0x37, 0x13, 0, 0 };
            Marshal.WriteInt32(buffer, 0);
            memory = new InProcessMemory();
            patch = new Patch(memory, buffer, replaceWith);
        }

        void after_each()
        {
            Marshal.Release(buffer);
        }

        void when_applying()
        {
            act = () => patch.Apply();

            it["should replace the bytes"] = () => memory.ReadBytes(buffer, 4).should_be(replaceWith);
            it["should be applied"] = () => patch.IsApplied.should_be_true();

            context["when removed"] = () =>
            {
                act = () => patch.Remove();

                it["should restore the original bytes"] = () => memory.ReadBytes(buffer, 4).should_be(new[] { 0, 0, 0, 0 });
                it["should not be applied"] = () => patch.IsApplied.should_be_false();
            };
        }

        void when_applying_in_a_non_writable_region()
        {
            act = () => new MemoryPageProtector(new Win32Implementation(), buffer, (IntPtr)4)
                .ExecuteWithProtection(MemoryProtectionConstraints.PAGE_EXECUTE_READ, patch.Apply);

            it["should replace nevertheless"] = () => memory.ReadBytes(buffer, 4).should_be(replaceWith);
            it["should be applied"] = () => patch.IsApplied.should_be_true();
        }
    }
}