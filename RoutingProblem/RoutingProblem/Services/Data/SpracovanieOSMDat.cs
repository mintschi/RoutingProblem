using OsmSharp.Streams;
using RoutingProblem.Models;
using RoutingProblem.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Threading;
using System.Web;
using System.Xml;

namespace RoutingProblem.Services.Data
{
    public class SpracovanieOSMDat
    {
        
        public bool SpracovanieXMLDat(GraphContext db, Models.Data data)
        {
            var nodes = new Dictionary<decimal, Node>();
            var xmlReader = new XmlDocument();
            long[] e2, e3;
            double[] e5;
            bool[] e4;
            var zeroString = "0000000000000";

            //xmlReader.Load("slovakia.osm");
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile("http://www.overpass-api.de/api/xapi?map?bbox="
                    + data.MinLon.ToString(CultureInfo.InvariantCulture) + ","
                    + data.MinLat.ToString(CultureInfo.InvariantCulture) + ","
                    + data.MaxLon.ToString(CultureInfo.InvariantCulture) + ","
                    + data.MaxLat.ToString(CultureInfo.InvariantCulture) + "[@meta]", "data.osm");

                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c" + "osmfilter data.osm --keep=\"highway = motorway = motorway_link = trunk = trunk_link = primary = primary_link = secondary = secondary_link = tertiary = tertiary_link = unclassified = road = residential = service = living_street\" >filtrovane.osm");

                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                string result = proc.StandardOutput.ReadToEnd();
                xmlReader.Load("filtrovane.osm");
            }

            XmlNodeList documentNodeList = xmlReader.DocumentElement.SelectNodes("/osm/node");
            XmlNodeList documentWayList = xmlReader.DocumentElement.SelectNodes("/osm/way");

            if (documentNodeList.Count > 0)
            {
                foreach (XmlNode row in documentNodeList)
                {
                    Node node = new Node();
                    node.IdNode = Convert.ToDecimal(data.IdData + zeroString.Substring(row.Attributes["id"].Value.Length, 13 - row.Attributes["id"].Value.Length) + row.Attributes["id"].Value);
                    node.Lat = double.Parse(row.Attributes["lat"].Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                    node.Lon = double.Parse(row.Attributes["lon"].Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                    nodes.Add(node.IdNode, node);
                }
            }

            int k = 0;
            e2 = new long[2500000];
            e3 = new long[2500000];
            e4 = new bool[2500000];
            e5 = new double[2500000];
            if (documentWayList.Count > 0)
            {
                decimal id_disabledM = 0;

                foreach (XmlNode row in documentWayList)
                {
                    long end_node = -1;
                    bool oneway = false;
                    bool highway = false;

                    for (int i = row.ChildNodes.Count - 1; i >= 0; i--)
                    {
                        if (row.ChildNodes[i].Name.Equals("tag") &&
                            row.ChildNodes[i].Attributes["k"].Value.Equals("building") &&
                            row.ChildNodes[i].Attributes["v"].Value.Equals("yes"))
                        {
                            break;
                        }

                        if (row.ChildNodes[i].Name.Equals("tag") &&
                            row.ChildNodes[i].Attributes["k"].Value.Equals("highway"))
                        {
                            highway = true;
                        }

                        if (row.ChildNodes[i].Name.Equals("tag") &&
                            row.ChildNodes[i].Attributes["k"].Value.Equals("highway") &&
                            (row.ChildNodes[i].Attributes["v"].Value.Equals("pedestrian") ||
                            row.ChildNodes[i].Attributes["v"].Value.Equals("footway") ||
                            row.ChildNodes[i].Attributes["v"].Value.Equals("bridleway") ||
                            row.ChildNodes[i].Attributes["v"].Value.Equals("steps") ||
                            row.ChildNodes[i].Attributes["v"].Value.Equals("path") ||
                            row.ChildNodes[i].Attributes["v"].Value.Equals("track") ||
                            row.ChildNodes[i].Attributes["v"].Value.Equals("cycleway")))
                        {
                            break;
                        }

                        if (row.ChildNodes[i].Name.Equals("tag") &&
                            row.ChildNodes[i].Attributes["k"].Value.Equals("oneway") &&
                            row.ChildNodes[i].Attributes["v"].Value.Equals("yes"))
                        {
                            oneway = true;
                        }

                        if (row.ChildNodes[i].Name.Equals("nd"))
                        {
                            if (!highway)
                            {
                                break;
                            }
                            if (end_node == -1)
                            {
                                var id = row.ChildNodes[i].Attributes["ref"].Value;
                                end_node = Convert.ToInt64(data.IdData + zeroString.Substring(id.Length, 13 - id.Length) + id);
                            }
                            else
                            {
                                var id = row.ChildNodes[i].Attributes["ref"].Value;
                                e2[k] = Convert.ToInt64(data.IdData + zeroString.Substring(id.Length, 13 - id.Length) + id);
                                e3[k] = end_node;
                                e5[k] = Utils.Distance(nodes[e2[k]], nodes[e3[k]]);

                                if (!oneway)
                                {
                                    e4[k] = true;
                                } else
                                {
                                    e4[k] = false;
                                }

                                end_node = e2[k];
                                k++;
                            }
                        }
                    }
                }
            }

            xmlReader = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            NaplnenieDatabazy(data, nodes, e2, e3, e4, e5, k);
            return true;
        }

        private void NaplnenieDatabazy(Models.Data data, Dictionary<decimal, Node> nodes, long[] e2, long[] e3, bool[] e4, double[] e5, int k)
        {
            var dtData = new DataTable();
            dtData.Columns.Add("id_data");
            dtData.Columns.Add("title");
            dtData.Columns.Add("min_lat");
            dtData.Columns.Add("min_lon");
            dtData.Columns.Add("max_lat");
            dtData.Columns.Add("max_lon");
            dtData.Columns.Add("active");

            dtData.Rows.Add(data.IdData, data.Title, data.MinLat, data.MinLon, data.MaxLat, data.MaxLon, data.Active);

            var dtNode = new DataTable();
            dtNode.Columns.Add("id_node");
            dtNode.Columns.Add("id_data");
            dtNode.Columns.Add("lat");
            dtNode.Columns.Add("lon");

            foreach (KeyValuePair<decimal, Node> n in nodes)
            {
                dtNode.Rows.Add(n.Key, data.IdData, n.Value.Lat, n.Value.Lon);
            }


            var dtEdge = new DataTable();
            dtEdge.Columns.Add("id_edge");
            dtEdge.Columns.Add("id_data");
            dtEdge.Columns.Add("start_node");
            dtEdge.Columns.Add("end_node");
            dtEdge.Columns.Add("distance_in_meters");

            for (int j = 0; j < k; j++)
            {
                dtEdge.Rows.Add(null, data.IdData, e2[j], e3[j], e5[j]);
                if (e4[j])
                {
                    dtEdge.Rows.Add(null, data.IdData, e3[j], e2[j], e5[j]);
                }
            }

            using (SqlConnection connection = new SqlConnection("Server=mainpc\\sqlexpress;Database=DopravnaSiet;Trusted_Connection=True;"))
            {
                using (var sqlBulk = new SqlBulkCopy(connection))
                {
                    connection.Open();
                    sqlBulk.BulkCopyTimeout = 100;
                    sqlBulk.BatchSize = 100000;
                    sqlBulk.DestinationTableName = "Data";
                    sqlBulk.WriteToServer(dtData);
                    sqlBulk.DestinationTableName = "Node";
                    sqlBulk.WriteToServer(dtNode);
                    sqlBulk.DestinationTableName = "Edge";
                    sqlBulk.WriteToServer(dtEdge);
                    connection.Close();
                }
            }
        }
    }
}