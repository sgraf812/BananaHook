# Hook native APIs in a managed flavour #

**BananaHook** provides an easy, signature agnostic detour handling mechanism. It originated in my hobby WoW bot.

## Supported Architectures ##

Currently it is only possible to hook in x86 processes, yet a simple X64Assembler combined with some restructuring should suffice to support x64. 

## Quick Example ##

```cs
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int EndSceneDelegate(IntPtr devicePointer);

...

EndSceneDelegate d = (EndSceneDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(0x1234), typeof(EndSceneDelegate));
var notifier = new ReflectionDetourNotifier((t, h) => new RetnHook(new InProcessMemory(), t, h), d);
notifier.DetourCalled += (s, e) => { foreach (var p in e.Parameters) Console.WriteLine(p.ToString()); };
notifier.Hook.Apply();
```

Looks noisy, but definitely is extensible (added Int3Hook without much problems for example) and very clean.

----------

## Naming ##
A short legend for those concepts (or at least how I use them):
*Hooking* is the interception of control flow with a given function with the same signature. It does not necessarily call or simulate the behavior of the original.
A *detour* is a special hook that returns control flow to the original function after doing its work (a redirection).
A *detour notifier* is just a little extension to the detour concept, which defines 'doing its work' as firing up an event. 

## Why should I bother downloading this? ##

Through the use of this event model, one can intercept multiple functions, which may even differ in signature, with one generic event handler.
E.g.: Decide to detour EndScene or Present based on the used DirectX version (9 or 11 respectively) and intercept with the same signature agnostic handler. Pretty slick ;)

## Somewhat more elaborate example
Providing a factory with parameters to a constructor that actually instantiates its dependency with that factory is clumsy and headspinning.
Too be fair, it's supposed to be used with an IoC container such as Autofac:
```cs
// Inside the IoC containers initialization module:
builder.RegisterType<InProcessMemory>();
builder.RegisterType<RetnHook>().As<IHook>();
builder.Register(DetourNotifierFactory);
builder.RegisterType<WhatEver>(); // see below

...

private static IDetourNotifier DetourNotifierFactory(IComponentContext container, IEnumerable<Parameter> parameters)
{
    return new ReflectionDetourNotifier(container.Resolve<ReflectionDetourNotifier.HookFactory>(),
       parameters.TypedAs<Delegate>());
}

...

// Inside your actual code:
class WhatEver
{
    private readonly EndSceneDelegate _endScene = ...; // probably inject through a ctor parameter too
    private readonly IDetourNotifier _notifier;

    public WhatEver(Func<Delegate, IDetourNotifier> notifierFactory)
    {
        _notifier = notifierFactory(_endScene);
        _notifier.DetourCalled += (s, e) => { foreach (var p in e.Parameters) Console.WriteLine(p.ToString()); }; // or sth
        _notifier.Hook.Apply();
    }
}

...

var we = container.Resolve<WhatEver>(); 
```
Note how decoupled and testable `WhatEver` is now. No noise regarding signatures etc. 
You can even switch to another delegate signature without changing any code (if your event handler doesn't rely on certain parameters of course).
