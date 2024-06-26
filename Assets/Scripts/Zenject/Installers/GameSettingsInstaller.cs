using Data;
using Models.Player;
using Settings;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
{
    #region Settings
    [SerializeField] MapSettings mapSettings;
    [SerializeField] CardSettings cardSettings;
    [SerializeField] CharacterSettings characterSettings;
    #endregion

    #region Asset Data
    [SerializeField] private StaticDataReference<PlayerClassSODefinition> playerClassDefinitions;

    #endregion

    public override void InstallBindings()
    {
        Container.BindInstances(
            mapSettings, 
            cardSettings,
            characterSettings,
            playerClassDefinitions
        );
    }
}