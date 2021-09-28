using System;
using System.Collections.Generic;
using System.Text;

namespace GXCSharp
{
    /// <summary>
    /// API Result.
    /// </summary>
    /// <typeparam name="TResult">the value of result or null</typeparam>
    public class CGXCResult<TResult> where TResult : class
    {
        public TResult? Value { get; private set; }

        public EGXCErrorCode Error { get; private set; }

        public bool Success { get; private set; }

        private CGXCResult(bool _ok, TResult? _vv, EGXCErrorCode _err)
        {
            Success = _ok;
            Value = _vv;
            Error = _err;
        }

        public static CGXCResult<TResult> Ok(TResult val) => new CGXCResult<TResult>(true, val, EGXCErrorCode.NONE);

        public static CGXCResult<TResult> Fail(EGXCErrorCode err) => new CGXCResult<TResult>(false, null, err);
    }
}
