namespace RRBot.Modules;
public class FunnyContext
{
    public SocketCommandContext Context;
    public FunnyContext(SocketCommandContext context) => Context = context;
}

[RequireOwner]
[Summary("Commands for bot owners only.")]
public class BotOwner : ModuleBase<SocketCommandContext>
{
    public CommandService Commands { get; set; }

    [Alias("botban")]
    [Command("blacklist")]
    [Summary("Ban a user from using the bot.")]
    [Remarks("$blacklist [user]")]
    public async Task<RuntimeResult> Blacklist(IGuildUser user)
    {
        if (user.IsBot)
            return CommandResult.FromError("Nope.");

        DbGlobalConfig globalConfig = await DbGlobalConfig.Get();
        globalConfig.BannedUsers.Add(user.Id);
        await Context.User.NotifyAsync(Context.Channel, $"Blacklisted {user.Sanitize()}.");
        return CommandResult.FromSuccess();
    }

    [Command("cleartextchannel")]
    [Summary("Deletes and recreates a text channel, effectively wiping its messages.")]
    [Remarks("$cleartextchannel [channel]")]
    public async Task ClearTextChannel(ITextChannel channel)
    {
        await channel.DeleteAsync();
        await Context.Guild.CreateTextChannelAsync(channel.Name, properties => {
            properties.CategoryId = channel.CategoryId;
            properties.IsNsfw = channel.IsNsfw;
            properties.Name = channel.Name;
            properties.PermissionOverwrites = new Optional<IEnumerable<Overwrite>>(channel.PermissionOverwrites.AsEnumerable());
            properties.Position = channel.Position;
            properties.SlowModeInterval = channel.SlowModeInterval;
            properties.Topic = channel.Topic;
        });
    }

    [Command("disablecmd")]
    [Summary("Disable a command.")]
    [Remarks("$disablecmd [cmd]")]
    public async Task<RuntimeResult> DisableCommand(string cmd)
    {
        string cmdLower = cmd.ToLower();
        if (cmdLower == "disablecmd")
            return CommandResult.FromError("I don't think that's a good idea.");

        Discord.Commands.SearchResult search = Commands.Search(cmd);
        if (!search.IsSuccess)
            return CommandResult.FromError($"**${cmdLower}** is not a command!");

        DbGlobalConfig globalConfig = await DbGlobalConfig.Get();
        globalConfig.DisabledCommands.Add(cmdLower);
        await Context.User.NotifyAsync(Context.Channel, $"Disabled ${cmdLower}.");
        return CommandResult.FromSuccess();
    }

    [Command("enablecmd")]
    [Summary("Enable a previously disabled command.")]
    [Remarks("$enablecmd [cmd]")]
    public async Task<RuntimeResult> EnableCommand(string cmd)
    {
        string cmdLower = cmd.ToLower();
        DbGlobalConfig globalConfig = await DbGlobalConfig.Get();
        if (!globalConfig.DisabledCommands.Contains(cmdLower))
            return CommandResult.FromError($"**{cmdLower}** is either not a command or is not disabled!");

        globalConfig.DisabledCommands.Remove(cmdLower);
        await Context.User.NotifyAsync(Context.Channel, $"Enabled ${cmdLower}.");
        return CommandResult.FromSuccess();
    }

    [Command("eval")]
    [Summary("Execute C# code.")]
    [Remarks("$eval [code]")]
    public async Task<RuntimeResult> Eval([Remainder] string code)
    {
        try
        {
            code = code.Replace("```cs", "").Trim('`');
            string[] imports = { "System", "System.Collections.Generic", "System.Text" };
            object evaluation = await CSharpScript.EvaluateAsync(code, ScriptOptions.Default.WithImports(imports), new FunnyContext(Context));

            EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Red)
                .WithTitle("Code Evaluation")
                .WithDescription($"Your code, ```cs\n{code}``` evaluates to: ```cs\n\"{evaluation}\"```");
            await ReplyAsync(embed: embed.Build());
            return CommandResult.FromSuccess();
        }
        catch (CompilationErrorException cee)
        {
            return CommandResult.FromError($"Compilation error: ``{cee.Message}``");
        }
        catch (Exception e) when (e is not NullReferenceException)
        {
            return CommandResult.FromError($"Other error: ``{e.Message}``");
        }
    }

    [Command("evalsilent")]
    [Summary("Evaluate C# code with no output.")]
    [Remarks("$evalsilent [code]")]
    public async Task EvalSilent([Remainder] string code)
    {
        await Context.Message.DeleteAsync();
        code = code.Replace("```cs", "").Trim('`');
        string[] imports = { "System", "System.Collections.Generic", "System.Text" };
        await CSharpScript.EvaluateAsync(code, ScriptOptions.Default.WithImports(imports), new FunnyContext(Context));
    }

    [Alias("setuserproperty")]
    [Command("setuserproperty")]
    [Summary("Set a property for a specific user in the database.")]
    [Remarks("$setuserproperty [user] [property] [value]")]
    public async Task<RuntimeResult> SetUserProperty(IGuildUser user, string property, [Remainder] string value)
    {
        DbUser dbUser = await DbUser.GetById(Context.Guild.Id, user.Id);
        try
        {
            dbUser[property] = Convert.ChangeType(value, dbUser[property].GetType());
            await Context.User.NotifyAsync(Context.Channel, $"Set {property} to ``{value}`` for {user.Sanitize()}.");
            return CommandResult.FromSuccess();
        }
        catch (Exception e)
        {
            return CommandResult.FromError($"Couldn't set property: {e.Message}");
        }
    }

    [Alias("unbotban")]
    [Command("unblacklist")]
    [Summary("Unban a user from using the bot.")]
    [Remarks("$unblacklist [user]")]
    public async Task<RuntimeResult> Unblacklist(IGuildUser user)
    {
        if (user.IsBot)
            return CommandResult.FromError("Nope.");

        DbGlobalConfig globalConfig = await DbGlobalConfig.Get();
        globalConfig.BannedUsers.Remove(user.Id);
        await Context.User.NotifyAsync(Context.Channel, $"Unblacklisted {user.Sanitize()}.");
        return CommandResult.FromSuccess();
    }

    [Command("updatedb")]
    [Summary("Pushes all cached data to the database.")]
    [Remarks("$updatedb")]
    public async Task UpdateDB()
    {
        long count = MemoryCache.Default.GetCount();
        foreach (string key in MemoryCache.Default.Select(kvp => kvp.Key))
        {
            try
            {
                if (MemoryCache.Default.Get(key) is not DbObject item)
                    continue;

                await item.Reference.SetAsync(item);
                MemoryCache.Default.Remove(key);
            }
            catch (NullReferenceException) {}
        }

        await Context.User.NotifyAsync(Context.Channel, $"Pushed all **{count}** items in the cache to the database.");
    }
}