using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Net.FrozenExports.PokerDemo.WebFrontEnd
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Root",
                url: "",
                defaults: new { controller = "PokerDemo", action = "PokerInput" }
            );

            routes.MapRoute(
                name: "Poker Hand Evaluation",
                url: "pokerevaluation",
                defaults: new { controller = "PokerDemo", action = "PokerEvaluation" }
            );

            routes.MapRoute(
                name: "Card Hands",
                url: "cardhands",
                defaults: new { 
                    controller = "CardHands", 
                    action = "GeneratedCardHands",
                    handCount = 2,
                    cardsPerHand = 5
                }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { 
                    controller = "PokerDemo", 
                    action = "PokerInput", 
                    id = UrlParameter.Optional }
            );
        }
    }
}