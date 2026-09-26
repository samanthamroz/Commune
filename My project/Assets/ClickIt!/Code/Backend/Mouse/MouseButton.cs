namespace ClickIt {
    [System.Flags]
    public enum MouseButton {
        None = 0,
        left = 1 << 0,    // 1
        middle = 1 << 1,  // 2
        right = 1 << 2    // 4
    }
}