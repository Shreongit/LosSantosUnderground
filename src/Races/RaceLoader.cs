using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;

namespace LosSantosUnderground.Races
{
    public static class RaceLoader
    {
        public static List<RaceDefinition> LoadRaces()
        {
            string scriptDirectory =
                Path.GetDirectoryName(typeof(RaceLoader).Assembly.Location);

            string path =
                Path.Combine(
                    scriptDirectory ?? AppDomain.CurrentDomain.BaseDirectory,
                    "Data",
                    "Json",
                    "races.json");

            if (!File.Exists(path))
            {
                path = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "scripts",
                    "Data",
                    "Json",
                    "races.json");
            }

            string json = null;

            if (File.Exists(path))
            {
                json = File.ReadAllText(path);
            }
            else
            {
                Assembly assembly = typeof(RaceLoader).Assembly;
                using (Stream stream = assembly.GetManifestResourceStream(
                    "LosSantosUnderground.Data.Json.races.json"))
                {
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                            json = reader.ReadToEnd();
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(json))
                return new List<RaceDefinition>();

            if (string.IsNullOrWhiteSpace(json))
                return new List<RaceDefinition>();

            JavaScriptSerializer serializer =
                new JavaScriptSerializer();

            RaceDataContainer data =
                serializer.Deserialize<RaceDataContainer>(
                    json);

            if (data == null ||
                data.Races == null)
            {
                return new List<RaceDefinition>();
            }

            return data.Races;
        }

        private class RaceDataContainer
        {
            public List<RaceDefinition> Races { get; set; }
                = new List<RaceDefinition>();
        }
    }
}