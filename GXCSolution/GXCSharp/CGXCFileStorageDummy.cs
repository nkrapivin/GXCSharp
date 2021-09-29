using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GXCSharp
{
    /// <summary>
    /// A dummy file storage used when none is provided, not a part of the public API.
    /// </summary>
    class CGXCFileStorageDummy : IGXCFileStorage
    {
        public Task<string?> GetProperty(EGXCFileProperty prop)
        {
            return Task.FromResult<string?>(null);
        }

        public Task SetProperty(EGXCFileProperty prop, string? value)
        {
            return Task.CompletedTask;
        }
    }
}
