using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utils.Attributes;

namespace Models.Map
{
    [CreateAssetMenu(fileName = "New " + nameof(NodeEventSODefinition), menuName = "RevolvingDawn/Map/" + nameof(NodeEventSODefinition))]
    public class NodeEventSODefinition : ScriptableObject
    {
        [SerializeField] private AssetReferenceSprite mapIconReference;
        [SerializeReference, DisplayAbstract(typeof(NodeEvent))] NodeEvent eventAction;

        public AssetReferenceSprite MapIconReference => mapIconReference;

        public NodeEvent CreateRuntimeEvent()
        {
            var newNodeEvent = Activator.CreateInstance(eventAction.GetType()) as NodeEvent;
            newNodeEvent.PopulateStaticData(name, mapIconReference);
            return newNodeEvent;
        }
    }
}