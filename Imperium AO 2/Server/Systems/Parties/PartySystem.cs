using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImperiumAO.Server.Systems.Parties;

public class PartySystem : IPartySystem
{
    private readonly Dictionary<int, Party> _parties = new();
    private readonly Dictionary<int, int> _characterToParty = new();
    private int _nextPartyId = 1;
    private readonly ILogger<PartySystem> _logger;

    public PartySystem(ILogger<PartySystem> logger)
    {
        _logger = logger;
    }

    public Party? CreateParty(int leaderId, string leaderName, int leaderLevel)
    {
        if (_characterToParty.ContainsKey(leaderId))
        {
            _logger.LogWarning($"Character {leaderId} is already in a party");
            return null;
        }

        var party = new Party
        {
            Id = _nextPartyId++,
            LeaderId = leaderId,
            Members = new()
            {
                new PartyMember { CharacterId = leaderId, Name = leaderName, Level = leaderLevel }
            }
        };

        _parties[party.Id] = party;
        _characterToParty[leaderId] = party.Id;

        _logger.LogInformation($"Party {party.Id} created by {leaderName}");
        return party;
    }

    public Party? GetParty(int partyId)
    {
        _parties.TryGetValue(partyId, out var party);
        return party;
    }

    public bool AddMember(int partyId, int characterId, string characterName, int level)
    {
        if (!_parties.TryGetValue(partyId, out var party))
        {
            return false;
        }

        if (_characterToParty.ContainsKey(characterId))
        {
            _logger.LogWarning($"Character {characterId} is already in a party");
            return false;
        }

        if (party.Members.Count >= party.MaxMembers)
        {
            _logger.LogWarning($"Party {partyId} is full");
            return false;
        }

        party.Members.Add(new PartyMember { CharacterId = characterId, Name = characterName, Level = level });
        _characterToParty[characterId] = partyId;

        _logger.LogInformation($"Character {characterName} joined party {partyId}");
        return true;
    }

    public bool RemoveMember(int partyId, int characterId)
    {
        if (!_parties.TryGetValue(partyId, out var party))
        {
            return false;
        }

        var member = party.Members.FirstOrDefault(m => m.CharacterId == characterId);
        if (member == null)
        {
            return false;
        }

        party.Members.Remove(member);
        _characterToParty.Remove(characterId);

        _logger.LogInformation($"Character {characterId} left party {partyId}");
        return true;
    }

    public bool DisbandParty(int partyId)
    {
        if (!_parties.TryGetValue(partyId, out var party))
        {
            return false;
        }

        foreach (var member in party.Members)
        {
            _characterToParty.Remove(member.CharacterId);
        }

        _parties.Remove(partyId);
        _logger.LogInformation($"Party {partyId} has been disbanded");
        return true;
    }

    public Party? GetCharacterParty(int characterId)
    {
        if (!_characterToParty.TryGetValue(characterId, out var partyId))
        {
            return null;
        }

        return GetParty(partyId);
    }

    public bool TransferLeadership(int partyId, int newLeaderId)
    {
        if (!_parties.TryGetValue(partyId, out var party))
        {
            return false;
        }

        var member = party.Members.FirstOrDefault(m => m.CharacterId == newLeaderId);
        if (member == null)
        {
            return false;
        }

        party.LeaderId = newLeaderId;
        return true;
    }

    public List<PartyMember> GetPartyMembers(int partyId)
    {
        if (!_parties.TryGetValue(partyId, out var party))
        {
            return new();
        }

        return party.Members.ToList();
    }

    public void ShareExperience(int partyId, int totalExp)
    {
        if (!_parties.TryGetValue(partyId, out var party))
        {
            return;
        }

        var expPerMember = totalExp / party.Members.Count;
        _logger.LogInformation($"Shared {expPerMember} experience to each member of party {partyId}");
    }

    public void ShareLoot(int partyId, int itemId)
    {
        if (!_parties.TryGetValue(partyId, out var party))
        {
            return;
        }

        var randomMember = party.Members[new Random().Next(party.Members.Count)];
        _logger.LogInformation($"Item {itemId} given to {randomMember.Name} in party {partyId}");
    }
}

