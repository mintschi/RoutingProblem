using RoutingProblem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingProblem.Services
{
    public class Utils
    {
        private const double RADIUS_EARTH_IN_METERS = 6369628.75;
        public static Int32 PocetNavstivenychHran { get; set; }

        public static double Vzdialenost(double startLat, double startLon, double endLat, double endLon)
        {
            double dLat = ConvertDegreesToRadians(endLat - startLat);
            double dLng = ConvertDegreesToRadians(endLon - startLon);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                    + Math.Cos(ConvertDegreesToRadians(startLat)) * Math.Cos(ConvertDegreesToRadians(endLat))
                    * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return (RADIUS_EARTH_IN_METERS * c);
        }

        public static double Distance(Node startNode, Node endNode)
        {
            double dLat = ConvertDegreesToRadians(endNode.Lat - startNode.Lat);
            double dLng = ConvertDegreesToRadians(endNode.Lon - startNode.Lon);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                    + Math.Cos(ConvertDegreesToRadians(startNode.Lat)) * Math.Cos(ConvertDegreesToRadians(endNode.Lat))
                    * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return (RADIUS_EARTH_IN_METERS * c);
        }

        private static double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }
    }
}
