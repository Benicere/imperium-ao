using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.Parties;

public interface IPartySystem
{
    Party? CreateParty(int leaderId, string leaderName, int leaderLevel);
    Party? GetParty(int partyId);
    bool AddMember(int partyId, int characterId, string characterName, int level);
    bool RemoveMember(int partyId, int characterId);
    bool DisbandParty(int partyId);
    Party? GetCharacterParty(int characterId);
    bool TransferLeadership(int partyId, int newLeaderId);
    List<PartyMember> GetPartyMembers(int partyId);
    void ShareExperience(int partyId, int totalExp);
    void ShareLoot(int partyId, int itemId);
}

