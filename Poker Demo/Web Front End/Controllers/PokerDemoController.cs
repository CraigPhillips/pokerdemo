using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Net.FrozenExports.PokerDemo.WebFrontEnd.Controllers
{
    /// <summary>
    ///     Simple controller used to gather information needed for the poker hand
    ///     input and evaluation pages and then trigger the rendering of those views
    ///     with the relevant data.
    /// </summary>
    public class PokerDemoController : Controller
    {
        /// <summary>
        ///     Retrieves information needed to display the poker hand input page and
        ///     then triggers the rendering of that page.
        /// </summary>
        /// <returns>
        ///     The view, coupled with data needed to render that view, that the MVC framework should 
        ///     use to display the poker hand input page.
        /// </returns>
        public ActionResult PokerInput()
        {
            return View();
        }

    }
}
