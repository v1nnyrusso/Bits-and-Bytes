using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bits_And_Bytes_Vincenzo_Russo.Models;

namespace Bits_And_Bytes_Vincenzo_Russo.Controllers
{
    public class HomeController : Controller
    {
        //Returns index view 
        public ActionResult Index()
        {
           
                return View();
            
        }

        //Returns about view
        public ActionResult About()
        {
           

            return View();
        }


        //Returns terms view
        public ActionResult TermsAndConditions()
        {
            return View();
        }

        //Returns FAQ view
        public ActionResult FAQ()
        {
            return View();
        }
    }
}