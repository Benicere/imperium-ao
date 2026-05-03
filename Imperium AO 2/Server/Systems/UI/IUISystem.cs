using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.UI;

public interface IUISystem
{
    void OpenWindow(int playerId, UIWindowType windowType);
    void CloseWindow(int playerId, UIWindowType windowType);
    void UpdateWindowData(int playerId, UIWindowType windowType, Dictionary<string, object> data);
    UIWindow? GetWindow(int playerId, UIWindowType windowType);
    List<UIWindow> GetOpenWindows(int playerId);
    void HandleButtonClick(int playerId, string buttonId);
    void SendUIUpdate(int playerId, UIWindowType windowType);
    void CreateCustomPanel(string panelId, UIPanel panel);
    UIPanel? GetPanel(string panelId);
    void ShowNotification(int playerId, string message, int durationMs);
}

