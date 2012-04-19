using System;

namespace BananaHook
{
    public class Detour
    {
        private readonly IHook _hook;
        private readonly Delegate _targetDelegate;

        public Detour(IHook hook, Delegate targetDelegate)
        {
            _hook = hook;
            _targetDelegate = targetDelegate;
        }

        #region Implementation of IDetour

        public object Invoke(params object[] parameters)
        {
            object ret;
            bool wasApplied = _hook.IsApplied;
            try
            {
                if (wasApplied)
                    _hook.Remove();

                ret = _targetDelegate.DynamicInvoke(parameters);
            }
            finally
            {
                if (wasApplied)
                    _hook.Apply();
            }
            return ret;
        }

        #endregion
    }
}