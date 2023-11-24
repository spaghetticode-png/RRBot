﻿using Discord.Interactions;
using Microsoft.Extensions.Logging;
using RRBot;

DiscordShardedClient client = new(new DiscordSocketConfig
{
    AlwaysDownloadUsers = true,
    GatewayIntents = GatewayIntents.All,
    LargeThreshold = 250,
    MessageCacheSize = 1500
});

HostApplicationBuilder builder = new(args);
builder.Services.AddSingleton(client)
    .AddSingleton<CommandService>()
    .AddSingleton<InteractionService>()
    .AddSingleton<InteractiveService>()
    .AddHostedService<DiscordClientHost>()
    .AddLavalink()
    .AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Warning))
    .AddInactivityTracking()
    .ConfigureInactivityTracking(options => options.DefaultTimeout = TimeSpan.FromSeconds(Constants.InactivityTimeoutSecs))
    .AddSingleton<AudioSystem>();

builder.Build().Run();