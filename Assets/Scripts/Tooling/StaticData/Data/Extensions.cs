namespace Tooling.StaticData.Data
{
    public static class Extensions
    {
        public static bool IsReferenceValid(this StaticDataReference reference)
        {
            return reference is { Type: not null } && !string.IsNullOrEmpty(reference.InstanceName);
        }
    }
}