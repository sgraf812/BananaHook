using System;
using System.Runtime.InteropServices;
using Machine.Specifications;

namespace BananaHook.Specs
{
    [Subject(typeof(NativeHook))]
    public class when_applied : NativeHookSpec
    {
        Establish context = () =>
            _hook.Apply();

        It should_call_the_hook = () =>
            _result.ShouldEqual(1);

        It should_be_applied = () =>
            _hook.IsApplied.ShouldBeTrue();
    }

    [Subject(typeof(NativeHook))]
    public class when_removed : NativeHookSpec
    {
        Establish context = () =>
        {
            _hook.Apply();
            _hook.Remove();
        };

        It should_perform_the_original_operation = () =>
            _result.ShouldEqual(2);

        It should_not_be_applied = () =>
            _hook.IsApplied.ShouldBeFalse();
    }

    public class NativeHookSpec
    {
        protected static SubjectToHook _subject;
        protected static NativeHook _hook;
        protected static object _result;

        Establish context = () =>
        {
            _subject = new SubjectToHook();
            IntPtr hookAddress = Marshal.GetFunctionPointerForDelegate((SubjectToHook.ManagedDelegate) Hook);
            _hook = new NativeHook(new InProcessMemory(), _subject.ManagedPointer, hookAddress);
        };

        Because of = () =>
            _result = _subject.Managed(2.0);

        Cleanup after = () =>
            _hook.Dispose();

        private static int Hook(double d)
        {
            return 1;
        }
    }
}