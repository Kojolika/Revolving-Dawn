namespace Models.Health
{
    [System.Serializable]
    public class HealthDefinition
    {
        [Newtonsoft.Json.JsonProperty("max_health")]
        public ulong MaxHealth;
    }
}