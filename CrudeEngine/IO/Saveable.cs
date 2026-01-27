using System;
using System.IO;
using Newtonsoft.Json;

namespace CrudeEngine.IO
{
    public enum SaveType
    {
        EntityData,
        PlayerPref,
        GameSave,
        GameData,
    }
    public abstract class Saveable
    {

        [JsonIgnore]
        public SaveType Type { get; protected set; }

        [JsonIgnore]
        public string SaveId { get; protected set; }

        public void SetSaveConfig(SaveType type, string saveId)
        {
            Type = type;
            SaveId = saveId;
        }

        public virtual string Save()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public virtual void Load(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
                return;

            JsonConvert.PopulateObject(jsonData, this, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}
