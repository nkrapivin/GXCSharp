using System;
using System.Collections.Generic;
using System.Text;

namespace GXCSharp
{
    /// <summary>
    /// A generic OnProgress delegate.
    /// </summary>
    /// <param name="currentAmount">Current amount of stuff transmitted</param>
    /// <param name="totalAmount">Total amount of stuff</param>
    public delegate void DGXCProgress(long currentAmount, long totalAmount);
}
