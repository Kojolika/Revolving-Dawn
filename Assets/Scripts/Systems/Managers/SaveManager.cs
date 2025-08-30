using System.IO;
using Models.Characters.Player;
using Cysharp.Threading.Tasks;
using Models.Characters;
using Models.Fight;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serialization;
using Systems.Managers;
using Tooling.Logging;
using Zenject;
using File = System.IO.File;
using System.Drawing;
using UnityEditor;

namespace Systems.Managers
{
    public class SaveManager : IManager
    {
        private static string SavePath => $"{UnityEngine.Application.persistentDataPath}/saves";
        private const  string SaveFormat = ".json";
        private static string RunsSavePath => $"{SavePath}/runs";
        private const  string PlayerDataFileName = "player_data";
        private static string PlayerSaveFilePath => $"{SavePath}/{PlayerDataFileName}{SaveFormat}";
        private const  string PlayerDataJsonObjectName = "player";

        private JsonSerializer jsonSerializer;

        [MenuItem("KoJy/Open Player Saves Folder")]
        public static void OpenSaveFolder()
        {
            EditorUtility.RevealInFinder(SavePath);
        }

        [Inject]
        private void Construct(CustomContractResolver customContractResolver)
        {
            jsonSerializer = CreateJsonSerializer(customContractResolver);
        }

        private static JsonSerializer CreateJsonSerializer(CustomContractResolver customContractResolver)
        {
            var jsonSerializer = new JsonSerializer
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling           = TypeNameHandling.Auto,
                Formatting                 = Formatting.Indented,
                ReferenceLoopHandling      = ReferenceLoopHandling.Ignore
            };
            jsonSerializer.Converters.Add(new AssetReferenceConverter());
            jsonSerializer.Converters.Add(new Serialization.ColorConverter());

            if (customContractResolver != null)
            {
                jsonSerializer.ContractResolver = customContractResolver;
            }

            return jsonSerializer;
        }

        public async UniTask Save(PlayerDefinition playerDefinition)
        {
            MyLogger.Log($"Saving file at {PlayerSaveFilePath}");
            if (!Directory.Exists(SavePath))
            {
                MyLogger.Log("Directory not found, creating new...");
                Directory.CreateDirectory(SavePath);
            }

            await File.WriteAllTextAsync(PlayerSaveFilePath, playerDefinition.ToString());

            await using StreamWriter file   = File.CreateText(PlayerSaveFilePath);
            using JsonTextWriter     writer = new(file);

            JToken playerJson = JToken.FromObject(playerDefinition, jsonSerializer);
            JObject json = new()
            {
                // Store the player data under the 'player' key
                { PlayerDataJsonObjectName, playerJson }
            };

            json.WriteTo(writer);
            MyLogger.Log("Saved successfully.");
        }

        public async UniTask DeleteCurrentRun()
        {
            
        }

        public async UniTask SaveFight(FightDefinition fightDefinition, PlayerCharacter playerCharacter)
        {
            if (!File.Exists(PlayerSaveFilePath))
            {
                throw new System.Exception($"Trying to save a fight without save file created already!");
            }

            try
            {
                JObject saveJson = JObject.Parse(await File.ReadAllTextAsync(PlayerSaveFilePath));
                if (saveJson.TryGetValue(PlayerDataJsonObjectName, out JToken playerData))
                {
                    PlayerDefinition playerDefinition = playerData.ToObject<PlayerDefinition>(jsonSerializer);
                    playerDefinition.CurrentRun.PlayerCharacter = playerCharacter;
                    playerDefinition.CurrentRun.CurrentFight    = fightDefinition;

                    using StreamWriter   file   = File.CreateText(PlayerSaveFilePath);
                    using JsonTextWriter writer = new(file);

                    JToken playerJson = JToken.FromObject(playerDefinition, jsonSerializer);
                    JObject json = new()
                    {
                        // Store the player data under the 'player' key
                        { PlayerDataJsonObjectName, playerJson }
                    };

                    json.WriteTo(writer);
                    MyLogger.Log("Saved successfully.");
                }
            }
            catch (JsonReaderException e)
            {
                MyLogger.LogError($"Error reading save file: {e.Message}");
            }
        }

        public async UniTask<PlayerDefinition> TryLoadSavedData()
        {
            MyLogger.Log($"Loading from {PlayerSaveFilePath}");
            if (File.Exists(PlayerSaveFilePath))
            {
                try
                {
                    JObject saveJson = JObject.Parse(await File.ReadAllTextAsync(PlayerSaveFilePath));
                    if (saveJson.TryGetValue(PlayerDataJsonObjectName, out JToken playerData))
                    {
                        PlayerDefinition playerDefinition = playerData.ToObject<PlayerDefinition>(jsonSerializer);

                        if (playerDefinition != null)
                        {
                            return playerDefinition;
                        }
                    }
                }
                catch (JsonReaderException e)
                {
                    MyLogger.LogError($"Error reading save file: {e.Message}");
                    return null;
                }
            }

            MyLogger.Log($"No save path found.");
            return null;
        }
    }
}