using System;
using Machine.Specifications;

namespace BananaHook.Specs
{
    [Subject(typeof(ReflectionDetourNotifier))]
    public class when_return_value_is_modified : ReflectionDetourNotifierSpec
    {
        private const int ExpectedValue = 1;
        protected static int _result;

        Establish context = () =>
        {
            _notifier =
                new ReflectionDetourNotifier(HookFactory, _subject.Managed);
            _notifier.DetourCalled += (s, e) =>
                e.ReturnValue = ExpectedValue;
            _notifier.Hook.Apply();
        };

        Because of = () =>
            _result = _subject.Managed(2.0);

        It should_return_the_modified_value = () =>
            _result.ShouldEqual(ExpectedValue);

        It should_call_the_original = () =>
            _subject.WasManagedCalled.ShouldBeTrue();
    }


    [Subject(typeof(ReflectionDetourNotifier))]
    public class when_original_should_not_be_called : ReflectionDetourNotifierSpec
    {
        Establish context = () =>
        {
            _notifier =
                new ReflectionDetourNotifier(HookFactory, _subject.Managed);
            _notifier.DetourCalled += (s, e) =>
            {
                e.CallOriginal = false;
                e.ReturnValue = 1;
            };
            _notifier.Hook.Apply();
        };

        Because of = () =>
            _subject.Managed(2.0);

        It should_not_call_the_original = () =>
            _subject.WasManagedCalled.ShouldBeFalse();
    }

    [Subject(typeof(ReflectionDetourNotifier))]
    public class when_unmanaged_target_is_called : ReflectionDetourNotifierSpec
    {
        protected static DetourCallbackEventArgs _eventArgs;

        Establish context = () =>
        {
            _notifier =
                new ReflectionDetourNotifier(HookFactory, _subject.UnmanagedSleep);
            _notifier.DetourCalled += (s, e) => _eventArgs = e;
            _notifier.Hook.Apply();
        };

        Because of = () =>
            _subject.UnmanagedSleep(0);

        It should_fire_the_event = () =>
            _eventArgs.ShouldNotBeNull();
    }

    [Subject(typeof(ReflectionDetourNotifier))]
    public class when_managed_target_is_called : ReflectionDetourNotifierSpec
    {
        private const int ExpectedValue = 2;
        protected static DetourCallbackEventArgs _eventArgs;
        protected static int _result;

        Establish context = () =>
        {
            _notifier =
                new ReflectionDetourNotifier(HookFactory, _subject.Managed);
            _notifier.DetourCalled += (s, e) => _eventArgs = e;
            _notifier.Hook.Apply();
        };

        Because of = () =>
            _result = _subject.Managed(ExpectedValue);

        It should_fire_the_event = () =>
            _eventArgs.ShouldNotBeNull();

        It should_call_the_original = () =>
            _subject.WasManagedCalled.ShouldBeTrue();
    }

    public class ReflectionDetourNotifierSpec
    {
        protected static SubjectToHook _subject;
        protected static ReflectionDetourNotifier _notifier;

        Establish context = () => _subject = new SubjectToHook();

        protected static IHook HookFactory(IntPtr target, IntPtr hook)
        {
            return new NativeHook(new InProcessMemory(), target, hook);
        }
    }
}