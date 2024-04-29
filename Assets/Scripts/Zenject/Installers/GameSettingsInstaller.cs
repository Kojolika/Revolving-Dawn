using Settings;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
{
    [SerializeField] private MapSettings mapSettings;

    public override void InstallBindings()
    {
        Container.BindInstances(mapSettings);
    }
}