using Tooling.StaticData.Data;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
{
    public override void InstallBindings()
    {
        Container.BindInstances(
            StaticDatabase.Instance.GetInstance<MapSettings>("default"),
            StaticDatabase.Instance.GetInstance<DowngradeCardSettings>("default"),
            StaticDatabase.Instance.GetInstance<CharacterSettings>("default")
        );
    }
}