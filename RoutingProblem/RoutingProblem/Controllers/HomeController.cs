using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoutingProblem.Data;
using RoutingProblem.Models;
using RoutingProblem.Services;

namespace RoutingProblem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //DopravnaSietContext dopravnaSietContext = new DopravnaSietContext();
            //dopravnaSietContext.Data.Where(d => d.Active == true).FirstOrDefault().Active = false;
            //dopravnaSietContext.SaveChanges();
            //SpracovanieOSMDat spracovanieOSMDat = new SpracovanieOSMDat();
            //spracovanieOSMDat.SpracovanieXMLDat(dopravnaSietContext, new Models.Data()
            //{
            //    IdData = 2,
            //    Title = "Žilina zakázané manévre",
            //    MinLat = 49.1094,
            //    MinLon = 18.5738,
            //    MaxLat = 49.2826,
            //    MaxLon = 19.0112,
            //    Active = true
            //});
            //Models.Data data = dopravnaSietContext.Data.Where(d => d.Active == true).First();
            //PrepareData.PrepareNodesGraph(dopravnaSietContext.Node.Where(d => d.IdData == data.IdData), dopravnaSietContext.DisabledMovement.Where(d => d.IdData == data.IdData));
            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
