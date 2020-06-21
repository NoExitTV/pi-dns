using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pi.Dns.Server.Api.Utilities
{
    public interface IBashUtil
    {
        /// <summary>
        /// Execute any string as a bash command
        /// and return the standard output as a enumerable of strings
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> ExecuteBash(string cmd);
    }
}
