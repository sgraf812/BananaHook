using System;
using System.Collections.Generic;

namespace BananaHook
{
    public interface IDetourNotifier : IDisposable
    {
        IHook Hook { get; }
        event EventHandler<DetourCallbackEventArgs> DetourCalled;
    }

    public class DetourCallbackEventArgs : EventArgs
    {
        public IList<object> Parameters { get; private set; }
        public bool CallOriginal { get; set; }
        public object ReturnValue { get; set; }

        public DetourCallbackEventArgs(IList<object> parameters)
        {
            Parameters = parameters;
            CallOriginal = true;
        }
    }
}