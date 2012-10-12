# Hook native APIs in a managed flavour #

**BananaHook** provides an easy, signature agnostic detour handling mechanism. It originated in my hobby WoW bot.

## Supported Architectures ##

Currently it is only possible to hook in x86 processes, yet a simple X64Assembler combined with some restructuring should suffice to support x64. 

## Quick Example ##

```
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

## Why should I bother downloading that shit? ##

Through the use of this event model, one can intercept multiple functions, which may even differ in signature, with one generic event handler.
E.g.: Decide to detour EndScene or Present based on the used DirectX version (9 or 11 respectively) and intercept with the same signature agnostic handler. Pretty slick ;)

