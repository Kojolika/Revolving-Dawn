using Fight;
using Models;
using TMPro;
using UnityEngine;
using Zenject;

namespace Views
{
    public class HealthView : MonoBehaviour
    {
        [SerializeField] SpriteRenderer borderRenderer;
        [SerializeField] SpriteRenderer healthFillRenderer;
        [SerializeField] TextMeshPro healthAmountText;

        private Health health;
        private BattleEngine battleEngine;
        private ICharacterView characterView;

        [Inject]
        private void Construct(Health health, ICharacterView characterView, BattleEngine battleEngine)
        {
            this.health = health;
            this.battleEngine = battleEngine;
            this.characterView = characterView;
        }

        public void PreviewCardEffects(CardModel cardModel)
        {

        }

        public class Factory : PlaceholderFactory<Health, ICharacterView, HealthView> { }
    }
}