using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.FrozenExports.PokerDemo.WebFrontEnd.Tests.ControllerEvaluation
{
    /// <summary>
    ///     Convenience class to check for proper exceptions when null arguments are
    ///     provided to classes being tested.
    /// </summary>
    public class ArgumentNullExceptionChecker
    {
        public void CheckForArgumentNullException(Action ActionToCheck)
        {
            if(ActionToCheck != null)
            {
                bool exceptionThrown = false;

                try { ActionToCheck(); }
                catch (ArgumentNullException)
                {
                    exceptionThrown = true;
                }

                Assert.IsTrue(exceptionThrown, "No ArgumentNull exception was thrown.");
            }
        }
    }
}
