using System;
using System.Collections.Generic;
using System.Text;

namespace CrudeEngine.IO
{
    public class Saveable
    {
        public enum SaveType
        {
            EntityData,
            PlayerPref,
            GameSave,
            GameData,
        }
        public SaveType Type { get; protected set; }
        public void Save()
        {

        }
    }
}
