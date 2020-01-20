using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace PlanetbaseFramework
{

    public interface IModConfig
    {

        bool isActive(string modName);
        void setActive(string modName, bool state);

    }

    public class SimpleFileBasedModConfig : IModConfig
    {

        private string configFile;
        private Dictionary<string, string> modConfigMap = null;

        public SimpleFileBasedModConfig(string configFile)
        {
            this.configFile = configFile;
        }

        private void loadConfig()
        {
            modConfigMap = new Dictionary<string, string>();
            if (!System.IO.File.Exists(configFile))
            {
                Debug.Log("Config file {0} not found. empty config..");
                return;
            }
            System.IO.StreamReader reader = new System.IO.StreamReader(configFile);
            String line = null;
            while ((line = reader.ReadLine()) != null)
            {
                Regex commentRegex = new Regex(@"^#.*");
                Regex propertyRegex = new Regex(@".+=.*");
                if (commentRegex.Match(line).Success || !propertyRegex.Match(line).Success)
                    continue;

                String[] parts = line.Split(new char[] { '=' }, 2);
                modConfigMap.Add(parts[0], (parts.Length > 1) ? parts[1] : "");
            }
            reader.Close();
        }

        private void saveConfig()
        {
            System.IO.StreamWriter writer = new System.IO.StreamWriter(configFile, false);

            foreach (KeyValuePair<string, string> entry in modConfigMap)
            {
                writer.WriteLine("{0}={1}", entry.Key, entry.Value);
            }
            writer.Flush();
            writer.Close();
        }

        public bool isActive(string modName)
        {
            if (modConfigMap == null)
                loadConfig();
            return (modConfigMap.ContainsKey(modName) && modConfigMap[modName].Equals("1"));
        }

        public void setActive(string modName, bool state)
        {
            if (modConfigMap == null)
                loadConfig();

            if (state)
            {
                if (!modConfigMap.ContainsKey(modName))
                    modConfigMap.Add(modName, "1");
            }
            else
            {
                if (modConfigMap.ContainsKey(modName))
                    modConfigMap.Remove(modName);
            }
            saveConfig();
        }

    }

}
