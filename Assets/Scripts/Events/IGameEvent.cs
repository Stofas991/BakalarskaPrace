/**
 * File: IGameEvent.cs
 * Author: Kryštof Glos
 * Last Modified: 24.4.2024
 * Description: Interface defining methods for game events.
 */

public interface IGameEvent
{
    void InitEvent();                       // Initializes the event
    void StartEvent();                      // Starts the event
    bool UpdateEvent();                     // Updates the event
    PopupWindowInfo EndEvent();            // Ends the event and returns popup window information
    PopupWindowInfo GetPopupWindowInfo();  // Returns popup window information
}

/**
 * Struct: PopupWindowInfo
 * Description: Data structure containing information for a popup window.
 */
public struct PopupWindowInfo
{
    public string textBoxContent;   // Text content for the main text box
    public string acceptContent;    // Text content for the accept button
    public string deniedContent;    // Text content for the denied button
    public bool isDenieable;        // Indicates if the denied button should be enabled
}
