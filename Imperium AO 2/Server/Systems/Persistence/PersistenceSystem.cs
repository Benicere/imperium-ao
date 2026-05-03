using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Linq;

namespace ImperiumAO.Server.Systems.Persistence;

public class PersistenceSystem : IPersistenceSystem
{
    private readonly Dictionary<int, PlayerSaveData> _playerData = new();
    private WorldSaveData? _worldData;
    private readonly List<SavePoint> _savePoints = new();
    private readonly ILogger<PersistenceSystem> _logger;
    private readonly string _savePath = "Saves";
    private int _savePointIdCounter = 1;

    public PersistenceSystem(ILogger<PersistenceSystem> logger)
    {
        _logger = logger;
        if (!Directory.Exists(_savePath))
        {
            Directory.CreateDirectory(_savePath);
        }
    }

    public void SavePlayer(PlayerSaveData playerData)
    {
        playerData.SavedAt = DateTime.UtcNow;
        _playerData[playerData.PlayerId] = playerData;

        var filePath = Path.Combine(_savePath, $"player_{playerData.PlayerId}.json");
        try
        {
            var json = JsonSerializer.Serialize(playerData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
            _logger.LogInformation($"Player {playerData.PlayerId} saved to {filePath}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error saving player {playerData.PlayerId}");
        }
    }

    public void LoadPlayer(int playerId)
    {
        var filePath = Path.Combine(_savePath, $"player_{playerId}.json");
        try
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var playerData = JsonSerializer.Deserialize<PlayerSaveData>(json);
                if (playerData != null)
                {
                    _playerData[playerId] = playerData;
                    _logger.LogInformation($"Player {playerId} loaded from {filePath}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error loading player {playerId}");
        }
    }

    public void SaveWorld(WorldSaveData worldData)
    {
        worldData.SavedAt = DateTime.UtcNow;
        _worldData = worldData;

        var filePath = Path.Combine(_savePath, "world.json");
        try
        {
            var json = JsonSerializer.Serialize(worldData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
            _logger.LogInformation($"World saved to {filePath}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving world");
        }
    }

    public void LoadWorld()
    {
        var filePath = Path.Combine(_savePath, "world.json");
        try
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                _worldData = JsonSerializer.Deserialize<WorldSaveData>(json);
                _logger.LogInformation("World loaded from file");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading world");
        }
    }

    public void CreateSavePoint(string description)
    {
        var savePoint = new SavePoint
        {
            Id = _savePointIdCounter++,
            CreatedAt = DateTime.UtcNow,
            Description = description,
            PlayerCount = _playerData.Count,
            FilePath = Path.Combine(_savePath, $"savepoint_{_savePointIdCounter}.json"),
            IsValid = true
        };

        _savePoints.Add(savePoint);
        _logger.LogInformation($"Save point created: {description}");
    }

    public void LoadSavePoint(int savePointId)
    {
        var savePoint = _savePoints.FirstOrDefault(sp => sp.Id == savePointId);
        if (savePoint != null && File.Exists(savePoint.FilePath))
        {
            _logger.LogInformation($"Loading save point {savePointId}");
        }
    }

    public void DeleteSavePoint(int savePointId)
    {
        var savePoint = _savePoints.FirstOrDefault(sp => sp.Id == savePointId);
        if (savePoint != null)
        {
            _savePoints.Remove(savePoint);
            if (File.Exists(savePoint.FilePath))
            {
                File.Delete(savePoint.FilePath);
            }
            _logger.LogInformation($"Save point {savePointId} deleted");
        }
    }

    public List<SavePoint> GetAllSavePoints()
    {
        return _savePoints.OrderByDescending(sp => sp.CreatedAt).ToList();
    }

    public SavePoint? GetLatestSavePoint()
    {
        return _savePoints.OrderByDescending(sp => sp.CreatedAt).FirstOrDefault();
    }

    public void AutoSave()
    {
        if (_worldData != null)
        {
            SaveWorld(_worldData);
        }

        foreach (var playerData in _playerData.Values)
        {
            SavePlayer(playerData);
        }

        _logger.LogInformation("Auto-save completed");
    }

    public bool ValidateSaveFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                return false;

            var json = File.ReadAllText(filePath);
            JsonSerializer.Deserialize<PlayerSaveData>(json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void BackupSaveData()
    {
        var backupPath = Path.Combine(_savePath, $"backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}");
        Directory.CreateDirectory(backupPath);

        foreach (var file in Directory.GetFiles(_savePath, "*.json"))
        {
            File.Copy(file, Path.Combine(backupPath, Path.GetFileName(file)));
        }

        _logger.LogInformation($"Backup created at {backupPath}");
    }
}


