using Settings;

namespace Models.Map
{
    [System.Serializable]
    public class EncounterEvent : NodeEvent
    {
        public override void Populate(MapSettings mapSettings, NodeDefinition node, int maxNodeLevelForMap)
        {
            //throw new System.NotImplementedException();
        }

        public override void StartEvent()
        {
            throw new System.NotImplementedException();
        }
    }
}