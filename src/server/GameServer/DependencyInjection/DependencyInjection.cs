//
// Copyright (C) 2013-2023 getMaNGOS <https://getmangos.eu>
//
// This program is free software. You can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation. either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY. Without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//

using GameServer.Handlers;
using GameServer.Network;
using GameServer.Requests;
using GameServer.Services;
using Mangos.Cluster;
using Mangos.Cluster.DataStores;
using Mangos.Cluster.Globals;
using Mangos.Cluster.Handlers;
using Mangos.Cluster.Handlers.Guild;
using Mangos.Cluster.Network;
using Mangos.Common.Globals;
using Mangos.Configuration;
using Mangos.DataStores;
using Mangos.Logging.DependencyInjection;
using Mangos.MySql.DependencyInjection;
using Mangos.Tcp;
using Mangos.World;
using Mangos.World.Auction;
using Mangos.World.Battlegrounds;
using Mangos.World.DataStores;
using Mangos.World.Gossip;
using Mangos.World.Handlers;
using Mangos.World.Loots;
using Mangos.World.Maps;
using Mangos.World.Network;
using Mangos.World.Objects;
using Mangos.World.Player;
using Mangos.World.Server;
using Mangos.World.Social;
using Mangos.World.Spells;
using Mangos.World.Warden;
using Mangos.World.Weather;
using Mangos.Zip;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace GameServer.DependencyInjection;
public static class DependencyInjection
{
    private const string ConfigurationFileName = "configuration.json";

    public static IServiceCollection AddConfigurationFile(this IServiceCollection services)
    {
        services.AddSingleton(provider =>
        {
            if (!File.Exists(ConfigurationFileName))
                throw new FileNotFoundException($"Unable to locate {ConfigurationFileName}");

            var json = File.ReadAllText(ConfigurationFileName);
            var config = JsonSerializer.Deserialize<MangosConfiguration>(json)
                ?? throw new Exception($"Unable to deserialize {ConfigurationFileName}");

            return config;
        });

        return services;
    }

    public static IServiceCollection AddCustomLogging(this IServiceCollection services)
    {
        services.AddMangosLogger();

        return services;
    }

    public static IServiceCollection AddMySqlDatabase(this IServiceCollection services)
    {
        services.AddDatabase();

        return services;
    }

    public static IServiceCollection AddTcpServer(this IServiceCollection services)
    {
        services.AddSingleton<TcpServer>();

        return services;
    }

    public static IServiceCollection AddGameModule(this IServiceCollection services)
    {
        services.AddScoped<ITcpConnection, GameTcpConnection>();
        services.AddScoped<IGameState, GameState>();

        services.AddScoped<CMSG_PING_Handler>();
        services.AddScoped<IHandlerDispatcher, HandlerDispatcher<CMSG_PING, CMSG_PING_Handler>>();

        return services;
    }

    public static IServiceCollection AddLegacyClusterServices(this IServiceCollection services)
    {
        services.AddScoped<ClientClass>();

        services.AddSingleton<DataStoreProvider>();
        services.AddSingleton<MangosGlobalConstants>();
        services.AddSingleton<Mangos.Common.Legacy.Globals.Functions>();
        services.AddSingleton<Mangos.Common.Legacy.Functions>();
        services.AddSingleton<Mangos.Cluster.Globals.Functions>();
        services.AddSingleton<ZipService>();
        services.AddSingleton<Mangos.World.Warden.NativeMethods>();
        services.AddSingleton<LegacyWorldCluster>();
        services.AddSingleton<WorldServerClass>();
        services.AddSingleton<WsDbcDatabase>();
        services.AddSingleton<WsDbcLoad>();
        services.AddSingleton<Packets>();
        services.AddSingleton<WcGuild>();
        services.AddSingleton<WcNetwork>();
        services.AddSingleton<WcHandlers>();
        services.AddSingleton<WcHandlersAuth>();
        services.AddSingleton<WcHandlersBattleground>();
        services.AddSingleton<WcHandlersChat>();
        services.AddSingleton<WcHandlersGroup>();
        services.AddSingleton<WcHandlersGuild>();
        services.AddSingleton<WcHandlersMisc>();
        services.AddSingleton<WcHandlersMovement>();
        services.AddSingleton<WcHandlersSocial>();
        services.AddSingleton<WcHandlersTickets>();
        services.AddSingleton<WsHandlerChannels>();
        services.AddSingleton<WcHandlerCharacter>();
        services.AddSingleton<WcHandlersGuild>();
        services.AddSingleton<WcHandlersGuild>();
        services.AddSingleton<WcHandlersGuild>();

        // ClusterServiceLocator: handle circular dependencies manually
        services.AddSingleton(provider =>
        {
            var locator = new ClusterServiceLocator();
            return locator;
        });

        return services;
    }

    public static IServiceCollection AddLegacyWorldServices(this IServiceCollection services)
    {
        services.AddSingleton<MangosGlobalConstants>();
        services.AddSingleton<Mangos.Common.Legacy.Globals.Functions>();
        services.AddSingleton<Mangos.Common.Legacy.Functions>();
        services.AddSingleton<ZipService>();
        services.AddSingleton<Mangos.Common.Legacy.NativeMethods>();
        services.AddSingleton<WorldServer>();
        services.AddSingleton<Mangos.Cluster.Globals.Functions>();
        services.AddSingleton<DataStoreProvider>();

        services.AddSingleton<Mangos.World.AI.WS_Creatures_AI>();
        services.AddSingleton<WS_Auction>();
        services.AddSingleton<WS_Battlegrounds>();
        services.AddSingleton<WS_DBCDatabase>();
        services.AddSingleton<WS_DBCLoad>();
        services.AddSingleton<Packets>();
        services.AddSingleton<WS_GuardGossip>();
        services.AddSingleton<WS_Loot>();
        services.AddSingleton<WS_Maps>();
        services.AddSingleton<WS_Corpses>();
        services.AddSingleton<WS_Creatures>();
        services.AddSingleton<WS_DynamicObjects>();
        services.AddSingleton<WS_GameObjects>();
        services.AddSingleton<WS_Items>();
        services.AddSingleton<WS_NPCs>();
        services.AddSingleton<WS_Pets>();
        services.AddSingleton<WS_Transports>();
        services.AddSingleton<CharManagementHandler>();
        services.AddSingleton<WS_CharMovement>();
        services.AddSingleton<WS_Combat>();
        services.AddSingleton<WS_Commands>();
        services.AddSingleton<WS_Handlers>();
        services.AddSingleton<WS_Handlers_Battleground>();
        services.AddSingleton<WS_Handlers_Chat>();
        services.AddSingleton<WS_Handlers_Gamemaster>();
        services.AddSingleton<WS_Handlers_Instance>();
        services.AddSingleton<WS_Handlers_Misc>();
        services.AddSingleton<WS_Handlers_Taxi>();
        services.AddSingleton<WS_Handlers_Trade>();
        services.AddSingleton<WS_Handlers_Warden>();
        services.AddSingleton<WS_Player_Creation>();
        services.AddSingleton<WS_Player_Initializator>();
        services.AddSingleton<WS_PlayerData>();
        services.AddSingleton<WS_PlayerHelper>();
        services.AddSingleton<WS_Network>();
        services.AddSingleton<WS_TimerBasedEvents>();
        services.AddSingleton<WS_Group>();
        services.AddSingleton<WS_Guilds>();
        services.AddSingleton<WS_Mail>();
        services.AddSingleton<WS_Spells>();
        services.AddSingleton<WS_Warden>();
        services.AddSingleton<WS_Weather>();

        return services;
    }
}
