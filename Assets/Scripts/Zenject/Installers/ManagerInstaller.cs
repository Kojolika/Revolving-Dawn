using Systems.Managers.Base;
using UnityEngine;

namespace Zenject.Installers
{
    public class ManagerInstaller : MonoInstaller<ManagerInstaller>
    {
        [SerializeField] private ScriptableObjectManagers scriptableObjectManagers;
        public override void InstallBindings()
        {
            foreach (var soManager in scriptableObjectManagers.SOManagers)
            {
                var type = soManager.GetType();
                Container.Bind(type).To(type).AsSingle();
            }
        }
    }
}