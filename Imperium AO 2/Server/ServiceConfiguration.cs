using System;
using System.Linq;
using ImperiumAO.Common.Database;
using ImperiumAO.Server.Core;
using ImperiumAO.Server.Data;
using ImperiumAO.Server.Handlers;
using ImperiumAO.Server.Map;
using ImperiumAO.Server.Network;
using ImperiumAO.Server.Systems.Admin;
using ImperiumAO.Server.Systems.Audio;
using ImperiumAO.Server.Systems.Chat;
using ImperiumAO.Server.Systems.Combat;
using ImperiumAO.Server.Systems.Crafting;
using ImperiumAO.Server.Systems.Effects;
using ImperiumAO.Server.Systems.Factions;
using ImperiumAO.Server.Systems.Forum;
using ImperiumAO.Server.Systems.Guilds;
using ImperiumAO.Server.Systems.Inventory;
using ImperiumAO.Server.Systems.Magic;
using ImperiumAO.Server.Systems.NPCs;
using ImperiumAO.Server.Systems.Parties;
using ImperiumAO.Server.Systems.Persistence;
using ImperiumAO.Server.Systems.Pets;
using ImperiumAO.Server.Systems.PvP;
using ImperiumAO.Server.Systems.Quests;
using ImperiumAO.Server.Systems.Skills;
using ImperiumAO.Server.Systems.Statistics;
using ImperiumAO.Server.Systems.Trading;
using ImperiumAO.Server.Systems.UI;
using ImperiumAO.Server.Systems.World;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ImperiumAO.Server;

public static class ServiceConfiguration
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseConfig>(configuration.GetSection("Database"));
        services.Configure<ServerOptions>(configuration.GetSection("Server"));

        var dbConfig = configuration.GetSection("Database").Get<DatabaseConfig>();
        services.AddDbContext<ImperiumAOContext>(options =>
            options.UseSqlServer(dbConfig?.ConnectionString ?? "Server=(localdb)\\mssqllocaldb;Database=ImperiumAO;Integrated Security=true;"));

        return services;
    }

    public static IServiceCollection AddGameServices(this IServiceCollection services)
    {
        // Networking
        services.AddSingleton<IConnectionManager, ConnectionManager>();
        services.AddSingleton<IPacketDispatcher, PacketDispatcher>();
        services.AddHostedService<TcpNetworkServer>();

        // Repositories
        services.AddScoped<IAccountRepository, ImperiumAO.Server.Data.AccountRepository>();
        services.AddScoped<ICharacterRepository, ImperiumAO.Server.Data.CharacterRepository>();

        // Core Game Systems
        services.AddSingleton<IMapManager, MapManager>();
        services.AddSingleton<IWorldManager, WorldManager>();
        services.AddSingleton<IPlayerManager, PlayerManager>();
        services.AddSingleton<INpcManager, NpcManager>();
        services.AddSingleton<GameServer>();

        // Combat & Effects
        services.AddSingleton<ICombatSystem, CombatSystem>();
        services.AddSingleton<IEffectSystem, EffectSystem>();
        services.AddSingleton<IEffectRenderSystem, EffectRenderSystem>();

        // Skills & Professions
        services.AddSingleton<ISkillSystem, SkillSystem>();
        services.AddSingleton<IProfessionSystem, ProfessionSystem>();

        // Inventory & Equipment
        services.AddSingleton<IInventorySystem, InventorySystem>();
        services.AddSingleton<IEquipmentSystem, EquipmentSystem>();
        services.AddSingleton<IItemDropManager, ItemDropManager>();

        // NPC & AI
        services.AddSingleton<IAISystem, AISystem>();
        services.AddSingleton<IPathfinding, AStarPathfinding>();
        services.AddSingleton<INPCDialogSystem, NPCDialogSystem>();

        // Trading
        //services.AddSingleton<ITradingSystem, TradingSystem>();
        //services.AddSingleton<TradeValidator>();

        // Social Systems
        services.AddSingleton<IGuildSystem, GuildSystem>();
        services.AddSingleton<IPartySystem, PartySystem>();
        services.AddSingleton<IChatSystem, ChatSystem>();
        //services.AddSingleton<IDuelSystem, DuelSystem>();

        // Quests & Factions
        services.AddSingleton<IQuestSystem, QuestSystem>();
        services.AddSingleton<IFactionSystem, FactionSystem>();

        // Banking & Crafting
        //services.AddSingleton<IBankingSystem, BankingSystem>();
        //services.AddSingleton<ICraftingSystem, CraftingSystem>();

        // PvP & Admin
        services.AddSingleton<IPvPSystem, PvPSystem>();
        services.AddSingleton<IAdminSystem, AdminSystem>();

        // Magic
        services.AddSingleton<IAdvancedMagicSystem, AdvancedMagicSystem>();

        // Pets & Summons
        services.AddSingleton<IPetSystem, PetSystem>();

        // Forum
        services.AddSingleton<IForumSystem, ForumSystem>();

        // Statistics
        services.AddSingleton<IStatisticsSystem, StatisticsSystem>();

        // World & Persistence
        services.AddSingleton<IWorldSyncSystem, WorldSyncSystem>();
        services.AddSingleton<IPersistenceSystem, PersistenceSystem>();

        // Audio
        services.AddSingleton<IAudioSystem, AudioSystem>();

        // UI
        services.AddSingleton<IUISystem, UISystem>();

        // Auto-register packet handlers
        var handlerTypes = typeof(ServiceConfiguration).Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IPacketHandler).IsAssignableFrom(t));

        foreach (var handlerType in handlerTypes)
            services.AddSingleton(typeof(IPacketHandler), handlerType);

        return services;
    }
}
