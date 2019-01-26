using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingProblem.Models
{
    public class Routes
    {
        public Routes()
        {
            Route = new LinkedList<Route>();
        }

        public LinkedList<Route> Route { get; set; }
    }
}
