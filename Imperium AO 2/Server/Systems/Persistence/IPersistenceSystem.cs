using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Persistence;

public interface IPersistenceSystem
{
    void SavePlayer(PlayerSaveData playerData);
    void LoadPlayer(int playerId);
    void SaveWorld(WorldSaveData worldData);
    void LoadWorld();
    void CreateSavePoint(string description);
    void LoadSavePoint(int savePointId);
    void DeleteSavePoint(int savePointId);
    List<SavePoint> GetAllSavePoints();
    SavePoint? GetLatestSavePoint();
    void AutoSave();
    bool ValidateSaveFile(string filePath);
    void BackupSaveData();
}

