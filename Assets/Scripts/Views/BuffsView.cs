using System.Collections.Generic;
using Models.Buffs;
using UI;
using UnityEngine;
using Zenject;

namespace Views
{
    public class BuffsView : MonoBehaviour
    {
        private Camera mainCamera;
        private ICharacterView characterView;
        private BuffElement.Factory buffElementFactory;
        private Dictionary<Buff, BuffElement> buffLookup;

        [Inject]
        private void Construct(Camera mainCamera, ICharacterView characterView, BuffElement.Factory buffElementFactory)
        {
            this.mainCamera = mainCamera;
            this.characterView = characterView;
            this.buffElementFactory = buffElementFactory;

            var characterBuffs = characterView.CharacterModel.Buffs;
            buffLookup = new();
            foreach (var buff in characterBuffs)
            {
                var newBuffElement = buffElementFactory.Create(buff);
                newBuffElement.transform.SetParent(transform);
                buffLookup[buff] = newBuffElement;
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
            newBuffElement.transform.SetParent(transform);
            buffLookup[buff] = newBuffElement;
        }
        private void UpdateBuffElement(Buff buff)
        {
            buffLookup[buff].SetStackSize(buff.StackSize);
        }

        private void Update()
        {
            var screenPos = mainCamera.WorldToScreenPoint(characterView.transform.position);

            if (transform.position != screenPos)
            {
                transform.position = screenPos;
            }
        }

        public class Factory : PlaceholderFactory<ICharacterView, BuffsView> { }
        public class CustomFactory : IFactory<ICharacterView, BuffsView>
        {
            private readonly Canvas fightOverlayCanvas;
            private readonly BuffsView buffsViewPrefab;
            private readonly DiContainer diContainer;
            public CustomFactory(Canvas fightOverlayCanvas, BuffsView buffsViewPrefab, DiContainer diContainer)
            {
                this.fightOverlayCanvas = fightOverlayCanvas;
                this.buffsViewPrefab = buffsViewPrefab;
                this.diContainer = diContainer;
            }

            public BuffsView Create(ICharacterView characterView)
            {
                var newBuffsView = Instantiate(buffsViewPrefab, fightOverlayCanvas.transform);
                diContainer.Inject(newBuffsView, new ICharacterView[] { characterView });

                return newBuffsView;
            }
        }
    }
}