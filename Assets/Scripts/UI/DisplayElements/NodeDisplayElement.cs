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

        public override async void Populate(Data data)
        {
            button.interactable = data.CurrentPlayerNode.NextNodes?.Contains(data.Definition.Coord) ?? false;
            label.SetText(data.Definition.Coord == data.CurrentPlayerNode.Coord ? "H" : data.Definition.Level.ToString());

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                data.Definition.Event.StartEvent();
            });

            var iconAssetRef = data.Definition.Event.MapIconReference;
            var opHandle = iconAssetRef.OperationHandle;

            if (!opHandle.IsValid())
            {
                opHandle = iconAssetRef.LoadAssetAsync();
            }

            if (!opHandle.IsDone)
            {
                await opHandle.Task;
            }

            if (opHandle.Status == AsyncOperationStatus.Succeeded)
            {
                image.sprite = opHandle.Result as Sprite;
            }
            else
            {
                Addressables.Release(opHandle);
            }
        }
    }
}