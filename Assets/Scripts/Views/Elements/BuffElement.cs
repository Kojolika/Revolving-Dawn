using Cysharp.Threading.Tasks;
using Fight.Engine;
using Models.Buffs;
using Systems.Managers;
using TMPro;
using Tooling.Logging;
using Tooling.StaticData.Data;
using UnityEngine;
using Zenject;

namespace Views
{
    public class BuffElement : MonoBehaviour
    {
        [SerializeField] SpriteRenderer buffArt;
        [SerializeField] TextMeshPro    buffAmount;

        public Buff               Buff   { get; private set; }
        public ICombatParticipant Target { get; private set; }

        [Inject]
        private void Construct(Buff buff, ICombatParticipant target, AddressablesManager addressablesManager)
        {
            Buff   = buff;
            Target = target;
            SetStackSize(target.GetBuff(buff));

            _ = addressablesManager.LoadGenericAsset(
                buff.Icon,
                () => this.GetCancellationTokenOnDestroy().IsCancellationRequested,
                asset => { buffArt.sprite = asset; }
            );
        }

        public void SetStackSize(int amount)
        {
            buffAmount.SetText(amount.ToString());
        }

        public class Factory : PlaceholderFactory<Buff, ICombatParticipant, BuffElement>
        {
        }

        public class CustomFactory : IFactory<Buff, ICombatParticipant, BuffElement>
        {
            private readonly BuffElement buffElementPrefab;
            private readonly DiContainer diContainer;

            public CustomFactory(BuffElement buffElementPrefab, DiContainer diContainer)
            {
                this.buffElementPrefab = buffElementPrefab;
                this.diContainer       = diContainer;
            }

            public BuffElement Create(Buff buff, ICombatParticipant target)
            {
                var newBuffElement = Instantiate(buffElementPrefab);
                diContainer.Inject(newBuffElement, new object[] { buff, target });
                return newBuffElement;
            }
        }
    }
}