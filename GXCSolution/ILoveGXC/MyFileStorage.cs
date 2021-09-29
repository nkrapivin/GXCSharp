using GXCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILoveGXC
{
    class MyFileStorage : IGXCFileStorage
    {
        public async Task<string?> GetProperty(EGXCFileProperty prop)
        {
            try
            {
                var impl = await MyFileImpl.Fetch();
                switch (prop)
                {
                    case EGXCFileProperty.REFRESH_TOKEN:
                        {
                            return impl.RefreshToken;
                        }

                    default:
                        {
                            return null;
                        }
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task SetProperty(EGXCFileProperty prop, string? value)
        {
            try
            {
                var impl = await MyFileImpl.Fetch();
                var dosave = true;

                switch (prop)
                {
                    case EGXCFileProperty.REFRESH_TOKEN:
                        {
                            impl.RefreshToken = value;
                            break;
                        }

                    default:
                        {
                            // no valid property has been set, don't bother saving.
                            dosave = false;
                            break;
                        }
                }

                if (dosave)
                {
                    await MyFileImpl.Push(impl);
                }
            }
            catch
            {
                // TODO: somehow report set errors?
            }
        }
    }
}
