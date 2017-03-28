using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BudgetingApplication.Controllers
{
    public class BadgesController : Controller
    {
        // GET: Badges
        public ActionResult Index()
        {
            return View();
        }
    }
}