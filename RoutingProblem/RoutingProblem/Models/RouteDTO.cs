using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingProblem.Models
{
    public class RouteDTO
    {
        public RouteDTO () {
            Nodes = new LinkedList<NodeLocationDTO>();
        }

        public Int32 PocetHranCesty { get; set; }
        public Int32 PocetNavstivenychHran { get; set; }
        public double DlzkaCesty { get; set; }
        public double CasVypoctu { get; set; }
        public LinkedList<NodeLocationDTO> Nodes { get; set; }
    }
}
