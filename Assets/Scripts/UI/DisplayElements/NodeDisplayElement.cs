using Models.Map;
using Tooling.Logging;
using UI.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace UI.DisplayElements
{
    public class NodeDisplayElement : DisplayElement<NodeDisplayElement.Data>
    {
        public class Data
        {
            public NodeDefinition Definition;
            public NodeDefinition CurrentPlayerNode;
        }
        [SerializeField] Label label;
        [SerializeField] Button button;
        [SerializeField] Image image;
        [SerializeField] Image playerIndicator;

        AsyncOperationHandle iconOpHandle;

        public override async void Populate(Data data)
        {
            button.interactable = data.CurrentPlayerNode.NextNodes?.Contains(data.Definition.Coord) ?? false;
            bool isPlayerHere = data.Definition.Coord == data.CurrentPlayerNode.Coord;
            label.SetText(isPlayerHere ? "H" : data.Definition.Level.ToString());
            playerIndicator.gameObject.SetActive(isPlayerHere);

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                data.Definition.Event.StartEvent();
                button.interactable = false;
            });

            var iconAssetRef = data.Definition.Event.MapIconReference;
            iconOpHandle = iconAssetRef.OperationHandle;

            if (!iconOpHandle.IsValid())
            {
                iconOpHandle = iconAssetRef.LoadAssetAsync();
            }

            if (!iconOpHandle.IsDone)
            {
                await iconOpHandle.Task;
            }

            if (iconOpHandle.Status == AsyncOperationStatus.Succeeded)
            {
                image.sprite = iconOpHandle.Result as Sprite;
            }
            else
            {
                Addressables.Release(iconOpHandle);
            }
        }

        private void OnDestroy()
        {
            Addressables.Release(iconOpHandle);
        }
    }
}