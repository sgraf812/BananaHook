using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Linq.Expressions;

namespace BananaHook
{
    public class ReflectionDetourNotifier : IDetourNotifier
    {
        private readonly Delegate _targetDelegate;
        private readonly Delegate _hookDelegate;
        private readonly IDetour _detour;

        public IHook Hook { get; private set; }
        public event EventHandler<DetourCallbackEventArgs> DetourCalled;

        public ReflectionDetourNotifier(Func<IntPtr, IntPtr, IHook> hookFactory, Delegate targetDelegate)
        {
            _targetDelegate = targetDelegate;
            _hookDelegate = GenerateInterceptor(targetDelegate);

            Hook = hookFactory(Marshal.GetFunctionPointerForDelegate(_targetDelegate),
                Marshal.GetFunctionPointerForDelegate(_hookDelegate));
            _detour = Hook.CreateDetour(_targetDelegate.GetType());
        }

        private Delegate GenerateInterceptor(Delegate targetDelegate)
        {
            var delegateType = targetDelegate.GetType();
            var mi = delegateType.GetMethod("Invoke");
            var parameters = (from p in mi.GetParameters()
                              select Expression.Parameter(p.ParameterType, p.Name)).ToArray();

            var builder = new InterceptorFuncBuilder(this);
            return builder.CreateLambdaBody(parameters, delegateType, mi);
        }

        protected virtual void OnDetourCalled(DetourCallbackEventArgs e)
        {
            var detourCalled = DetourCalled;
            if (detourCalled != null)
            {
                detourCalled(this, e);
            }
        }

        private class InterceptorFuncBuilder
        {
            private readonly List<Expression> _body = new List<Expression>();
            private readonly ReflectionDetourNotifier _notifier;

            public InterceptorFuncBuilder(ReflectionDetourNotifier notifier)
            {
                _notifier = notifier;
            }

            /// <summary>
            /// (parameters) =>
            /// {
            ///     List&lt;object&gt; parameters = new List&lt;object&gt; { parameters };
            ///     OnDetourCalled(new DetourCallbackEventArgs(parameters));
            ///     return (T)_detour.Invoke(parameters);
            /// }
            /// </summary>
            public Delegate CreateLambdaBody(ParameterExpression[] parameters, Type delegateType, MethodInfo mi)
            {
                _body.Clear();
                var parameterListVariable = AddParametersToList(parameters);
                var eventArgsVariable = FireEvent(parameterListVariable);
                CallOriginal(mi, parameters, eventArgsVariable);
                var lambda = CompileLambda(parameters, parameterListVariable, eventArgsVariable);
                return CastToOriginalDelegateType(lambda, delegateType);
            }

            /// <summary>
            /// List&lt;object&gt; parameters = new List&lt;object&gt; { ... };
            /// </summary>
            private ParameterExpression AddParametersToList(IEnumerable<ParameterExpression> parameterTypes)
            {
                var parameterVariable = Expression.Variable(typeof(List<object>), "parameters");
                _body.Add(parameterVariable);
                _body.Add(Expression.Assign(parameterVariable, Expression.New(typeof(List<object>))));
                _body.AddRange(from p in parameterTypes
                               select
                                   Expression.Call(parameterVariable, "Add", null, Expression.Convert(p, typeof(object))));
                return parameterVariable;
            }

            /// <summary>
            /// OnDetourCalled(new DetourCallbackEventArgs(parameters));
            /// </summary>
            private ParameterExpression FireEvent(Expression parameterListVariable)
            {
                var eventArgsVariable = Expression.Variable(typeof(DetourCallbackEventArgs), "e");
                _body.Add(eventArgsVariable);
                var eventArgsConstructor =
                    typeof(DetourCallbackEventArgs).GetConstructor(new[] { typeof(IList<object>) });
                _body.Add(Expression.Assign(eventArgsVariable,
                    Expression.New(eventArgsConstructor, parameterListVariable)));
                var fireEvent = Expression.Call(Expression.Constant(_notifier),
                    new Action<DetourCallbackEventArgs>(_notifier.OnDetourCalled).Method,
                    eventArgsVariable);
                _body.Add(fireEvent);
                return eventArgsVariable;
            }

            /// <summary>
            /// ret = _detour.Invoke(...);
            /// </summary>
            private void CallOriginal(MethodInfo mi, IEnumerable<ParameterExpression> parameters,
                ParameterExpression eventArgs)
            {
                Func<object[], DetourCallbackEventArgs, object> actualCall = (p, e) =>
                {
                    object ret = null;
                    if (e.CallOriginal)
                    {
                        ret = _notifier._detour.Invoke(p);
                    }
                    if (e.ReturnValue != null)
                    {
                        ret = e.ReturnValue;
                    }
                    return ret;
                };
                var parametersAsArray = Expression.NewArrayInit(typeof(object),
                    from p in parameters select Expression.Convert(p, typeof(object)));
                var callOriginal = Expression.Call(Expression.Constant(actualCall.Target), actualCall.Method,
                    parametersAsArray, eventArgs);

                if (HasNoReturnValue(mi))
                {
                    _body.Add(callOriginal);
                    _body.Add(Expression.Empty());
                }
                else
                {
                    _body.Add(Expression.Convert(callOriginal, mi.ReturnType));
                }
            }

            private static bool HasNoReturnValue(MethodInfo mi)
            {
                return mi.ReturnType == typeof(void);
            }

            /// <summary>
            /// (...) => { ... };
            /// </summary>
            private Delegate CompileLambda(ParameterExpression[] parameters, params ParameterExpression[] variables)
            {
                var res = Expression.Block(variables, _body);
                var ret = Expression.Lambda(res, parameters).Compile();
                return ret;
            }

            /// <summary>
            /// return (T)ret;
            /// </summary>
            private static Delegate CastToOriginalDelegateType(Delegate ret, Type delegateType)
            {
                return Delegate.CreateDelegate(delegateType, ret, "Invoke");
            }
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            Hook.Dispose();
        }

        #endregion
    }
}