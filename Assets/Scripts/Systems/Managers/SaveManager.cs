using System.IO;
using Models.Characters.Player;
using Cysharp.Threading.Tasks;
using Models.Characters;
using Models.Fight;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serialization;
using Systems.Managers.Base;
using Tooling.Logging;
using Zenject;
using File = System.IO.File;

namespace Systems.Managers
{
    public class SaveManager : IManager
    {
        static string SavePath => $"{UnityEngine.Application.persistentDataPath}/saves";
        const string SaveFormat = ".json";
        static string RunsSavePath => $"{SavePath}/runs";
        const string PlayerDataFileName = "player_data";
        static string PlayerSaveFilePath => $"{SavePath}/{PlayerDataFileName}{SaveFormat}";
        const string PlayerDataJsonObjectName = "player";

        private JsonSerializer jsonSerializer;

        [Inject]
        void Construct(ZenjectDependenciesContractResolver zenjectDependencyContractResolver)
        {
            jsonSerializer = new JsonSerializer
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
            };
            jsonSerializer.Converters.Add(new AssetReferenceConverter());
            jsonSerializer.ContractResolver = zenjectDependencyContractResolver;
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

            using StreamWriter file = File.CreateText(PlayerSaveFilePath);
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
                    playerDefinition.CurrentRun.CurrentFight = fightDefinition;

                    using StreamWriter file = File.CreateText(PlayerSaveFilePath);
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