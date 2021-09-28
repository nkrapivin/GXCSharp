using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GXCSharp
{
    /// <summary>
    /// Secure file storage interface.
    /// </summary>
    public interface IGXCFileStorage
    {
        /// <summary>
        /// Puts an item into the secure file storage.
        /// </summary>
        /// <param name="prop">item to set</param>
        /// <param name="value">value of an item</param>
        /// <returns></returns>
        Task SetProperty(EGXCFileProperty prop, string? value);

        /// <summary>
        /// Fetches an item from the secure file storage.
        /// </summary>
        /// <param name="prop">item to get</param>
        /// <returns>the value as a string</returns>
        Task<string?> GetProperty(EGXCFileProperty prop);
    }
}
