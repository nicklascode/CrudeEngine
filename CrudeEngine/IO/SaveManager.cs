using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using CrudeEngine.SceneSystem;
using CrudeEngine.ECS;

namespace CrudeEngine.IO
{
    public class SaveManager
    {
        public static string GAME_SAVES_LOCATION = Path.Combine(AppContext.BaseDirectory, "Saves"); 
        public static long SAVE_VERSION = 1;

        struct SaveGameData
        {
            public string Name;
            public long Version;

            public SaveGameData(string name, long version) { Name = name; Version = version; }
        }

        public static void SaveGame(string saveName)
        {
            SaveGameData saveGame = new SaveGameData(saveName, SAVE_VERSION);
            string mainSaveData = JsonConvert.SerializeObject(saveGame);

            string saveGameDir = Path.Combine(GAME_SAVES_LOCATION, saveName);
            Directory.CreateDirectory(saveGameDir);

            foreach (SaveType saveType in Enum.GetValues<SaveType>().Where(s => s != SaveType.PlayerPref))
            {
                string saveTypePath = Path.Combine(saveGameDir, saveType.ToString());
                Directory.CreateDirectory(saveTypePath);

                // Save entities
                if(saveType == SaveType.EntityData)
                {
                    // Get current scene and call save on all saveable entities
                    Scene current = Game.Instance.GetCurrentScene();
                    if (current == null)
                        throw new Exception("Current scene is null, thus there is nothin to save");

                    foreach (Entity entity in current.GetEntities())
                    {
                        SaveEntity(saveTypePath, entity);
                    }
                }
            }

            string mainSaveFilePath = Path.Combine(saveGameDir, "save.json");
            File.WriteAllText(mainSaveFilePath, mainSaveData);
        }

        public static void LoadGame(string saveName)
        {
            string saveGameDir = Path.Combine(GAME_SAVES_LOCATION, saveName);
            string mainSaveFilePath = Path.Combine(saveGameDir, "save.json");

            if (!File.Exists(mainSaveFilePath))
                throw new FileNotFoundException($"Save file not found: {mainSaveFilePath}");

            string mainSaveData = File.ReadAllText(mainSaveFilePath);
            SaveGameData saveGame = JsonConvert.DeserializeObject<SaveGameData>(mainSaveData);

            if (saveGame.Version != SAVE_VERSION)
                throw new Exception($"Save version mismatch. Expected {SAVE_VERSION}, got {saveGame.Version}");

            Scene current = Game.Instance.GetCurrentScene();
            if (current == null)
                throw new Exception("Current scene is null, cannot load save data");

            foreach (SaveType saveType in Enum.GetValues<SaveType>().Where(s => s != SaveType.PlayerPref))
            {
                string saveTypePath = Path.Combine(saveGameDir, saveType.ToString());

                if (!Directory.Exists(saveTypePath))
                    continue;

                if (saveType == SaveType.EntityData)
                {
                    string[] entityFiles = Directory.GetFiles(saveTypePath, "*.json");
                    foreach (string entityFile in entityFiles)
                    {
                        LoadEntity(entityFile, current);
                    }
                }
            }
        }

        private static void SaveEntity(string path, Entity entity)
        {
            if (entity == null)
                return;

            string json = entity.Save();
            string entityFilePath = Path.Combine(path, $"{entity.Id.ToString()}.json");
            File.WriteAllText(entityFilePath, json);
        }

        private static void LoadEntity(string filePath, Scene scene)
        {
            if (!File.Exists(filePath))
                return;

            string json = File.ReadAllText(filePath);
            // TODO: Handle type?
            Entity entity = new Entity();
            entity.Load(json);

            if (entity != null)
            {
                scene.AddEntity(entity);
            }
        }
    }
}
