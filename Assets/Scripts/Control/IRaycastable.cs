namespace RPG.Control {
    public interface IRaycastable
    {
        bool HandleRaycast(PlayerController caller);
        CursorType GetCursorType();
    }
}