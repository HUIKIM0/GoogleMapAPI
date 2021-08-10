using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.MapProviders;
using GMap;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.Windows;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Linq;
using System.Reflection.Metadata;
using System.Security.Permissions;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Security.Policy;
using System.Xml;

namespace GoogleMapApi
{
    public partial class Form1 : Form
    {
        GMapControl position = new GMapControl();
        private List<PointLatLng> positionList1 = new List<PointLatLng>();
        private List<PointLatLng> positionList2 = new List<PointLatLng>();

        GMapMarker marker;
        GMapMarker marker1;
        public Form1()
        {
            InitializeComponent();




            gMapControl1.CanDragMap = true; //드래그 사용
            gMapControl1.DragButton = MouseButtons.Left; //마우스 왼쪽 버튼이용

            gMapControl1.Position = new GMap.NET.PointLatLng(48.8589507, 2.2775175);
        }


        private void btnMove_Click(object sender, EventArgs e)
        {
            var insert = new PointLatLng();
            insert.Lat = Convert.ToDouble(txtLat.Text);
            insert.Lng = Convert.ToDouble(txtLong.Text);

            gMapControl1.Position = insert;
            gMapControl1.Refresh();

            gMapControl1.MapProvider = GMapProviders.GoogleMap; //GoogleMap 사용
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache; //어떤 방식으로 검색하는지

            Console.WriteLine(gMapControl1.Position.Lat + ";" + gMapControl1.Position.Lng);



            gMapControl1.ShowCenter = true;

            //gMapControl1.Zoom = 12;


        }

        private void btnMarker_Click(object sender, EventArgs e)
        {
            positionList1.Add(gMapControl1.Position);
            GMap.NET.WindowsForms.GMapOverlay markers = new GMap.NET.WindowsForms.GMapOverlay("markers");
            GMap.NET.WindowsForms.GMapMarker marker =
                new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                new GMap.NET.PointLatLng(gMapControl1.Position.Lat, gMapControl1.Position.Lng),
                GMap.NET.WindowsForms.Markers.GMarkerGoogleType.blue_pushpin);
            markers.Markers.Add(marker);

            gMapControl1.Overlays.Add(markers);
            gMapControl1.Overlays.Add(markers);

            gMapControl1.Zoom++;
            gMapControl1.Zoom--;

            position.PointToClient(Location);



        }

        private void GMapControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                var center = gMapControl1.Position;

                positionList1.Add(gMapControl1.Position);

                GMapOverlay markers = new GMapOverlay("markers");
                marker =
                    new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                        new PointLatLng(center.Lat, center.Lng - 0.0001),
                        GMarkerGoogleType.blue_pushpin)
                    {
                        ToolTipText = "test",
                        ToolTipMode = MarkerTooltipMode.OnMouseOver
                    };


                marker1 =
                    new GMarkerGoogle(
                        new PointLatLng(center.Lat, 126.9784050),
                        GMarkerGoogleType.blue_pushpin)
                    {
                        ToolTipText = "test",
                        ToolTipMode = MarkerTooltipMode.OnMouseOver
                    };

                GMapMarker marker2 =
                    new GMarkerGoogle(
                        new PointLatLng(center.Lat + 0.0001, center.Lng),
                        GMarkerGoogleType.blue_pushpin)
                    {
                        ToolTipText = "test",
                        ToolTipMode = MarkerTooltipMode.OnMouseOver
                    };

                GMapMarker marker3 =
                    new GMarkerGoogle(
                        new PointLatLng(center.Lat - 0.0001, center.Lng),
                        GMarkerGoogleType.blue_pushpin)
                    {
                        ToolTipText = "test",
                        ToolTipMode = MarkerTooltipMode.OnMouseOver
                    };

                GMapMarker marker4 =
                    new GMarkerGoogle(
                        new PointLatLng(center.Lat, center.Lng),
                        GMarkerGoogleType.blue_pushpin)
                    {
                        ToolTipText = "test",
                        ToolTipMode = MarkerTooltipMode.OnMouseOver
                    };

                markers.Markers.Add(marker);
                markers.Markers.Add(marker1);
                markers.Markers.Add(marker2);
                markers.Markers.Add(marker3);
                markers.Markers.Add(marker4);

                gMapControl1.Overlays.Add(markers);

                gMapControl1.Zoom = gMapControl1.Zoom + 1;
                gMapControl1.Zoom = gMapControl1.Zoom - 1;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            gMapControl1.CanDragMap = true;

            position.Zoom = 0;
            position.Manager.Mode = AccessMode.ServerAndCache;
            position.SetPositionByKeywords(txtSearch.Text);

            position.Refresh();

            gMapControl1.Position = position.Position;
            gMapControl1.Refresh();

            gMapControl1.RoutesEnabled = true;
            gMapControl1.Manager.Mode = AccessMode.ServerAndCache;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;

            Console.WriteLine(gMapControl1.Position.Lat + " : " + gMapControl1.Position.Lng);

            gMapControl1.ShowCenter = true;

            gMapControl1.Zoom = 12;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            GMapOverlay polygons = new GMapOverlay("polygons");
            GMapPolygon polygon = new GMapPolygon(positionList1, "jardin des Tuileries");

            polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
            polygon.Stroke = new Pen(Color.Red, 1);

            polygons.Polygons.Add(polygon);
            gMapControl1.Overlays.Add(polygons);
            gMapControl1.Zoom++;
            gMapControl1.Zoom--;

            GMapRoute tablemaproute = new GMapRoute(positionList1, "Ruta ubication");

            GMapOverlay tablerouteoverlay = new GMapOverlay("Capa de la reta");

            tablerouteoverlay.Routes.Add(tablemaproute);

            gMapControl1.Overlays.Add(tablerouteoverlay);
            gMapControl1.Zoom = gMapControl1.Zoom + 1;
            gMapControl1.Zoom = gMapControl1.Zoom - 1;


            double distance = tablemaproute.Distance;
            MessageBox.Show(distance.ToString());


        }

        private void GMapControl1_OnPositionChanged_1(PointLatLng point)
        {
            position.Position = point;
        }

        private void GMapControl1_OnMapDrag()
        {
            gMapControl1.Refresh();
            gMapControl1.Position = position.Position;
        }

        private void GMapControl1_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {

            MessageBox.Show("좌표 : " + item.Position.ToString());

            label4.Text = item.Position.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("no.", 40, HorizontalAlignment.Center);
            listView1.Columns.Add("년", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("월", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("일", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("유속", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("유향", 70, HorizontalAlignment.Center);

            

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

            GMapOverlay routes = new GMapOverlay("routes");




            switch (comboBox2.Text)
            {
                case "1항로":
                    gMapControl1.Overlays.Clear();
                    GMapOverlay point1 = new GMapOverlay("markers");


                    positionList2.Clear();


                    GMarkerGoogle p1 = new GMarkerGoogle(new PointLatLng(34.9976333, 128.4639194), GMarkerGoogleType.green_pushpin);
                    GMarkerGoogle p2 = new GMarkerGoogle(new PointLatLng(34.97354167, 128.4814028), GMarkerGoogleType.green_pushpin);
                    GMarkerGoogle p3 = new GMarkerGoogle(new PointLatLng(35.02056389, 128.6297972), GMarkerGoogleType.green_pushpin);
                    GMarkerGoogle p4 = new GMarkerGoogle(new PointLatLng(35.05329444, 128.6965389), GMarkerGoogleType.green_pushpin);
                    GMarkerGoogle p5 = new GMarkerGoogle(new PointLatLng(35.02176111, 128.7906333), GMarkerGoogleType.green_pushpin);
                    GMarkerGoogle p6 = new GMarkerGoogle(new PointLatLng(34.81855278, 128.8614861), GMarkerGoogleType.green_pushpin);
                    GMarkerGoogle p7 = new GMarkerGoogle(new PointLatLng(34.60358889, 128.6940722), GMarkerGoogleType.green_pushpin);
                    GMarkerGoogle p8 = new GMarkerGoogle(new PointLatLng(33.70414722, 127.1455694), GMarkerGoogleType.green_pushpin);
                    GMarkerGoogle p9 = new GMarkerGoogle(new PointLatLng(33.54196944, 126.1568111), GMarkerGoogleType.green_pushpin);
                    GMarkerGoogle p10 = new GMarkerGoogle(new PointLatLng(33.3496, 126.1224667), GMarkerGoogleType.green_pushpin);


                    point1.Markers.Add(p1);
                    positionList1.Add(p1.Position);

                    point1.Markers.Add(p2);
                    positionList1.Add(p2.Position);
                    point1.Markers.Add(p3);
                    positionList1.Add(p3.Position);
                    point1.Markers.Add(p4);
                    positionList1.Add(p4.Position);
                    point1.Markers.Add(p5);
                    positionList1.Add(p5.Position);
                    point1.Markers.Add(p6);
                    positionList1.Add(p6.Position);
                    point1.Markers.Add(p7);
                    positionList1.Add(p7.Position);
                    point1.Markers.Add(p8);
                    positionList1.Add(p8.Position);
                    point1.Markers.Add(p9);
                    positionList1.Add(p9.Position);
                    point1.Markers.Add(p10);
                    positionList1.Add(p10.Position);

                    GMapRoute route1 = new GMapRoute(positionList1, "1항로");
                    route1.Stroke = new Pen(Color.Red, 3);
                    routes.Routes.Add(route1);
                    gMapControl1.Overlays.Add(routes);

                    gMapControl1.Overlays.Add(point1);



                    break;
                case "2항로":
                    gMapControl1.Overlays.Clear();

                    positionList1.Clear();

                    GMapOverlay point2 = new GMapOverlay("markers");
                    GMarkerGoogle o1 = new GMarkerGoogle(new PointLatLng(34.99763333, 128.4639194), GMarkerGoogleType.red_pushpin);
                    GMarkerGoogle o2 = new GMarkerGoogle(new PointLatLng(34.97354167, 128.4814028), GMarkerGoogleType.red_pushpin);
                    GMarkerGoogle o3 = new GMarkerGoogle(new PointLatLng(34.922175, 128.47735), GMarkerGoogleType.red_pushpin);
                    GMarkerGoogle o4 = new GMarkerGoogle(new PointLatLng(34.91025556, 128.4815444), GMarkerGoogleType.red_pushpin);
                    GMarkerGoogle o5 = new GMarkerGoogle(new PointLatLng(34.89288889, 128.4782444), GMarkerGoogleType.red_pushpin);
                    GMarkerGoogle o6 = new GMarkerGoogle(new PointLatLng(34.87413056, 128.4696056), GMarkerGoogleType.red_pushpin);
                    GMarkerGoogle o7 = new GMarkerGoogle(new PointLatLng(34.85554722, 128.4724611), GMarkerGoogleType.red_pushpin);
                    GMarkerGoogle o8 = new GMarkerGoogle(new PointLatLng(34.83441389, 128.4480111), GMarkerGoogleType.red_pushpin);
                    GMarkerGoogle o9 = new GMarkerGoogle(new PointLatLng(34.47875833, 128.4072528), GMarkerGoogleType.red_pushpin);
                    GMarkerGoogle o10 = new GMarkerGoogle(new PointLatLng(33.70414722, 127.1455694), GMarkerGoogleType.red_pushpin);
                    GMarkerGoogle o11 = new GMarkerGoogle(new PointLatLng(33.54196944, 126.1568111), GMarkerGoogleType.red_pushpin);
                    GMarkerGoogle o12 = new GMarkerGoogle(new PointLatLng(33.3496, 126.1224667), GMarkerGoogleType.red_pushpin);

                    point2.Markers.Add(o1);

                    positionList2.Add(o1.Position);
                    point2.Markers.Add(o2);
                    positionList2.Add(o2.Position);
                    point2.Markers.Add(o3);
                    positionList2.Add(o3.Position);
                    point2.Markers.Add(o4);
                    positionList2.Add(o4.Position);
                    point2.Markers.Add(o5);
                    positionList2.Add(o5.Position);
                    point2.Markers.Add(o6);
                    positionList2.Add(o6.Position);
                    point2.Markers.Add(o7);
                    positionList2.Add(o7.Position);
                    point2.Markers.Add(o8);
                    positionList2.Add(o8.Position);
                    point2.Markers.Add(o9);
                    positionList2.Add(o9.Position);
                    point2.Markers.Add(o10);
                    positionList2.Add(o10.Position);
                    point2.Markers.Add(o11);
                    positionList2.Add(o11.Position);
                    point2.Markers.Add(o12);
                    positionList2.Add(o12.Position);

                    gMapControl1.Overlays.Add(point2);

                    GMapRoute route2 = new GMapRoute(positionList2, "2항로");
                    route2.Stroke = new Pen(Color.Yellow, 3);
                    routes.Routes.Add(route2);
                    gMapControl1.Overlays.Add(routes);


                    break;

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           // laTIME.Text = comboBox1.Text;
        }


        private void btnSelect_Click(object sender, EventArgs e)
        {
           
            String time = comboBox1.Text.Substring(0, 2);


            SqlConnection sqlcon = new SqlConnection("Server=1.220.15.197; Database=TEST; uid=TEST; pwd=p@ssw0rd!123");

            sqlcon.Open();

            string strSql_Select = "select 년도,월,일, 유속, 유향 from dbo.표층유속$ where 월=" + Month + " and 일=" + day + "and 시간=" + time + "order by 년도 asc";
         

            SqlCommand cmd_Select = new SqlCommand(strSql_Select, sqlcon);
      

            SqlDataReader rd = cmd_Select.ExecuteReader();
            

            int n = 0;
            
            

            listView1.Items.Clear();
            while (rd.Read())
            {
                n++;
                ListViewItem item = new ListViewItem(n.ToString());
                item.SubItems.Add(rd["년도"].ToString());
                item.SubItems.Add(rd["월"].ToString());
                item.SubItems.Add(rd["일"].ToString());
                item.SubItems.Add(rd["유속"].ToString());
                item.SubItems.Add(rd["유향"].ToString());
                listView1.Items.Add(item);
                
            
                
            }
            
            rd.Close();
        
            sqlcon.Close();

            

        }
        int year, Month, day;
        string point;
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            point = comboBox3.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ServiceKey = "488cpMHiLM5jeDssAcQug==";
            string ObcCode = comboBox4.Text;


            string query = "http://www.khoa.go.kr/oceangrid/grid/api/buObsRecent/search.do?ServiceKey=" + ServiceKey + "&ObsCode=" + ObcCode + "&ResultType=xml";

            WebRequest wr = WebRequest.Create(query); //api URL 읽기
            wr.Method = "GET";

            WebResponse wrs = wr.GetResponse();
            Stream s = wrs.GetResponseStream();
            StreamReader sr = new StreamReader(s);

            string response = sr.ReadToEnd();


            //xml 파싱

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(response);

            XmlNode xn = xd["result"]["data"];
            ListViewItem lvi = new ListViewItem();

            switch (comboBox4.Text)
            {
                case "TW_0085":
                    lvi.Text = "마산항";
                    lvi.SubItems.Add(xn["record_time"].InnerText);//관측시간
                    lvi.SubItems.Add(xn["wind_dir"].InnerText + " deg");//풍향
                    lvi.SubItems.Add(xn["wind_speed"].InnerText + " m/s");//풍속
                    lvi.SubItems.Add(xn["current_dir"].InnerText + " deg");//유향
                    lvi.SubItems.Add(xn["current_speed"].InnerText + " cm/s");//유속
                    lvi.SubItems.Add(xn["wave_height"].InnerText);//파고

                    listView2.Items.Add(lvi);
                    break;
                case "TW_0086":
                    lvi.Text = "부산항신항";
                    lvi.SubItems.Add(xn["record_time"].InnerText);//관측시간
                    lvi.SubItems.Add(xn["wind_dir"].InnerText + " deg");//풍향
                    lvi.SubItems.Add(xn["wind_speed"].InnerText + " m/s");//풍속
                    lvi.SubItems.Add(xn["current_dir"].InnerText + " deg");//유향
                    lvi.SubItems.Add(xn["current_speed"].InnerText + " cm/s");//유속
                    lvi.SubItems.Add(xn["wave_height"].InnerText);//파고

                    listView2.Items.Add(lvi);
                    break;
                case "KG_0025":
                    lvi.Text = "남해동부";
                    lvi.SubItems.Add(xn["record_time"].InnerText);//관측시간
                    lvi.SubItems.Add(xn["wind_dir"].InnerText + " deg");//풍향
                    lvi.SubItems.Add(xn["wind_speed"].InnerText + " m/s");//풍속
                    lvi.SubItems.Add(xn["current_dir"].InnerText + " deg");//유향
                    lvi.SubItems.Add(xn["current_speed"].InnerText + " cm/s");//유속
                    lvi.SubItems.Add(xn["wave_height"].InnerText);//파고

                    listView2.Items.Add(lvi);
                    break;
                case "KG_0028":
                    lvi.Text = "제주해협";
                    lvi.SubItems.Add(xn["record_time"].InnerText);//관측시간
                    lvi.SubItems.Add(xn["wind_dir"].InnerText + " deg");//풍향
                    lvi.SubItems.Add(xn["wind_speed"].InnerText + " m/s");//풍속
                    lvi.SubItems.Add(xn["current_dir"].InnerText + " deg");//유향
                    lvi.SubItems.Add(xn["current_speed"].InnerText + " cm/s");//유속
                    lvi.SubItems.Add(xn["wave_height"].InnerText);//파고

                    listView2.Items.Add(lvi);
                    break;
            }
            }

        DateTime dt1;
        private void dateTimePicker1_ValueChanged_1(object sender, EventArgs e)
        {
            dt1 = dateTimePicker1.Value;
            year = dt1.Year;
            Month = dt1.Month;
            day = dt1.Day;
        }


    }
}


