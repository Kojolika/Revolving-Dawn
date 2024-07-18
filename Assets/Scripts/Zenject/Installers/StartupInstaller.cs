using GameLoop;
using UnityEngine;

namespace Zenject.Installers
{
    [CreateAssetMenu(fileName = nameof(StartupInstaller), menuName = "Installers/" + nameof(StartupInstaller))]
    public class StartupInstaller : ScriptableObjectInstaller<StartupInstaller>
    {
        [SerializeField] AddressableAssetLabelLoader addressableAssetLabelLoader;
        public override void InstallBindings()
        {
            Container.BindInstances(addressableAssetLabelLoader);
        }
    }
}