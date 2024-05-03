public interface IGameEvent
{
    void InitEvent();
    void StartEvent();
    bool UpdateEvent();
    PopupWindowInfo EndEvent();
    PopupWindowInfo GetPopupWindowInfo();
}

public struct PopupWindowInfo
{
    public string textBoxContent;
    public string acceptContent;
    public string deniedContent;
    public bool isDenieable;
}
