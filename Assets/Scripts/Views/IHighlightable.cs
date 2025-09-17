namespace Views
{
    public interface IHighlightable
    {
        void Highlight();
        void HighlightFriendly();
        void HighlightEnemy();
        void Unhighlight();
    }
}