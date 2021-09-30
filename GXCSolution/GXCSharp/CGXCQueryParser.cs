using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace GXCSharp
{
    static class CGXCQueryParser
    {
        public static IDictionary<string, string> Parse(string queryString)
        {
            var ret = new Dictionary<string, string>();
            var copy = queryString;

            copy = copy.TrimStart('/', '?');

            var coll = HttpUtility.ParseQueryString(copy);
            if (coll is NameValueCollection)
            {
                foreach (string key in coll)
                {
                    ret[key] = coll[key];
                }
            }

            return ret;
        }
    }
}
