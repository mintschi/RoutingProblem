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
            DopravnaSietContext dopravnaSietContext = new DopravnaSietContext();
            //SpracovanieOSMDat spracovanieOSMDat = new SpracovanieOSMDat();
            //spracovanieOSMDat.SpracovanieXMLDat(dopravnaSietContext);
            PrepareData.PrepareNodesGraph(dopravnaSietContext.Node);
            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
