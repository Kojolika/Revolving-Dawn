using Systems.Managers;
using UnityEngine;
using Views;

namespace Zenject.Installers
{
    public class FightInstaller : MonoInstaller<FightInstaller>
    {
        [SerializeField] LevelView levelView;

        public override void InstallBindings()
        {

        }
    }
}