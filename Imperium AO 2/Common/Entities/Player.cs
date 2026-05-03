using System;

namespace ImperiumAO.Common.Entities;

public class Player : Character
{
    public int AccountId { get; set; }
    public int Strength { get; set; }
    public int Intelligence { get; set; }
    public int Constitution { get; set; }
    public int Dexterity { get; set; }
    public int Wisdom { get; set; }
    public int Charisma { get; set; }
    public int Gold { get; set; }
    public int Bank { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Class { get; set; }
    public int Race { get; set; }
    public int Gender { get; set; }

    public virtual Account? Account { get; set; }
}
