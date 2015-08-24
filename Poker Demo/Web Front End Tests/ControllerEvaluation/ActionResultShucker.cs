using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Net.FrozenExports.PokerDemo.WebFrontEnd.Tests.ControllerEvaluation
{
    /// <summary>
    ///     Removes the outer ActionResultShell and getes the underlying model object
    ///     that was bundled with it.    
    /// </summary>
    public class ActionResultShucker
    {
        /// <summary>
        ///     Removes the outside wrapper from the provided View and returns
        ///     the underlying model object which this object will try to cast to the
        ///     provided type.
        /// </summary>
        /// <typeparam name="T">The type of the underlying model object.</typeparam>
        /// <param name="ResultToShuck">The View which contains the desired object.</param>
        /// <returns>
        ///     The underlying model object if it is present and could be cast appropriately. 
        ///     Otherwise, null.
        /// </returns>
        public T Shuck<T>(ViewResult ResultToShuck) where T:class
        {
            T shuckedModelObject = null;

            if (ResultToShuck != null && ResultToShuck.Model != null)
                shuckedModelObject = ResultToShuck.Model as T;

            return shuckedModelObject;
        }

        /// <summary>
        ///     Removes the outside wrapper from the provided JSON and returns
        ///     the underlying model object which this object will try to cast to the
        ///     provided type.
        /// </summary>
        /// <typeparam name="T">The type of the underlying model object.</typeparam>
        /// <param name="ResultToShuck">The JSON which contains the desired object.</param>
        /// <returns>
        ///     The underlying model object if it is present and could be cast appropriately. 
        ///     Otherwise, null.
        /// </returns>
        public T Shuck<T>(JsonResult ResultToShuck) where T:class
        {
            T shuckedModelObject = null;

            if (ResultToShuck != null && ResultToShuck.Data != null)
                shuckedModelObject = ResultToShuck.Data as T;

            return shuckedModelObject;
        }
    }
}
