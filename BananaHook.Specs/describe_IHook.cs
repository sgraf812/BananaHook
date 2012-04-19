using System;
using System.Runtime.InteropServices;
using BananaHook.Infrastructure;
using NSpec;

namespace BananaHook.Specs
{
    abstract class describe_IHook : nspec
    {
        IHook _hook;
        object _result;
        SubjectToHook _subject;

        void before_each()
        {
            _subject = new SubjectToHook();
            var hookAddress = Marshal.GetFunctionPointerForDelegate((SubjectToHook.ManagedDelegate)Hook);
            _hook = CreateHook(new InProcessMemory(), _subject.ManagedPointer, hookAddress);
        }

        protected abstract IHook CreateHook(IMemory memory, IntPtr targetAddress, IntPtr hookAddress);

        void when_the_hooked_function_is_called()
        {
            act = () => _result = _subject.Managed(2.0);

            context["and the hook was applied"] = () =>
            {
                before = () => _hook.Apply();

                it["should call the hook function"] = () => _result.should_be(1);
                it["should be applied"] = () => _hook.IsApplied.should_be_true();

                context["and then removed again"] = () =>
                {
                    before = () => _hook.Remove();

                    it["should call the original function"] = () => _result.should_be(2);
                    it["should not be applied"] = () => _hook.IsApplied.should_be_false();
                };
            };

            after = () => _hook.Dispose();
        }

        private static int Hook(double d)
        {
            return 1;
        }
    }
}