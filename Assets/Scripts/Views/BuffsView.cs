using System.Collections.Generic;
using Models.Buffs;
using Models.Characters;
using Systems;
using UI;
using UnityEngine;
using Zenject;

namespace Views
{
    public class BuffsView : MonoBehaviour
    {
        private Transform characterViewTransform;
        private BuffElement.Factory buffElementFactory;
        private Dictionary<Buff, BuffElement> buffLookup;

        [Inject]
        private void Construct(Character character, BuffElement.Factory buffElementFactory)
        {
            this.buffElementFactory = buffElementFactory;

            var characterBuffs = character.Buffs;
            buffLookup = new();
            foreach (var buff in characterBuffs)
            {
                AddBuffElement(buff);
            }

            // We want to display below the health
            // TODO: Link them in the inspector instead of hardcoding it here
            transform.localPosition = new Vector3(0f, -0.075f, 0f);

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
    }
}