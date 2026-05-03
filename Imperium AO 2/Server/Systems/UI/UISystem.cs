using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using System.Linq;

namespace ImperiumAO.Server.Systems.UI;

public class UISystem : IUISystem
{
    private readonly Dictionary<int, Dictionary<UIWindowType, UIWindow>> _playerWindows = new();
    private readonly Dictionary<string, UIPanel> _panels = new();
    private readonly ILogger<UISystem> _logger;
    private int _windowIdCounter = 1;

    public UISystem(ILogger<UISystem> logger)
    {
        _logger = logger;
    }

    public void OpenWindow(int playerId, UIWindowType windowType)
    {
        if (!_playerWindows.ContainsKey(playerId))
        {
            _playerWindows[playerId] = new();
        }

        var window = new UIWindow
        {
            Id = _windowIdCounter++,
            PlayerId = playerId,
            Type = windowType,
            IsVisible = true,
            OpenedAt = DateTime.UtcNow,
            Width = 300,
            Height = 400
        };

        _playerWindows[playerId][windowType] = window;
        _logger.LogInformation($"Window {windowType} opened for player {playerId}");
    }

    public void CloseWindow(int playerId, UIWindowType windowType)
    {
        if (_playerWindows.TryGetValue(playerId, out var windows))
        {
            if (windows.TryGetValue(windowType, out var window))
            {
                window.IsVisible = false;
                windows.Remove(windowType);
                _logger.LogInformation($"Window {windowType} closed for player {playerId}");
            }
        }
    }

    public void UpdateWindowData(int playerId, UIWindowType windowType, Dictionary<string, object> data)
    {
        var window = GetWindow(playerId, windowType);
        if (window != null)
        {
            window.Data = data;
            _logger.LogInformation($"Window {windowType} data updated for player {playerId}");
        }
    }

    public UIWindow? GetWindow(int playerId, UIWindowType windowType)
    {
        if (_playerWindows.TryGetValue(playerId, out var windows))
        {
            windows.TryGetValue(windowType, out var window);
            return window;
        }
        return null;
    }

    public List<UIWindow> GetOpenWindows(int playerId)
    {
        if (_playerWindows.TryGetValue(playerId, out var windows))
        {
            return windows.Values.Where(w => w.IsVisible).ToList();
        }
        return new();
    }

    public void HandleButtonClick(int playerId, string buttonId)
    {
        _logger.LogInformation($"Player {playerId} clicked button: {buttonId}");
    }

    public void SendUIUpdate(int playerId, UIWindowType windowType)
    {
        var window = GetWindow(playerId, windowType);
        if (window != null)
        {
            _logger.LogInformation($"UI update sent for {windowType} to player {playerId}");
        }
    }

    public void CreateCustomPanel(string panelId, UIPanel panel)
    {
        _panels[panelId] = panel;
        _logger.LogInformation($"Custom panel created: {panelId}");
    }

    public UIPanel? GetPanel(string panelId)
    {
        _panels.TryGetValue(panelId, out var panel);
        return panel;
    }

    public void ShowNotification(int playerId, string message, int durationMs)
    {
        _logger.LogInformation($"Notification for player {playerId}: {message} ({durationMs}ms)");
    }
}


