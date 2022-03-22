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
        private List<PointLatLng> positionList1 = new List<PointLatLng>();      // 1항로 (default)
        private List<PointLatLng> positionList2 = new List<PointLatLng>();      // 2항로

        GMapMarker marker;
        GMapMarker marker1;

        public Form1()
        {
            InitializeComponent();

            this.FormClosed += Form1_FormClosed;

            gMapControl1.CanDragMap = true; //드래그 사용
            gMapControl1.DragButton = MouseButtons.Left; //마우스 왼쪽 버튼이용하여 드래그하겠다

            gMapControl1.Position = new GMap.NET.PointLatLng(48.8589507, 2.2775175); //지도 시작 좌표
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //listView1.View = View.Details;
            //listView1.GridLines = true;
            //listView1.FullRowSelect = true;
            //listView1.Columns.Add("no.", 40, HorizontalAlignment.Center);
            //listView1.Columns.Add("년", 70, HorizontalAlignment.Center);
            //listView1.Columns.Add("월", 70, HorizontalAlignment.Center);
            //listView1.Columns.Add("일", 70, HorizontalAlignment.Center);
            //listView1.Columns.Add("유속", 70, HorizontalAlignment.Center);
            //listView1.Columns.Add("유향", 70, HorizontalAlignment.Center);

            gMapControl1.RoutesEnabled = true;
            gMapControl1.Manager.Mode = AccessMode.ServerAndCache;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;

            gMapControl1.Position = new GMap.NET.PointLatLng(36.39, 127.17);

            gMapControl1.Zoom = 7;

            listView3.View = View.Details;
            listView3.GridLines = true;
            listView3.FullRowSelect = true;


            //listView3.Columns.Add("no. ", 40, HorizontalAlignment.Center);
            //listView3.Columns.Add("날짜", 120, HorizontalAlignment.Center);
            //listView3.Columns.Add("시간", 50, HorizontalAlignment.Center);
            //listView3.Columns.Add("유속", 70, HorizontalAlignment.Center);
            //listView3.Columns.Add("유향", 70, HorizontalAlignment.Center);

        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
           // throw new NotImplementedException();
            Application.Exit();
        }


        //키워드로 검색 버튼  ex)울산시청 이라고 txtSearch에 치면 좌표를 txtLat , txtLong에 보여줌
        private void btnSearch_Click(object sender, EventArgs e)
        {
            gMapControl1.CanDragMap = true;

            position.Zoom = 0;
            position.Manager.Mode = AccessMode.ServerAndCache;
            position.SetPositionByKeywords(txtSearch.Text);      //검색 키워드

            position.Refresh();

            gMapControl1.Position = position.Position;
            gMapControl1.Refresh();

            gMapControl1.RoutesEnabled = true;
            gMapControl1.Manager.Mode = AccessMode.ServerAndCache;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;

           lblPosition.Text = string.Format("위도: {0}, 경도:{1}: ",gMapControl1.Position.Lat, gMapControl1.Position.Lng);

            txtLat.Text = gMapControl1.Position.Lat.ToString();
            txtLng.Text = gMapControl1.Position.Lng.ToString();


            gMapControl1.ShowCenter = true;

            gMapControl1.Zoom = 12;
        }



        // 좌표로 검색 버튼
        private void btnMove_Click(object sender, EventArgs e) // Move 버튼
        {
            var insert = new PointLatLng(); 
            insert.Lat = Convert.ToDouble(txtLat.Text);   //위도
            insert.Lng = Convert.ToDouble(txtLng.Text); //경도

            gMapControl1.Position = insert; //해당 위치로 이동
            gMapControl1.Refresh();

            gMapControl1.MapProvider = GMapProviders.GoogleMap; //GoogleMap 사용 
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache; //어떤 방식으로 검색하는지

            lblPosition.Text = string.Format("위도 {0} : 경도 {1}",gMapControl1.Position.Lat,gMapControl1.Position.Lng);


            gMapControl1.ShowCenter = true;

            gMapControl1.Zoom = 12;
        }

        //Marker 버튼. 지도 하단에 위치 
        private void btnMarker_Click(object sender, EventArgs e) 
        {
            positionList1.Add(gMapControl1.Position); //사용자의 지도 화면 가운데의 좌표
            GMap.NET.WindowsForms.GMapOverlay markers = new GMap.NET.WindowsForms.GMapOverlay("markers"); //오버레이(지도) 만들기
            GMap.NET.WindowsForms.GMapMarker marker = 
                new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                new GMap.NET.PointLatLng(gMapControl1.Position.Lat, gMapControl1.Position.Lng),
                GMap.NET.WindowsForms.Markers.GMarkerGoogleType.blue_pushpin); //마커 정보
            markers.Markers.Add(marker); //마커 만들기

            gMapControl1.Overlays.Add(markers); //오버레이(지도)에 마커 추가
            gMapControl1.Overlays.Add(markers);

            gMapControl1.Zoom++; 
            gMapControl1.Zoom--;

            position.PointToClient(Location);
        }

        // 지도 마우스 오른쪽 버튼 클릭 -> 마커 핀 찍힘
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
                        ToolTipText = "test1",
                        ToolTipMode = MarkerTooltipMode.OnMouseOver
                    };



                marker1 =
                    new GMarkerGoogle(
                        new PointLatLng(center.Lat, 126.9784050),
                        GMarkerGoogleType.blue_pushpin)
                    {
                        ToolTipText = "test2",
                        ToolTipMode = MarkerTooltipMode.OnMouseOver
                    };

                GMapMarker marker2 =
                    new GMarkerGoogle(
                        new PointLatLng(center.Lat + 0.0001, center.Lng),
                        GMarkerGoogleType.blue_pushpin)
                    {
                        ToolTipText = "test3",
                        ToolTipMode = MarkerTooltipMode.OnMouseOver
                    };

                GMapMarker marker3 =
                    new GMarkerGoogle(
                        new PointLatLng(center.Lat - 0.0001, center.Lng),
                        GMarkerGoogleType.blue_pushpin)
                    {
                        ToolTipText = "test4",
                        ToolTipMode = MarkerTooltipMode.OnMouseOver
                    };

                GMapMarker marker4 =
                    new GMarkerGoogle(
                        new PointLatLng(center.Lat, center.Lng),
                        GMarkerGoogleType.blue_pushpin)
                    {
                        ToolTipText = "test5",
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


        // 거리 재는 버튼. 지도 하단에 있음
        private void btnDistance_Click(object sender, EventArgs e) 
        {
            GMapOverlay polygons = new GMapOverlay("polygons"); //지도 그려주는 공간(GMapOverlay)
            GMapPolygon polygon = new GMapPolygon(positionList1, "jardin des Tuileries");//선 만들기

            polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red)); 
            polygon.Stroke = new Pen(Color.Red, 1);


            polygons.Polygons.Add(polygon);
            gMapControl1.Overlays.Add(polygons);
            gMapControl1.Zoom++;
            gMapControl1.Zoom--;

            GMapRoute tablemaproute = new GMapRoute(positionList1, "Ruta ubication");     //List안에 있는 값을 좌표로 인식. 불러오기
            GMapOverlay tablerouteoverlay = new GMapOverlay("Capa de la reta");         //지도 그려주는 공간

            tablerouteoverlay.Routes.Add(tablemaproute);

            gMapControl1.Overlays.Add(tablerouteoverlay);
            gMapControl1.Zoom = gMapControl1.Zoom + 1;
            gMapControl1.Zoom = gMapControl1.Zoom - 1;

            double distance = tablemaproute.Distance; //값들의 거리 계산
            MessageBox.Show(distance.ToString());
        }

        //지도(그림)그래그 포지션 변경. 이벤트 속성
        private void GMapControl1_OnPositionChanged_1(PointLatLng point)
        {
            position.Position = point;
        }


        //지도(그림) 드러그 해서 이동. 이벤트 속성
        private void GMapControl1_OnMapDrag()
        {
            gMapControl1.Refresh();
            gMapControl1.Position = position.Position;      
        }


        //마커 클릭시 MessageBox로 좌표 보여줌
        private void GMapControl1_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            MessageBox.Show("좌표 : " + item.Position.ToString());

            label4.Text = item.Position.ToString();
        }


        //항로 cbox
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        { 
            GMapOverlay routes = new GMapOverlay("routes");   //지도 그리기

            switch (comboBox2.Text)
            {
                case "1항로":
                    gMapControl1.Overlays.Clear();

                    GMapOverlay point1 = new GMapOverlay("markers");

                    positionList2.Clear();

                    //1항로에 있는 중심지점 포인트들, 초록압정
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

                    route1.Stroke = new Pen(Color.Red, 3);  //선 빨간색으로 마크 찍은거 이어줌
                    routes.Routes.Add(route1);

                    gMapControl1.Overlays.Add(routes);    //지도위에 선 그리고
                    gMapControl1.Overlays.Add(point1);    //지도위에 핀 그리고

                    
                    gMapControl1.Position = new GMap.NET.PointLatLng(34.14, 127.47);    //1항로 마커 찍은거 지도 중심에 보여주기

                    gMapControl1.Zoom = 9;

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
                    gMapControl1.Position = new GMap.NET.PointLatLng(34.2231580253797, 127.339782714844);
                    gMapControl1.Zoom = 9;
                    break;

            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           // laTIME.Text = comboBox1.Text;
        }


        // 과거 데이터 조회 버튼
        private void btnSelect_Click(object sender, EventArgs e)
        {
           
            String time = comboBox1.Text.Substring(0, 2);

            SqlConnection sqlcon = new SqlConnection("Server=1.220.15.197; Database=TEST; uid=TEST; pwd=p@ssw0rd!123");

            //DB연결
            sqlcon.Open();

            string strSql_Select = "select year,month,day,currentSpeed,currentDirect from dbo.sample1 where month=" + Month + " and day=" + day + "and time=" + time + " order by year asc";
            string strSql_avg = "select avg(case when currentSpeed is null then 0 else currentSpeed END)평균유속 from dbo.sample1 where month=" + Month + " and day=" + day + " and time=" + time + "";
            // ↑ avg함수로 평균을 구하는데, null값은 0으로 쳐라. avg계산한 칼럼의 별명은 평균유속

            SqlCommand cmd_Select = new SqlCommand(strSql_Select, sqlcon);    //strSql_Select 데이터 검색 쿼리
            SqlCommand cmd_avg = new SqlCommand(strSql_avg, sqlcon);          //strSql_avg 데이터계산(평균유속) 쿼리


            //데이터 검색 쿼리 DataReader Open
            SqlDataReader rd = cmd_Select.ExecuteReader();     
            int n = 0;

            listView1.Items.Clear();
            while (rd.Read())              
            {
                n++;
                ListViewItem item = new ListViewItem(n.ToString());      //첫번째 칼럼 속성에 1,2,3... 찍힘
                item.SubItems.Add(rd["year"].ToString());               // 왜 ListView2처럼 열편집으로 안넣어줬는지?(rd때문인가?)
                item.SubItems.Add(rd["month"].ToString());               //디버깅 하면 년 월 일 유속 유향 보이는데 코드에는 없
                item.SubItems.Add(rd["day"].ToString());    
                item.SubItems.Add(rd["currentSpeed"].ToString()+ " cm/s");
                item.SubItems.Add(rd["currentDirect"].ToString()+ " deg");
                
                listView1.Items.Add(item);
            }
            rd.Close();     //꼭 닫아야함


            //데이터 계산(평균유속) 쿼리 DataReader Open
            SqlDataReader rdd = cmd_avg.ExecuteReader();        
            while (rdd.Read())
            {
                txtavcCurrent.Text = rdd["평균유속"].ToString() + " cm/s";
            }
            rdd.Close();   //꼭 닫아야함

            //DB 닫음
            sqlcon.Close();
        }

        DateTime dt1;
        private void dateTimePicker1_ValueChanged_1(object sender, EventArgs e)//과거데이터
        {
            dt1 = dateTimePicker1.Value;
            year = dt1.Year;
            Month = dt1.Month;
            day = dt1.Day;
        }

        int year, Month, day;
        string point;
        //좌표 cbox
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            point = comboBox3.Text;
        }


        //최근 데이터 조회(국립해양조사원 api사용)
        private void button1_Click(object sender, EventArgs e)
        {

            string ServiceKey = "488cpMHiLM5jeDssAcQug==";
            string ObcCode = "";
      
            switch (comboBox4.Text)   //최근 데이터 조회할 위치<meta> </meta>
            {
                case "마산항":
                    ObcCode = "TW_0085"; break;
                case "부산항신항":
                    ObcCode = "TW_0086"; break;
                case "남해동부":
                    ObcCode = "KG_0025"; break;
                case "제주해협":
                    ObcCode = "KG_0028"; break;
                case "통영항":
                    ObcCode = "TW_0084"; break;
                case "중문해수욕장":
                    ObcCode = "TW_0075"; break;
            }

            //해양관측부이 최신 관측 데이터 api
            string query = "http://www.khoa.go.kr/oceangrid/grid/api/buObsRecent/search.do?ServiceKey=" + ServiceKey + "&ObsCode=" + ObcCode + "&ResultType=xml";

            WebRequest wr = WebRequest.Create(query); //api URL 읽기
            wr.Method = "GET";   //URL에 지정하여 서버로 전달
            WebResponse wrs = wr.GetResponse();
            Stream s = wrs.GetResponseStream();
            StreamReader sr = new StreamReader(s);

            string response = sr.ReadToEnd();         //요청한 값 끝까지 읽어온다


            //xml 파싱

            XmlDocument xd = new XmlDocument();     //데이터 XML문서화 할거임 new
            xd.LoadXml(response);    

            XmlNode xn = xd["result"]["data"];


            //ListViewItem lvi = new ListViewItem(comboBox4.Text);
            ListViewItem lvi = new ListViewItem();

                    lvi.Text = comboBox4.Text;  //관측소명

                    lvi.SubItems.Add(xn["record_time"].InnerText);//관측시간
                    lvi.SubItems.Add(xn["wind_dir"].InnerText + " deg");//풍향
                    lvi.SubItems.Add(xn["wind_speed"].InnerText + " m/s");//풍속
                    lvi.SubItems.Add(xn["current_dir"].InnerText + " deg");//유향
                    lvi.SubItems.Add(xn["current_speed"].InnerText + " m/s");//유속
                    lvi.SubItems.Add(xn["wave_height"].InnerText);//파고

                    listView2.Items.Add(lvi);
                
            }


        //관측소 콤보박스
        //관측소 선택시 지도에 마커 표시
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e) 
        {
            switch (comboBox4.Text)
            {
                case "마산항":
                    
                    gMapControl1.Overlays.Clear();    
                    GMapOverlay point1 = new GMapOverlay("markers");
                    GMarkerGoogle p1 = new GMarkerGoogle(new PointLatLng(35.10319, 128.6318), GMarkerGoogleType.green_big_go);  //★
                    point1.Markers.Add(p1);    //마커 추가
                    gMapControl1.Overlays.Add(point1);  //마커 지도에 보여줌
                    gMapControl1.Position = new GMap.NET.PointLatLng(34.2231580253797, 127.539782714844);  //마커가 위치할 좌표 ★
                    gMapControl1.Zoom = 9;
                    break;
                case "부산항신항":
                    gMapControl1.Overlays.Clear();
                    GMapOverlay point2 = new GMapOverlay("markers");
                    GMarkerGoogle p2 = new GMarkerGoogle(new PointLatLng(35.04378, 128.7618), GMarkerGoogleType.green_big_go);
                    point2.Markers.Add(p2);
                    gMapControl1.Overlays.Add(point2);
                    gMapControl1.Position = new GMap.NET.PointLatLng(34.2231580253797, 127.539782714844);
                    gMapControl1.Zoom = 9;
                    break;

                case "남해동부":
                    gMapControl1.Overlays.Clear();
                    GMapOverlay point3 = new GMapOverlay("markers");
                    GMarkerGoogle p3 = new GMarkerGoogle(new PointLatLng(34.22247, 128.419), GMarkerGoogleType.green_big_go);
                    point3.Markers.Add(p3);
                    gMapControl1.Overlays.Add(point3);
                    gMapControl1.Position = new GMap.NET.PointLatLng(34.2231580253797, 127.539782714844);
                    gMapControl1.Zoom = 9;
                    break;

                case "제주해협":
                    gMapControl1.Overlays.Clear();
                    GMapOverlay point4 = new GMapOverlay("markers");
                    
                    GMarkerGoogle p4 = new GMarkerGoogle(new PointLatLng(33.91181, 126.4921), GMarkerGoogleType.green_big_go);
                    point4.Markers.Add(p4);
                    gMapControl1.Overlays.Add(point4);
                    gMapControl1.Position = new GMap.NET.PointLatLng(34.2231580253797, 127.539782714844);
                    gMapControl1.Zoom = 9;
                    break;
                case "통영항":
                    gMapControl1.Overlays.Clear();
                    GMapOverlay point5 = new GMapOverlay("markers");

                    GMarkerGoogle p5 = new GMarkerGoogle(new PointLatLng(34.77333, 128.46), GMarkerGoogleType.green_big_go);
                    point5.Markers.Add(p5);
                    gMapControl1.Overlays.Add(point5);
                    gMapControl1.Position = new GMap.NET.PointLatLng(34.2231580253797, 127.539782714844);
                    gMapControl1.Zoom = 9;
                    break;
                case "중문해수욕장":
                    gMapControl1.Overlays.Clear();
                    GMapOverlay point6 = new GMapOverlay("markers");

                    GMarkerGoogle p6 = new GMarkerGoogle(new PointLatLng(33.23444, 126.4097), GMarkerGoogleType.green_big_go);
                    point6.Markers.Add(p6);
                    gMapControl1.Overlays.Add(point6);
                    gMapControl1.Position = new GMap.NET.PointLatLng(34.2231580253797, 127.539782714844);
                    gMapControl1.Zoom = 9;
                    break;

            }
        }


        // 실시간 데이터 조회(국립해양조사원 api사용)
        private void button2_Click(object sender, EventArgs e)
        {
            string ObsLat = txtlatt.Text;  //위도
            string ObsLon = txtLon.Text;  //경도

            string query = "http://www.khoa.go.kr/oceangrid/grid/api/romsCurrent/search.do?" +
                "ServiceKey=488cpMHiLM5jeDssAcQug==" +
                "&ObsLon=" + ObsLon + "&ObsLat=" + ObsLat +
                "&ResultType=xml";
            WebRequest wr = WebRequest.Create(query); //api URL 읽기

            wr.Method = "GET";
            WebResponse wrs = wr.GetResponse();
            Stream s = wrs.GetResponseStream();
            StreamReader sr = new StreamReader(s);

            string response = sr.ReadToEnd();

            // XML 파싱
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(response);


            XmlNode xn = xd["result"];

            int n = 0;

            for (int i = 4; i < xn.ChildNodes.Count; i++)
            {

                n++;
                ListViewItem lvi = new ListViewItem();
                // richTextBox1.Text += xn.ChildNodes[i]["current_speed"].InnerText + "\n";


                //richTextBox1.Text = response;
                int date1 = int.Parse(xn.ChildNodes[i]["date"].InnerText);     
                int hour1 = int.Parse(xn.ChildNodes[i]["hour"].InnerText);
                //int current_direct = int.Parse(xn.ChildNodes[i]["current_direct"].InnerText)
                string date2 = date1.ToString();
                string hour2 = hour1.ToString();


                    if (date2 == txtDate.Text)    // 날짜tbox. 입력한 날짜의 실시간 데이터가 있다면
                    {
                        if (hour2 == comboBox5.Text)        // 시간 cbox. 선택한 시간이 실시간 데이터가 있다면
                        {
                            int No = listView3.Items.Count+1;   //listView의 인덱스는 0부터 시작하므로 +1 해줌
                            lvi.Text = No.ToString();     //Item. 

                            lvi.SubItems.Add(xn.ChildNodes[i]["date"].InnerText);   //SubItems
                            lvi.SubItems.Add(xn.ChildNodes[i]["hour"].InnerText);
                            lvi.SubItems.Add(xn.ChildNodes[i]["current_speed"].InnerText);
                            lvi.SubItems.Add(xn.ChildNodes[i]["current_direct"].InnerText);
                            listView3.Items.Add(lvi);

                            break;
                        }
                    }


                string obslat = txtlatt.Text;
                string obslon = txtLon.Text;


                GMapOverlay point1 = new GMapOverlay("markers");
                GMarkerGoogle p1 = new GMarkerGoogle(new PointLatLng(double.Parse(obslat), double.Parse(obslon)), GMarkerGoogleType.lightblue_pushpin);
                point1.Markers.Add(p1);
                gMapControl1.Overlays.Add(point1);
                gMapControl1.Position = new GMap.NET.PointLatLng(double.Parse(obslat), double.Parse(obslon));
                gMapControl1.Zoom = 9;

                
            }
        }

    }
}


