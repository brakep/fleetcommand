using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Timers;

namespace Fleet_Command
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class AuthToken
	{
        //
		
	}
    public class PilotJson
    {
        [JsonProperty("items")]
        public Pilot Pilot { get; set; }
    }

    public class Pilot
    {
        [JsonProperty("first_name")]
        public string Firstname { get; set; }

        [JsonProperty("last_name")]
        public string Lastname { get; set; }

    }

    class PilotCollection
    {
        public IEnumerable<Piloot> items { get; set; }
    }

    class Piloot
    {
        public Character2 character { get; set; }
    }

    class Character2
    {
        public string name { get; set;}
    }


    public partial class MainWindow : Window
    {
        public System.Windows.Threading.DispatcherTimer dispatcherTimer;
        public System.Windows.Threading.DispatcherTimer refreshtokenTimer;
        //public Stopwatch stopwatch = new Stopwatch();
        public MainWindow()
        {
            InitializeComponent();
            regfleet.updateregister();

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 3);
            dispatcherTimer.Start();
            refreshtokenTimer = new System.Windows.Threading.DispatcherTimer();
            refreshtokenTimer.Tick += new EventHandler(refreshtokenTimer_Tick);
            refreshtokenTimer.Interval = new TimeSpan(0, 15, 0);
            refreshtokenTimer.Start();
            Activate();
            Topmost = true;
            changebox.Items.Clear();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Label2.Content = DateTime.Now.Minute + ":" + DateTime.Now.Second;
            GetFleet(fleetlinkparsed);
            CommandManager.InvalidateRequerySuggested();
        }
        private void refreshtokenTimer_Tick(object sender, EventArgs e)
        {
            refreshtoken();
            CommandManager.InvalidateRequerySuggested();
        }

       
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (processfleetlink(fleetlink.Text))
            {
               Process.Start("https://login.eveonline.com/oauth/authorize?response_type=code&redirect_uri=fleetcommand://authresponse&client_id=d5fc31ac1be9495c85d2a075c0005b3a&scope=fleetRead&state=ProcessID");
            }
        }

        private Boolean processfleetlink(string p)
        {
            //throw new NotImplementedException();
            if (p.Contains(@"https://crest-tq.eveonline.com/fleets/")){
              fleetlinkparsed = p + @"members/";
              return true;
            }else{
                MessageBox.Show(p + " is not a valid fleet link");
                return false;
            }

        }
        private string client_id = "d5fc31ac1be9495c85d2a075c0005b3a";
        private string client_secret = "YUmc05otcBGFmB0fD0EfSxizLzFOyKvGSLkp0eWr";
        private string encoded_key = "ZDVmYzMxYWMxYmU5NDk1Yzg1ZDJhMDc1YzAwMDViM2E6WVVtYzA1b3RjQkdGbUIwZkQwRWZTeGl6THpGT3lLdkdTTGtwMGVXcg==";
        private string fleetlinkparsed;
        private string fleetjson;
        private string crestrespone;
        private string keymaster;
        public string Keymaster
        {
            get
            {
                return keymaster;
            }
            set { 
                NameValueCollection query = HttpUtility.ParseQueryString(new Uri(value).Query);
                string authCode = query.Get("code");
                keymaster = authCode;
                blabla(authCode);
            }
        }
        private string accessToken;
        private string refresh_token;

        private void Button_Click_2(object sender, RoutedEventArgs e){
            blabla(keymaster);
        }

        async Task<string> DoWebRequest(string cresturi)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://crest-tq.eveonline.com/");
            HttpRequestMessage crestRequest = new HttpRequestMessage(HttpMethod.Get, cresturi);
            crestRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage crestResponseMessage = await client.SendAsync(crestRequest);
            string response = await crestResponseMessage.Content.ReadAsStringAsync();
            return response;
        }


        public async void GetFleet(string fleeturi)
        {
            Task<string> task = DoWebRequest(fleeturi);
            fleetjson = await task;
            processFleet(fleetjson);
            //MessageBox.Show(fleetjson);
        }
        public async void ParsFleet(string memberuri)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://crest-tq.eveonline.com/");
            HttpRequestMessage crestRequest = new HttpRequestMessage(HttpMethod.Get, memberuri);
            crestRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage crestResponseMessage = await client.SendAsync(crestRequest);
            var response = await crestResponseMessage.Content.ReadAsStringAsync();
            var mycharacter = JsonConvert.DeserializeObject<RootCharacter>(response);
            //charlistbox.Items.Add(mycharacter.corporation.name + " " + mycharacter.name);

        }
        public async void blabla(string blablabla)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://crest-tq.eveonline.com/");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://login.eveonline.com/oauth/token"));
            request.Content = new FormUrlEncodedContent(
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("code", blablabla),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("client_id", client_id),
                    new KeyValuePair<string, string>("client_secret", client_secret),
                }
            );
            var myresp = await (await client.SendAsync(request)).Content.ReadAsStringAsync();
            JObject response = JObject.Parse(myresp);
            accessToken = response["access_token"].Value<string>();
            refresh_token = response["refresh_token"].Value<string>();
            GetFleet(fleetlinkparsed);

        }


        public async void refreshtoken()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://crest-tq.eveonline.com/");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://login.eveonline.com/oauth/token"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", encoded_key);
            request.Content = new FormUrlEncodedContent(
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", refresh_token),
                }
            );
            var myresp = await (await client.SendAsync(request)).Content.ReadAsStringAsync();
            JObject response = JObject.Parse(myresp);
            accessToken = response["access_token"].Value<string>();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            GetAlliances();
        }

        private async void GetAlliances()
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage crestRequest = new HttpRequestMessage(HttpMethod.Get, "https://public-crest.eveonline.com/alliances/");
            HttpResponseMessage crestResponseMessage = await client.SendAsync(crestRequest);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
        }
        public class SolarSystem
        {
            public string id_str { get; set; }
            public string href { get; set; }
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Capsuleer
        {
            public string href { get; set; }
        }

        public class Character
        {
            public string name { get; set; }
            public bool isNPC { get; set; }
            public string href { get; set; }
            public string id_str { get; set; }
            public int id { get; set; }
            public Capsuleer capsuleer { get; set; }
        }

        public class Ship
        {
            public string id_str { get; set; }
            public string href { get; set; }
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Station
        {
            public string id_str { get; set; }
            public string href { get; set; }
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Item
        {
            public bool takesFleetWarp { get; set; }
            public object squadID { get; set; }
            public SolarSystem solarSystem { get; set; }
            public object wingID { get; set; }
            public string boosterID_str { get; set; }
            public int roleID { get; set; }
            public Character character { get; set; }
            public int boosterID { get; set; }
            public string boosterName { get; set; }
            public string href { get; set; }
            public string squadID_str { get; set; }
            public string roleID_str { get; set; }
            public string roleName { get; set; }
            public Ship ship { get; set; }
            public string joinTime { get; set; }
            public string wingID_str { get; set; }
            public Station station { get; set; }
        }

        public class RootCharacter
        {
            public Corporation corporation { get; set; }
            public string name { get; set; }

        }
        
        
        public class Corporation
        {
            public string name { get; set; }
            public string id_str { get; set; }
        }


        public class RootObject
        {
            public string totalCount_str { get; set; }
            public List<Item> items { get; set; }
            public int pageCount { get; set; }
            public string pageCount_str { get; set; }
            public int totalCount { get; set; }
        }

        public class ShipThing
        {
            public string ship { get; set; }
            public int count { get; set; }
        }

        public class CountShips
        {
            private string _ship;
            public string ship { get { return _ship; } set { _ship = value; } }
            public int count { get; set; }
            public string solarsystem { get; set; }
        }
        
        public List<CountShips> fleetlist = new List<CountShips>();

        public class fleetpilot
        {
            public string name { get; set; }
            public int id { get; set; }
            private string _ship;
            public string ship { 
                get 
                {
                    return _ship;
                } 
                set
                {
                    if (value != _ship) 
                    {
                        oldship = _ship;
                        shipchanged = true;
                        _ship = value;
                        if (value != "Capsule")
                        {
                            shipchanged = false;
                        }
                    }
                    else
                    {
                        shipchanged = false;
                    }
                }
            }
            public string solarsystem { get; set; }
            public Boolean active { get; set; }
            public Boolean shipchanged { get; set; }
            public string oldship { get; set; }

        }

        public List<fleetpilot> totalfleet = new List<fleetpilot>();

        private void processFleet(string jsonstring)
        {
            var model = JsonConvert.DeserializeObject<RootObject>(jsonstring);
            if (model.items != null){
            lvfleet.ItemsSource = null;
            fleetlist.Clear();
            foreach (var fleetmember in model.items)
            {
                fleetpilot findpilot = totalfleet.Find(x => (x.id == fleetmember.character.id));
                if (findpilot == null)
                {
                    totalfleet.Add(new fleetpilot { name = fleetmember.character.name, id = fleetmember.character.id, ship = fleetmember.ship.name, solarsystem = fleetmember.solarSystem.name, active = true });
                }
                else
                {
                    //
                    findpilot.active = true;
                    findpilot.ship = fleetmember.ship.name;
                    findpilot.solarsystem = fleetmember.solarSystem.name;
                }
            }


            foreach (var fleetmember in totalfleet)
            {
                if (fleetmember.active) { 
                    CountShips doubleitem = fleetlist.Find(x => (x.ship == fleetmember.ship) && (x.solarsystem == fleetmember.solarsystem));
                    if (doubleitem == null)
                    {
                        fleetlist.Add(new CountShips { ship = fleetmember.ship, count = 1, solarsystem = fleetmember.solarsystem });
                    }
                    else
                    {
                        if (fleetmember.active)
                        {
                            doubleitem.count++;
                        }
                    }
                    if (fleetmember.shipchanged)
                    {
                        //ListBoxItem s = new ListBoxItem();
                        //ListBoxItem myBox = new ListBoxItem("test");
                    
                       // changebox.Items.Insert(0, myBox);


                        changebox.Items.Insert(0, string.Format("{0:hh:mm:ss}", DateTime.Now) + ">" + fleetmember.solarsystem + " , " + fleetmember.name + ":  " + fleetmember.oldship + " => " + fleetmember.ship);
                        fleetmember.shipchanged = false;
                        //changebox.Items.Add(fleetmember.solarsystem + " , " + fleetmember.name + ":  " + fleetmember.oldship + " => " + fleetmember.ship);
                       // ListBoxItem myitem = (changebox.Items[0] as ListBoxItem);
                        //var listboxitem = (ListBoxItem)changebox.ItemContainerGenerator.ContainerFromItem(changebox.Items[0]);
                        //Color col = (Color)ColorConverter.ConvertFromString("Red");
                        //Brush brush = new SolidColorBrush(col);
                        //listboxitem.Background = brush;
                    }
                }
            }
            fleetlist.Sort((x, y) => y.count.CompareTo(x.count));
            lvfleet.ItemsSource = fleetlist;
            }
            //foreach (var myship in countshiplist)
            //{
                //fleetlist.Items.Add(new CountShips { ship = myship.ship, count = myship.count });
            //}
            
        }
        public class allalliances
        {
            public List<alliances> alliancelist { get; set; }
        }
        public class alliances
        {
            public string name{ get; set; }
            public int allianceID { get; set; }
            public List<memberCorporations> corporations { get; set; }
        }
        public class memberCorporations
        {
            public string name { get; set; }
            public int corporationID { get; set; }
        }
 
        private void loadXML_Click(object sender, RoutedEventArgs e)
        {
            AllianceCorporations allcorp = new AllianceCorporations(@"c:\temp\alliances.txt");
            MessageBox.Show(allcorp.GetAllianceNameAndID("680022174").AllianceName);
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            //var model = JsonConvert.DeserializeObject<RootObject>(textBox3.Text);
            //textBox3.Clear();
            //foreach (var fleetmember in model.items)
            //{
            //    ParsFleet(fleetmember.character.href);
           // }

        }

        private void parsecharacter_Click(object sender, RoutedEventArgs e)
        {
            //
            //textBox3.Clear();
            GetFleet("https://crest-tq.eveonline.com/characters/93883323/");
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            //var mycharacter = JsonConvert.DeserializeObject<RootCharacter>(textBox3.Text);
            //textBox3.Text = mycharacter.corporation.name + " " + mycharacter.name;

            //JObject response = JObject.Parse(textBox3.Text);
            //textBox3.Text = response["name"].Value<string>();

        }

        private void fleetlink_GotFocus(object sender, RoutedEventArgs e)
        {
            fleetlink.Text = "";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GetFleet(fleetlinkparsed);

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            refreshtoken();
        }

    }
}
//https://crest-tq.eveonline.com/fleets/1085811234913/
//https://api.eveonline.com/eve/AllianceList.xml.aspx
//https://public-crest.eveonline.com/alliances/