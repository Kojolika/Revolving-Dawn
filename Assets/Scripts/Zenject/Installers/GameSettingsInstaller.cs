using System.Collections.Generic;
using Data;
using Models.Player;
using Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
{
    #region Settings
    [SerializeField] private MapSettings mapSettings;
    #endregion

    #region Asset Data
    [SerializeField] private List<AssetReferenceT<PlayerClassDefinition>> playerClassDefinitions;

    #endregion

    public override void InstallBindings()
    {
        Container.BindInstances(mapSettings);
        Container.BindInstances(playerClassDefinitions);
    }
}