using System.Collections.Generic;
using Models.Mana;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "New " + nameof(CharacterSettings), menuName = "RevolvingDawn/Settings/" + nameof(CharacterSettings))]
    public class CharacterSettings : ScriptableObject
    {
        [SerializeField] private int handSize;
        [SerializeField] private int drawAmount;
        [SerializeField] private int usableManaPerTurn;
        [SerializeField] private int numberOfManaRefreshedPerTurn;
        [SerializeField] private List<ManaSODefinition> allManaTypesAvailable = new();
        

        public int HandSize => handSize;
        public int DrawAmount => drawAmount;
        public int UsableManaPerTurn => usableManaPerTurn;
        public int NumberOfManaRefreshedPerTurn => numberOfManaRefreshedPerTurn;
        public List<ManaSODefinition> AllManaTypesAvailable => allManaTypesAvailable;
    }
}