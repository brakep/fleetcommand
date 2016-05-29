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
//using System.Runtime.Serialization.Json;
//using System.Collections.Specialized.NameValueCollection;

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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key = key.OpenSubKey("Classes", true);
            key = key.CreateSubKey("fleetcommand");
            key.SetValue("", "URL:Fleet Command");
            key.SetValue("URL Protocol", "");
            key = key.CreateSubKey("shell");
            key = key.CreateSubKey("open");
            key = key.CreateSubKey("command");
            string path = "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location+"\" \"%1\"";
            key.SetValue("", path);
        }
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Process.Start("https://login.eveonline.com/oauth/authorize?response_type=code&redirect_uri=fleetcommand://authresponse&client_id=d5fc31ac1be9495c85d2a075c0005b3a&scope=fleetRead&state=ProcessID");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e){
            blabla(keymaster);
            //var jsonTask = GetJsonAsync(keymaster).Result;
            //textBox1.Text = jsonTask;
            //textBox.Text = jsonTask.ToString();
            //textBox.Text = jsonTask.Result;
          //  MessageBox.Show(jsonTask.Result);
        }
        public async void GetFleet(string fleeturi)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://crest-tq.eveonline.com/");
            HttpRequestMessage crestRequest = new HttpRequestMessage(HttpMethod.Get, fleeturi);
            crestRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage crestResponseMessage = await client.SendAsync(crestRequest);
            var response = await crestResponseMessage.Content.ReadAsStringAsync();
            JObject parsedresponse = JObject.Parse(response);
            textBox3.Text += response;
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
            charlistbox.Items.Add(mycharacter.corporation.name + " " + mycharacter.name);

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
                    new KeyValuePair<string, string>("client_id", "d5fc31ac1be9495c85d2a075c0005b3a"),
                    new KeyValuePair<string, string>("client_secret", "YUmc05otcBGFmB0fD0EfSxizLzFOyKvGSLkp0eWr"),
                }
            );
            var myresp = await (await client.SendAsync(request)).Content.ReadAsStringAsync();
            //JObject response = JObject.Parse(await (await client.SendAsync(request)).Content.ReadAsStringAsync());
            textBox1.Text = myresp;
            JObject response = JObject.Parse(myresp);
            //JObject accesresponse = JObject.Parse
            accessToken = response["access_token"].Value<string>();
            textBox2.Text = response["access_token"].Value<string>();


            GetFleet(fleetUrl.Text);

            /*
            HttpRequestMessage crestRequest = new HttpRequestMessage(HttpMethod.Get, "https://crest-tq.eveonline.com/fleets/1085811234913/");
            //HttpRequestMessage crestRequest = new HttpRequestMessage(HttpMethod.Get, "https://crest-tq.eveonline.com/fleets/");
            //crestRequest.Headers.Authorization = new AuthenticationHeaderValue("Authorization", "Bearer" + accessToken);
            crestRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer",  accessToken);
            //crestRequest.Headers.Add("Authorization", "B2earer " + accessToken);
            HttpResponseMessage crestResponseMessage = await client.SendAsync(crestRequest);
            //crestResponseMessage.EnsureSuccessStatusCode();

            //JObject crestResponseJson = JObject.Parse(await crestResponseMessage.Content.ReadAsStringAsync());
            textBox3.Text = await crestResponseMessage.Content.ReadAsStringAsync();
            
            */
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            GetFleet(fleetUrl.Text);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            GetAlliances();
        }

        private async void GetAlliances()
        {
            HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("https://crest-tq.eveonline.com/");
            //HttpRequestMessage crestRequest = new HttpRequestMessage(HttpMethod.Get, "https://crest-tq.eveonline.com/fleets/1085811234913/wings/");
            //HttpRequestMessage crestRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.eveonline.com/eve/AllianceList.xml.aspx");
            HttpRequestMessage crestRequest = new HttpRequestMessage(HttpMethod.Get, "https://public-crest.eveonline.com/alliances/");
            // crestRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage crestResponseMessage = await client.SendAsync(crestRequest);
            allianceList.Text = await crestResponseMessage.Content.ReadAsStringAsync();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            //string mytext = "";
            using (var streamReader = new StreamReader(@"C:\temp\jsonfleet.txt", Encoding.UTF8))
            {
                //mytext = streamReader.ReadToEnd();
                fleetBox.Text = streamReader.ReadToEnd();
                fleetButton.Content = "Loaded";
            }

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
        }

        private void processFleet_Click(object sender, RoutedEventArgs e)
        {
            var model = JsonConvert.DeserializeObject<RootObject>(fleetBox.Text); 
            foreach (var fleetmember in model.items)
            {
                //fleetlist.Items.Add(fleetmember.character.name);
            }




            List<CountShips> countshiplist  = new List<CountShips>();


            var shiplist = new List<StringSplitOptions>();
            foreach (var fleetmember in model.items)
            {
                CountShips doubleitem = countshiplist.Find(x => x.ship == fleetmember.ship.name);
                if (doubleitem == null)
                {
                countshiplist.Add(new CountShips { ship = fleetmember.ship.name, count = 1 });
                }else{
                    doubleitem.count++;
                }
                
            }
            foreach (var myship in countshiplist)
            {
                fleetlist.Items.Add(new CountShips { ship = myship.ship, count = myship.count });
            }

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
            var model = JsonConvert.DeserializeObject<RootObject>(textBox3.Text);
            textBox3.Clear();
            foreach (var fleetmember in model.items)
            {
                ParsFleet(fleetmember.character.href);
            }

        }

        private void parsecharacter_Click(object sender, RoutedEventArgs e)
        {
            //
            textBox3.Clear();
            GetFleet("https://crest-tq.eveonline.com/characters/93883323/");
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            var mycharacter = JsonConvert.DeserializeObject<RootCharacter>(textBox3.Text);
            textBox3.Text = mycharacter.corporation.name + " " + mycharacter.name;

            //JObject response = JObject.Parse(textBox3.Text);
            //textBox3.Text = response["name"].Value<string>();

        }

    }
}
//https://crest-tq.eveonline.com/fleets/1085811234913/
//https://api.eveonline.com/eve/AllianceList.xml.aspx
//https://public-crest.eveonline.com/alliances/