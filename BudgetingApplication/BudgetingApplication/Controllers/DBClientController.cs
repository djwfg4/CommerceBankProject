using BudgetingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BudgetingApplication.Controllers
{
    public class DBClientController : Controller
    {
        static DatabaseContext dbContext = new DatabaseContext();
        
        // GET: Client
        public ActionResult Index()
        {
            return View();
        }

        public Client getClient()
        {
           

            Client client = dbContext.Clients.Single(cl => cl.ClientID == 1);
            return client;
        }
    }
}