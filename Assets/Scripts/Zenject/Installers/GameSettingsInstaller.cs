using Settings;
using Tooling.StaticData.Data;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
{
    private          MapSettings       mapSettings;
    [SerializeField] CardSettings      cardSettings;
    [SerializeField] CharacterSettings characterSettings;


    public override void InstallBindings()
    {
        Container.BindInstances(
            mapSettings,
            cardSettings,
            characterSettings
        );
    }
}