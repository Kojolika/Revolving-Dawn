using Cysharp.Threading.Tasks;
using Models.Buffs;
using Systems.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Views
{
    public class BuffElement : MonoBehaviour
    {
        [SerializeField] Image buffArt;
        [SerializeField] TextMeshProUGUI buffAmount;

        public Buff Buff { get; private set; }

        [Inject]
        private void Construct(Buff buff, AddressablesManager addressablesManager)
        {
            Buff = buff;
            SetStackSize(buff.StackSize);
            _ = addressablesManager.LoadGenericAsset(buff.Definition.Icon,
                () => this.GetCancellationTokenOnDestroy().IsCancellationRequested,
                asset => buffArt.sprite = asset
            );
        }

        public void SetStackSize(ulong amount)
        {
            buffAmount.SetText($"{amount}");
        }

        public class Factory : PlaceholderFactory<Buff, BuffElement> { }
        public class CustomFactory : IFactory<Buff, BuffElement>
        {
            private readonly BuffElement buffElementPrefab;
            private readonly DiContainer diContainer;
            public CustomFactory(BuffElement buffElementPrefab, DiContainer diContainer)
            {
                this.buffElementPrefab = buffElementPrefab;
                this.diContainer = diContainer;
            }

            public BuffElement Create(Buff buff)
            {
                var newBuffElement = Instantiate(buffElementPrefab);
                diContainer.Inject(newBuffElement, new Buff[] { buff });

                return newBuffElement;
            }
        }
    }
}