using System;

namespace BananaHook
{
    public interface IHook : IDisposable
    {
        bool IsApplied { get; }
        void Apply();
        void Remove();
        Detour CreateDetour(Type delegateType);
    }
}