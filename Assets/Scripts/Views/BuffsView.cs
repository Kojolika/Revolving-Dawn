using System.Collections.Generic;
using Common.Util;
using Fight.Engine;
using Models.Buffs;
using Models.Characters;
using Systems;
using Tooling.StaticData.Data;
using UI;
using UnityEngine;
using Zenject;

namespace Views
{
    public class BuffsView : MonoBehaviour
    {
        private Transform                     characterViewTransform;
        private BuffElement.Factory           buffElementFactory;
        private Dictionary<Buff, BuffElement> buffLookup;
        private ICombatParticipant            target;

        [Inject]
        private void Construct(ICombatParticipant combatParticipant, BuffElement.Factory buffElementFactory)
        {
            this.buffElementFactory = buffElementFactory;
            target                  = combatParticipant;

            var characterBuffs = combatParticipant.GetBuffs().OrEmptyIfNull();
            buffLookup = new();
            foreach (var (stackSize, buff) in characterBuffs)
            {
                AddBuffElement(buff);
            }

            // We want to display below the health
            // TODO: Link them in the inspector instead of hardcoding it here
            transform.localPosition = new Vector3(0f, -0.075f, 0f);

            //characterBuffs.BuffRemoved       += RemoveBuffElement;
            //characterBuffs.BuffAdded         += AddBuffElement;
            //characterBuffs.BuffAmountUpdated += UpdateBuffElement;
        }

        private void RemoveBuffElement(Buff buff)
        {
            var buffToRemove = buffLookup[buff];
            buffLookup.Remove(buff);
            Destroy(buffToRemove);
        }

        private void AddBuffElement(Buff buff)
        {
            var newBuffElement = buffElementFactory.Create(buff, target);
            newBuffElement.transform.SetParent(transform, false);
            buffLookup[buff] = newBuffElement;
        }

        private void UpdateBuffElement(Buff buff)
        {
            buffLookup[buff].SetStackSize(target.GetBuff(buff));
        }
    }
}