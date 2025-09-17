using Cysharp.Threading.Tasks;
using Models.Map;
using Systems.Managers;
using Tooling.Logging;
using UI.Common;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.DisplayElements
{
    public class NodeDisplayElement : MonoBehaviour
    {
        [SerializeField] private Label  label;
        [SerializeField] private Button button;
        [SerializeField] private Image  image;
        [SerializeField] private Image  playerIndicator;

        private AddressablesManager addressablesManager;
        private PlayerDataManager   playerDataManager;
        private Data                data;

        public NodeDefinition Model => data?.Definition;

        [Inject]
        private void Construct(AddressablesManager addressablesManager, PlayerDataManager playerDataManager, Data data)
        {
            this.addressablesManager = addressablesManager;
            this.playerDataManager   = playerDataManager;
            this.data                = data;
        }

        private void Start()
        {
            button.interactable = data.CurrentPlayerNode.NextNodes?.Contains(data.Definition.Coord) ?? false;
            bool isPlayerHere = data.Definition.Coord == data.CurrentPlayerNode.Coord;
            label.SetText(isPlayerHere ? "H" : data.Definition.Level.ToString());
            playerIndicator.gameObject.SetActive(isPlayerHere);

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                button.interactable = false;

                _ = playerDataManager.UpdateMapNode(data.Definition);
            });

            _ = addressablesManager.LoadGenericAsset(
                data.Definition.Event.Icon,
                () => this.GetCancellationTokenOnDestroy().IsCancellationRequested,
                asset => image.sprite = asset
            );
        }

        public class Factory : PlaceholderFactory<Data, NodeDisplayElement>
        {
        }

        public class Data
        {
            public NodeDefinition Definition;
            public NodeDefinition CurrentPlayerNode;
        }
    }
}