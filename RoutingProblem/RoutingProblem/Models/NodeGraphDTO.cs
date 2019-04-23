using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingProblem.Models
{
    public class NodeGraphDTO
    {
        public NodeGraphDTO()
        {
            MultiLabelMark = new List<MultiLabelMark>();
        }

        public Node Node { get; set; }
        public NodeGraphDTO PreviousNode { get; set; }
        public double CurrentDistance { get; set; }

        //astar
        public double EstimateDistanceToEnd { get; set; }
        public double FScore { get; set; }

        //bothside dijkster
        public NodeGraphDTO PreviousNodeR { get; set; }
        public double CurrentDistanceR { get; set; }

        //multi label
        public List<MultiLabelMark> MultiLabelMark { get; set; }

        public bool Settled { get; set; }
        public bool Unsettled { get; set; }
        public bool SettledR { get; set; }
        public bool UnsettledR { get; set; }
    }
}
