using Settings;

namespace Models.Map
{
    [System.Serializable]
    public class BeginRunEvent : NodeEvent
    {
        public override void Populate(MapSettings mapSettings, NodeDefinition node)
        {
            //throw new System.NotImplementedException();
        }

        public override void StartEvent()
        {
            throw new System.NotImplementedException();
        }
    }
}