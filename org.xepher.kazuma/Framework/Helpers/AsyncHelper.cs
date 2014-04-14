using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Helpers
{
    /// <summary>
    /// Helper class for calling async methods from synchronous methods.
    /// </summary>
    /// <remarks>
    /// The WinRT version of this implementation is borrowed from Stephen Toub.
    /// http://blogs.msdn.com/b/pfxteam/archive/2012/01/20/10259049.aspx
    /// </remarks>
    public static class AsyncHelper
    {
        /// <summary>
        /// Loads data asynchronously, calling the callback when completed.
        /// </summary>
        /// <typeparam name="T">The type of the data being loaded</typeparam>
        /// <param name="callback">The Action that will receive the result</param>
        /// <param name="loader">The Func that will load the data</param>
        public static void LoadData<T>(Action<T> callback, Func<Task<T>> loader)
        {
            var task = loader();
            var awaiter = task.GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                if (task.Exception != null)
                {
                    throw task.Exception;
                }
                else
                {
                    callback(task.Result);
                }
            });
        }
    }
}
