using Tooling.StaticData.Data;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
{
    public override void InstallBindings()
    {
        Container.BindInstances(
            StaticDatabase.Instance.GetStaticDataInstance<MapSettings>("default"),
            StaticDatabase.Instance.GetStaticDataInstance<DowngradeCardSettings>("default"),
            StaticDatabase.Instance.GetStaticDataInstance<CharacterSettings>("default")
        );
    }
}