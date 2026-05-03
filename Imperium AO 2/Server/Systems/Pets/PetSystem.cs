using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using System.Linq;

namespace ImperiumAO.Server.Systems.Pets;

public class PetSystem : IPetSystem
{
    private readonly Dictionary<int, Pet> _activePets = new();
    private readonly Dictionary<int, List<Pet>> _playerPets = new();
    private readonly ILogger<PetSystem> _logger;
    private int _petIdCounter = 1;

    public PetSystem(ILogger<PetSystem> logger)
    {
        _logger = logger;
    }

    public void SummonPet(int playerId, PetType type, string name, int durationSeconds)
    {
        if (_activePets.Values.Any(p => p.OwnerId == playerId))
        {
            _logger.LogWarning($"Player {playerId} already has an active pet");
            return;
        }

        var pet = new Pet
        {
            Id = _petIdCounter++,
            OwnerId = playerId,
            Name = name,
            Type = type,
            Level = 1,
            Health = 50,
            MaxHealth = 50,
            Mana = 25,
            MaxMana = 25,
            Damage = 10,
            Defense = 5,
            IsActive = true,
            SummonedAt = DateTime.UtcNow,
            DurationSeconds = durationSeconds
        };

        _activePets[pet.Id] = pet;

        if (!_playerPets.ContainsKey(playerId))
        {
            _playerPets[playerId] = new();
        }
        _playerPets[playerId].Add(pet);

        _logger.LogInformation($"Player {playerId} summoned pet {name} (Type: {type})");
    }

    public void DismissPet(int playerId)
    {
        var pet = GetActivePet(playerId);
        if (pet != null)
        {
            _activePets.Remove(pet.Id);
            pet.IsActive = false;
            _logger.LogInformation($"Pet {pet.Name} dismissed");
        }
    }

    public Pet? GetActivePet(int playerId)
    {
        return _activePets.Values.FirstOrDefault(p => p.OwnerId == playerId && !p.IsExpired);
    }

    public List<Pet> GetPlayerPets(int playerId)
    {
        if (_playerPets.TryGetValue(playerId, out var pets))
        {
            return pets;
        }
        return new();
    }

    public void CommandPet(int playerId, int commandType)
    {
        var pet = GetActivePet(playerId);
        if (pet != null)
        {
            _logger.LogInformation($"Pet {pet.Name} received command {commandType}");
        }
    }

    public void RemovePet(int petId)
    {
        _activePets.Remove(petId);
    }

    public void GainPetExperience(int petId, int amount)
    {
        if (_activePets.TryGetValue(petId, out var pet))
        {
            var oldLevel = pet.Level;
            pet.GainExperience(amount);

            if (pet.Level > oldLevel)
            {
                _logger.LogInformation($"Pet {pet.Name} leveled up to {pet.Level}");
            }
        }
    }

    public void HealPet(int petId, int amount)
    {
        if (_activePets.TryGetValue(petId, out var pet))
        {
            pet.Heal(amount);
        }
    }

    public bool IsPetAlive(int petId)
    {
        if (_activePets.TryGetValue(petId, out var pet))
        {
            return pet.Health > 0;
        }
        return false;
    }

    public void UpdatePets()
    {
        var expiredPets = _activePets.Where(kvp => kvp.Value.IsExpired).Select(kvp => kvp.Key).ToList();
        foreach (var petId in expiredPets)
        {
            RemovePet(petId);
            _logger.LogInformation($"Pet {petId} duration expired");
        }
    }
}


