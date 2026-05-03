namespace ImperiumAO.Common.Database.Models;

public record CharacterSummary(
    int Id,
    int AccountId,
    string Name,
    int Level,
    int Experience,
    byte RaceId,
    byte ClassId,
    byte GenreId,
    int BodyId,
    int HeadId,
    int WeaponId,
    int HelmetId,
    int ShieldId,
    byte Heading,
    int PosMap,
    int PosX,
    int PosY,
    int Gold,
    int Health,
    int MaxHealth,
    int Mana,
    int MaxMana,
    int Stamina,
    int MaxStamina
);
