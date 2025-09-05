namespace ElasticSea.Framework.Interactions
{
    public static class InteractionsExtensions
    {
        public static ExclusiveSimpleInteractable AsExclusive(this IInteractable interactions)
        {
            return new ExclusiveSimpleInteractable(interactions);
        }
    }
}