using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingProblem.Models
{
    public class RoutesDTO
    {
        public RoutesDTO()
        {
            Route = new LinkedList<RouteDTO>();
        }

        public LinkedList<RouteDTO> Route { get; set; }
    }
}
