﻿using System.IO;
using Characters.Model;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Systems.Managers.Base;
using Tooling.Logging;
using File = System.IO.File;

namespace Systems.Managers
{
    public class SaveManager : IManager
    {
        static string SavePath => $"{UnityEngine.Application.persistentDataPath}/saves";
        static string SaveFormat = ".json";
        static string RunsSavePath => $"{SavePath}/runs";
        static string PlayerDataFileName = "player_data";
        static string PlayerSaveFilePath => $"{SavePath}/{PlayerDataFileName}{SaveFormat}";
        static string PlayerDataJsonObjectName = "player";

        public async UniTask Save(PlayerDefinition playerDefinition)
        {
            MyLogger.Log($"Saving file at {PlayerSaveFilePath}");
            if (!Directory.Exists(SavePath))
            {
                MyLogger.Log("Directory not found, creating new...");
                Directory.CreateDirectory(SavePath);
            }

            await File.WriteAllTextAsync(PlayerSaveFilePath, playerDefinition.ToString());
            using (StreamWriter file = File.CreateText(PlayerSaveFilePath))
            {
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    JToken playerJson = JToken.FromObject(playerDefinition);
                    JObject json = new JObject();
                    // Store the player data under the 'player' key
                    json.Add(PlayerDataJsonObjectName, playerJson);

                    json.WriteTo(writer);
                    MyLogger.Log("Saved successfully.");
                }
            }
        }

        public async UniTask<PlayerDefinition> TryLoadSavedData()
        {
            MyLogger.Log($"Loading from {PlayerSaveFilePath}");
            if (File.Exists(PlayerSaveFilePath))
            {
                JObject saveJson = JObject.Parse(await File.ReadAllTextAsync(PlayerSaveFilePath));

                if (saveJson.TryGetValue(PlayerDataJsonObjectName, out JToken playerData))
                {
                    PlayerDefinition playerDefinition = saveJson.ToObject<PlayerDefinition>();

                    if (playerDefinition != null)
                    {
                        return playerDefinition;
                    }
                }
            }

            MyLogger.Log($"No save path found.");
            return null;
        }
    }
}