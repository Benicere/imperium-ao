using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.UI;

public enum UIWindowType
{
    Inventory,
    Equipment,
    Character,
    Skills,
    Map,
    Party,
    Guild,
    Quests,
    Trade,
    Chat,
    Bank,
    Shop,
    Crafting
}

public class UIWindow
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public UIWindowType Type { get; set; }
    public bool IsVisible { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
    public DateTime OpenedAt { get; set; }
}

public class UIButton
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool IsEnabled { get; set; }
    public string ActionId { get; set; } = string.Empty;
}

public class UIPanel
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<UIButton> Buttons { get; set; } = new();
    public List<UIElement> Elements { get; set; } = new();
    public bool IsVisible { get; set; }
}

public class UIElement
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
}

