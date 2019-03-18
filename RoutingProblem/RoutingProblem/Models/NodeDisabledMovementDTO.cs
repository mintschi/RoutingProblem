using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingProblem.Models
{
    public class NodeDisabledMovementDTO : NodeGraphDTO
    {
        public NodeDisabledMovementDTO() : base()
        {
        }

        public Node NodeFirst { get; set; }
    }
}
