using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Pets;

public interface IPetSystem
{
    void SummonPet(int playerId, PetType type, string name, int durationSeconds);
    void DismissPet(int playerId);
    Pet? GetActivePet(int playerId);
    List<Pet> GetPlayerPets(int playerId);
    void CommandPet(int playerId, int commandType);
    void RemovePet(int petId);
    void GainPetExperience(int petId, int amount);
    void HealPet(int petId, int amount);
    bool IsPetAlive(int petId);
    void UpdatePets();
}

