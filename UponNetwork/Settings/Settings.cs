using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UponNetwork.Settings;

namespace Microcoin.Settings
{
    [Serializable]
    public class Settings
    {
        public UponNetworkSettings PeerNetworkSettings { get; set; }
        public BlockchainSettings BlockchainSettings { get; set; }

        public Settings() { 
            PeerNetworkSettings = new UponNetworkSettings();
            BlockchainSettings = new BlockchainSettings();
        }

        public static Settings LoadOrCreateSettingsFile(string filePath)
        {
            Settings settings;
            var dir = System.AppDomain.CurrentDomain.BaseDirectory;
            string fullFilePath = Path.Combine(dir, filePath);
            if (!File.Exists(fullFilePath))
            {
                settings = new Settings();
                settings.SaveSettingsToFile(fullFilePath);
                return settings;
            }
            else
            {
                settings = Settings.LoadSettingsFromFile(fullFilePath);
                return settings;
            }
        }

        public static Settings LoadSettingsFromFile( string filePath )
        {
            if( !File.Exists( filePath ) ) 
                throw new FileNotFoundException( filePath );

            try
            {
                Settings settings;
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                using (FileStream file = File.OpenRead(filePath))
                    settings = (Settings)serializer.Deserialize(file);
                return settings;
            } catch (InvalidOperationException exception) {
                throw new ApplicationException($"{filePath} wrong syntax error");
            }
        }

        public void SaveSettingsToFile( string filePath )
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            using (FileStream file = File.Open(filePath, FileMode.Create))
                serializer.Serialize(file, this);
        }
    }
}
