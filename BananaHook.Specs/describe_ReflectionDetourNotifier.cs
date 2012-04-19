using System;
using NSpec;

namespace BananaHook.Specs
{
    class describe_ReflectionDetourNotifier : nspec
    {
        const int ExpectedValue = 1;
        DetourCallbackEventArgs _eventArgs;
        ReflectionDetourNotifier _notifier;
        object _result;
        SubjectToHook _subject;

        void before_each()
        {
            _subject = new SubjectToHook();
        }

        void when_the_hook_is_called()
        {
            before = () =>
            {
                _notifier = new ReflectionDetourNotifier(HookFactory, _subject.Managed);
                _notifier.Hook.Apply();
            };

            act = () => _result = _subject.Managed(2.0);

            context["and the return value is modified in an event handler"] = () =>
            {
                before = () => _notifier.DetourCalled += (s, e) => e.ReturnValue = ExpectedValue;

                it["should return the modified value"] = () => _result.should_be(ExpectedValue);
                it["should call the original"] = () => _subject.WasManagedCalled.should_be_true();
            };

            context["and the original shall not be called"] = () =>
            {
                before = () => _notifier.DetourCalled += (s, e) =>
                {
                    e.CallOriginal = false;
                    e.ReturnValue = ExpectedValue;
                };

                it["should not call the original"] = () => _subject.WasManagedCalled.should_be_false();
            };

            after = () => _notifier.Dispose();
        }

        void when_the_target_is_unmanaged()
        {
            before = () =>
            {
                _notifier = new ReflectionDetourNotifier(HookFactory, _subject.UnmanagedSleep);
                _notifier.Hook.Apply();
                _notifier.DetourCalled += (s, e) => _eventArgs = e;
            };

            act = () => _subject.UnmanagedSleep(0);

            it["should fire the event as expected"] = () => _eventArgs.should_not_be_null();
        }

        static IHook HookFactory(IntPtr target, IntPtr hook)
        {
            return new RetnHook(new InProcessMemory(), target, hook);
        }
    }
}