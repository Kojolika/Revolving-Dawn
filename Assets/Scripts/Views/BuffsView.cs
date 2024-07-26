using System.Collections.Generic;
using Models.Buffs;
using Systems;
using UI;
using UnityEngine;
using Zenject;

namespace Views
{
    public class BuffsView : MonoBehaviour
    {
        private ICharacterView characterView;
        private Transform characterViewTransform;
        private BuffElement.Factory buffElementFactory;
        private Dictionary<Buff, BuffElement> buffLookup;

        [Inject]
        private void Construct(ICharacterView characterView, BuffElement.Factory buffElementFactory)
        {
            this.characterView = characterView;
            this.characterViewTransform = characterView.transform;
            this.buffElementFactory = buffElementFactory;

            var characterBuffs = characterView.CharacterModel.Buffs;
            buffLookup = new();
            foreach (var buff in characterBuffs)
            {
                AddBuffElement(buff);
            }

            characterBuffs.BuffRemoved += RemoveBuffElement;
            characterBuffs.BuffAdded += AddBuffElement;
            characterBuffs.BuffAmountUpdated += UpdateBuffElement;
        }

        private void RemoveBuffElement(Buff buff)
        {
            var buffToRemove = buffLookup[buff];
            buffLookup.Remove(buff);
            Destroy(buffToRemove);
        }
        private void AddBuffElement(Buff buff)
        {
            var newBuffElement = buffElementFactory.Create(buff);
            newBuffElement.transform.SetParent(transform, false);
            buffLookup[buff] = newBuffElement;
        }
        private void UpdateBuffElement(Buff buff)
        {
            buffLookup[buff].SetStackSize(buff.StackSize);
        }

        public class Factory : PlaceholderFactory<ICharacterView, BuffsView> { }
        public class CustomFactory : IFactory<ICharacterView, BuffsView>
        {
            private readonly BuffsView buffsViewPrefab;
            private readonly DiContainer diContainer;
            public CustomFactory(BuffsView buffsViewPrefab, DiContainer diContainer)
            {
                this.buffsViewPrefab = buffsViewPrefab;
                this.diContainer = diContainer;
            }

            public BuffsView Create(ICharacterView characterView)
            {
                var newBuffsView = Instantiate(buffsViewPrefab, characterView.transform);
                diContainer.Inject(newBuffsView, new ICharacterView[] { characterView });

                return newBuffsView;
            }
        }
    }
}