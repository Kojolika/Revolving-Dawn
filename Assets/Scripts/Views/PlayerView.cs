using Cysharp.Threading.Tasks;
using Fight.Engine;
using Models.Characters;
using Systems.Managers;
using Tooling.Logging;
using Tooling.StaticData.Data;
using UnityEngine;
using Zenject;

namespace Views
{
    public class PlayerView : MonoBehaviour, IParticipantView
    {
        [SerializeField] SpriteRenderer spriteRenderer;

        private PlayerCharacter playerCharacter;
        private BuffsView       buffsView;
        private HealthView      healthView;

        #region ICharacterView

        public ICombatParticipant Model    => playerCharacter;
        public Collider           Collider { get; private set; }
        public Renderer           Renderer => spriteRenderer;

        public void Highlight()
        {
            throw new System.NotImplementedException();
        }

        public void HighlightFriendly()
        {
            throw new System.NotImplementedException();
        }

        public void HighlightEnemy()
        {
            throw new System.NotImplementedException();
        }

        public void Unhighlight()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        [Inject]
        private void Construct(PlayerCharacter playerCharacter, AddressablesManager addressablesManager, HealthView healthView, BuffsView buffsView)
        {
            this.playerCharacter = playerCharacter;
            this.healthView      = healthView;
            this.buffsView       = buffsView;

            _ = addressablesManager.LoadGenericAsset(
                playerCharacter.Class.ClassArt,
                () => this.GetCancellationTokenOnDestroy().IsCancellationRequested,
                asset =>
                {
                    spriteRenderer.sprite = asset;
                    Collider              = spriteRenderer.gameObject.AddComponent<BoxCollider>();
                }
            );
        }

        public class Factory : PlaceholderFactory<PlayerCharacter, PlayerView>
        {
        }
    }
}