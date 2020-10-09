using Castle.DynamicProxy;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FaceImageAPI.Domain.AOP
{
    /// <summary>
    /// Aop 日志拦截器
    /// </summary>
    public class ExceptionLogInterceptor : IInterceptor
    {
        //private readonly ILogger<ExceptionLogInterceptor> logger;

        //public ExceptionLogInterceptor(ILogger<ExceptionLogInterceptor> logger)
        //{
        //    this.logger = logger;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            var logger = LogManager.GetLogger(invocation.GetType().FullName);
            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                if (invocation.Method.ReturnType != typeof(void))
                {
                    try
                    {
                        invocation.ReturnValue = Activator.CreateInstance(invocation.Method.ReturnType);
                    }
                    catch
                    {
                        invocation.ReturnValue = null;
                    }
                }
            }
        }
    }

    internal static class InternalAsyncHelper
    {
        public static async Task AwaitTaskWithPostActionAndFinally(Task actualReturnValue, Func<Task> postAction, Action<Exception> finalAction)
        {
            Exception exception = null;

            try
            {
                await actualReturnValue;
                await postAction();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                finalAction(exception);
            }
        }

        public static async Task<T> AwaitTaskWithPostActionAndFinallyAndGetResult<T>(Task<T> actualReturnValue, Func<object, Task> postAction, Action<Exception> finalAction)
        {
            Exception exception = null;
            try
            {
                var result = await actualReturnValue;
                await postAction(result);
                return result;
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                finalAction(exception);
            }
        }

        public static object CallAwaitTaskWithPostActionAndFinallyAndGetResult(Type taskReturnType, object actualReturnValue, Func<object, Task> action, Action<Exception> finalAction)
        {
            return typeof(InternalAsyncHelper)
                .GetMethod("AwaitTaskWithPostActionAndFinallyAndGetResult", BindingFlags.Public | BindingFlags.Static)
                .MakeGenericMethod(taskReturnType)
                .Invoke(null, new object[] { actualReturnValue, action, finalAction });
        }
    }

}
