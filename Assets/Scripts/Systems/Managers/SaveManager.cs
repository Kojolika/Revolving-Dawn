using System.IO;
using Models.Characters.Player;
using Cysharp.Threading.Tasks;
using Models.Characters;
using Models.Fight;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serialization;
using Tooling.Logging;
using Tooling.StaticData.Data;
using Zenject;
using File = System.IO.File;
using UnityEditor;
using UnityEngine;

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

        public static bool IsSavingEnabled
        {
#if !PRODUCTION || ENABLE_DEBUG_MENU
            get => PlayerPrefs.GetInt("IsSavingEnabled") == 1;
            set => PlayerPrefs.SetInt("IsSavingEnabled", value ? 1 : 0);
#else
            get => true;
#endif
        }

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
                ReferenceLoopHandling      = ReferenceLoopHandling.Ignore,
                Converters =
                {
                    new AssetReferenceConverter(),
                    new ColorConverter(),
                    new StaticDataConverter()
                }
            };

            if (customContractResolver != null)
            {
                jsonSerializer.ContractResolver = customContractResolver;
            }

            return jsonSerializer;
        }

        public async UniTask Save(PlayerDefinition playerDefinition)
        {
            if (!IsSavingEnabled)
            {
                MyLogger.Info("Saving disabled returning...");
                return;
            }

            if (playerDefinition == null)
            {
                MyLogger.Error("Trying to save a null player definition!");
                return;
            }

            MyLogger.Info($"Saving file at {PlayerSaveFilePath}");
            if (!Directory.Exists(SavePath))
            {
                MyLogger.Info("Directory not found, creating new...");
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
            MyLogger.Info("Saved successfully.");
        }

        public async UniTask SaveFight(FightDefinition fightDefinition, PlayerCharacter playerCharacter)
        {
            if (!IsSavingEnabled)
            {
                MyLogger.Info("Saving disabled returning...");
                return;
            }

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
                    MyLogger.Info("Saved successfully.");
                }
            }
            catch (JsonReaderException e)
            {
                MyLogger.Error($"Error reading save file: {e.Message}");
            }
        }

        public async UniTask<PlayerDefinition> TryLoadSavedData()
        {
            MyLogger.Info($"Loading from {PlayerSaveFilePath}");
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
                    MyLogger.Error($"Error reading save file: {e.Message}");
                    return null;
                }
            }

            MyLogger.Info($"No save path found.");
            return null;
        }
    }
}