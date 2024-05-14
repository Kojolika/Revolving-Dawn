using Data;
using Models.Player;
using Settings;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
{
    #region Settings
    [SerializeField] private MapSettings mapSettings;
    [SerializeField] private CardSettings cardSettings;
    #endregion

    #region Asset Data
    [SerializeField] private StaticDataReference<PlayerClassDefinition> playerClassDefinitions;

    #endregion

    public override void InstallBindings()
    {
        Container.BindInstances(mapSettings, cardSettings, playerClassDefinitions);
    }
}