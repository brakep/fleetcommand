using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fleet_Command
{ 
    class AllianceNameID
    { 
        public AllianceNameID(string name,string id)
        {
            alliancename = name;
            allianceid = id;
        }
        private string alliancename;
        private string allianceid;
        public string AllianceName { get {return alliancename;}  }
        public string AllianceID { get { return allianceid; } }
    }
    
    class AllianceCorporations
    {
        public string GetAllianceName(string corporationid)
        {
            string value = corporations[corporationid];
            return alliances[value];
        }
        public AllianceNameID GetAllianceNameAndID(string corporationid)
        {
            return (new AllianceNameID(alliances[corporations[corporationid]], corporations[corporationid]));
        }
        
        public AllianceCorporations(string alliancexml)
        {
            loadalliances(alliancexml);
        }
        private Dictionary<string, string> alliances = new Dictionary<string, string>();
        private Dictionary<string, string> corporations = new Dictionary<string, string>();

        private void loadalliances(string alliancexmldir)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(alliancexmldir);
            XmlElement root = doc.DocumentElement;
            foreach (XmlNode resultnode in root.ChildNodes)
            {
                if (resultnode.Name == "result")
                {
                    foreach (XmlNode alliancelist in resultnode.ChildNodes)
                    foreach (XmlNode alliance in alliancelist.ChildNodes)
                    {
                        alliances.Add(alliance.Attributes["allianceID"].Value, alliance.Attributes["name"].Value);
                        foreach (XmlNode corporationlist in alliance.ChildNodes)
                        foreach (XmlNode corporation in corporationlist.ChildNodes)
                        {
                            corporations.Add(corporation.Attributes["corporationID"].Value, alliance.Attributes["allianceID"].Value);
                        }
                    }
                }
            }
        }
    }
}
