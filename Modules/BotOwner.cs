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
    [Remarks("$blacklist BowDown097")]
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
    [Remarks("$cleartextchannel \\#furry-rp")]
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
    [Remarks("$disablecmd eval")]
    public async Task<RuntimeResult> DisableCommand(string cmd)
    {
        string cmdLower = cmd.ToLower();
        if (cmdLower is "disablecmd" or "enablecmd")
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
    [Remarks("$enablecmd disablecmd")]
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
    [Remarks("$eval Context.Channel.SendMessageAsync(\"Mods are fat\");")]
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
    [Remarks("$evalsilent Context.Channel.SendMessageAsync(\"Mods are obese\");")]
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
    [Remarks("$setuserproperty Dragonpreet Cash NaN")]
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

    [Command("setvotes")]
    [Summary("Set a user's votes in an election.")]
    [Remarks("$setvotes 3 BowDown097 1000000")]
    public async Task<RuntimeResult> SetVotes(int electionId, IGuildUser user, int votes)
    {
        QuerySnapshot elections = await Program.database.Collection($"servers/{Context.Guild.Id}/elections").GetSnapshotAsync();
        if (!MemoryCache.Default.Any(k => k.Key.StartsWith("election") && k.Key.EndsWith(electionId.ToString())) && !elections.Any(r => r.Id == electionId.ToString()))
            return CommandResult.FromError("There is no election with that ID!");

        DbConfigChannels channels = await DbConfigChannels.GetById(Context.Guild.Id);
        if (!Context.Guild.TextChannels.Any(channel => channel.Id == channels.ElectionsAnnounceChannel))
            return CommandResult.FromError("This server's election announcement channel has yet to be set or no longer exists.");

        DbElection election = await DbElection.GetById(Context.Guild.Id, electionId);
        election.Candidates[user.Id.ToString()] = votes;
        await Polls.UpdateElection(election, channels, Context.Guild);

        return CommandResult.FromSuccess();
    }

    [Alias("unbotban")]
    [Command("unblacklist")]
    [Summary("Unban a user from using the bot.")]
    [Remarks("$unblacklist \"El Pirata Basado\"")]
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