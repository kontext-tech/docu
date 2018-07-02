using System;
using System.Collections.Generic;

namespace Kontext.Utils
{
    /// <summary>
    /// Provides methods to convert Enums to arrays
    /// </summary>
    public interface IEnumHelper
    {
        /// <summary>
        /// Convert enums to list of value and name tuples
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        IEnumerable<Tuple<int, string>> GetEnumList(Type enumType);

        /// <summary>
        /// Perform bitwise or operations
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        object GetEnumFromArray(Type enumType, int[] values);

        /// <summary>
        /// Get flags
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        IEnumerable<Enum> GetFlags(Enum input);
    }
}
