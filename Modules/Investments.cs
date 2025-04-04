﻿namespace RRBot.Modules;
[Summary("Invest in our selection of coins, Bit or Shit. The prices here are updated in REAL TIME with the REAL LIFE values.")]
public partial class Investments : ModuleBase<SocketCommandContext>
{
    [Command("invest")]
    [Summary("Invest in a cryptocurrency. Currently accepted currencies are BTC, ETH, LTC, and XRP. Here, the amount you put in should be RR Cash.")]
    [Remarks("$invest ethereum 600")]
    [RequireCash]
    public async Task<RuntimeResult> Invest(string crypto, decimal amount)
    {
        if (amount < Constants.TransactionMin)
            return CommandResult.FromError($"You need to invest at least {Constants.TransactionMin:C2}.");

        string? abbreviation = ResolveAbbreviation(crypto);
        if (abbreviation is null)
            return CommandResult.FromError("That is not a currently accepted currency!");
        
        DbUser user = await MongoManager.FetchUserAsync(Context.User.Id, Context.Guild.Id);
        if (user.Cash < amount)
            return CommandResult.FromError("You can't invest more than what you have!");

        decimal cryptoValue = await QueryCryptoValue(abbreviation);
        decimal cryptoAmount = amount / cryptoValue;
        if (cryptoAmount < Constants.InvestmentMinAmount)
        {
            return CommandResult.FromError($"The amount you specified converts to less than {Constants.InvestmentMinAmount} of {abbreviation.ToUpper()}, which is not permitted.\n"
                + $"You'll need to invest at least **{cryptoValue * Constants.InvestmentMinAmount:C2}**.");
        }

        CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
        culture.NumberFormat.CurrencyNegativePattern = 2;
        await user.SetCash(Context.User, user.Cash - amount);
        user[abbreviation] = (decimal)user[abbreviation] + Math.Round(cryptoAmount, 4);
        user.AddToStats(new Dictionary<string, string>
        {
            { $"Money Put Into {abbreviation.ToUpper()}", amount.ToString("C2", culture) },
            { $"{abbreviation.ToUpper()} Purchased", cryptoAmount.ToString("0.####") }
        });

        await Context.User.NotifyAsync(Context.Channel, $"You invested in **{cryptoAmount:0.####}** {abbreviation.ToUpper()}, currently valued at **{amount:C2}**.");
        await MongoManager.UpdateObjectAsync(user);
        return CommandResult.FromSuccess();
    }

    [Command("investments")]
    [Summary("Check your investments, or someone else's, and their value.")]
    [Remarks("$investments gurrenm4")]
    public async Task<RuntimeResult> InvestmentsView([Remainder] IGuildUser? user = null)
    {
        if (user?.IsBot == true)
            return CommandResult.FromError("Nope.");
            
        DbUser dbUser = await MongoManager.FetchUserAsync(user?.Id ?? Context.User.Id, Context.Guild.Id);

        StringBuilder investments = new();
        if (dbUser.Btc >= Constants.InvestmentMinAmount)
            investments.AppendLine($"**Bitcoin (BTC)**: {dbUser.Btc:0.####} ({await QueryCryptoValue("BTC") * dbUser.Btc:C2})");
        if (dbUser.Eth >= Constants.InvestmentMinAmount)
            investments.AppendLine($"**Ethereum (ETH)**: {dbUser.Eth:0.####} ({await QueryCryptoValue("ETH") * dbUser.Eth:C2})");
        if (dbUser.Ltc >= Constants.InvestmentMinAmount)
            investments.AppendLine($"**Litecoin (LTC)**: {dbUser.Ltc:0.####} ({await QueryCryptoValue("LTC") * dbUser.Ltc:C2})");
        if (dbUser.Xrp >= Constants.InvestmentMinAmount)
            investments.AppendLine($"**XRP**: {dbUser.Xrp:0.####} ({await QueryCryptoValue("XRP") * dbUser.Xrp:C2})");

        EmbedBuilder embed = new EmbedBuilder()
            .WithColor(Color.Red)
            .WithTitle("Investments")
            .WithDescription(investments.Length > 0 ? investments.ToString() : "None");
        await ReplyAsync(embed: embed.Build());
        return CommandResult.FromSuccess();
    }

    [Alias("values")]
    [Command("prices")]
    [Summary("Check the values of currently available cryptocurrencies.")]
    public async Task Prices()
    {
        decimal btc = await QueryCryptoValue("BTC");
        decimal eth = await QueryCryptoValue("ETH");
        decimal ltc = await QueryCryptoValue("LTC");
        decimal xrp = await QueryCryptoValue("XRP");

        EmbedBuilder embed = new EmbedBuilder()
            .WithColor(Color.Red)
            .WithTitle("Cryptocurrency Values")
            .RrAddField("Bitcoin (BTC)", btc.ToString("C2"))
            .RrAddField("Ethereum (ETH)", eth.ToString("C2"))
            .RrAddField("Litecoin (LTC)", ltc.ToString("C2"))
            .RrAddField("XRP", xrp.ToString("C2"));
        await ReplyAsync(embed: embed.Build());
    }

    [Command("withdraw")]
    [Summary("Withdraw a specified cryptocurrency to RR Cash, with a 2% withdrawal fee. Here, the amount you put in should be in the crypto, not RR Cash. See $invest's help info for currently accepted currencies.")]
    [Remarks("$withdraw ltc 10")]
    public async Task<RuntimeResult> Withdraw(string crypto, decimal amount)
    {
        if (amount < Constants.InvestmentMinAmount)
            return CommandResult.FromError($"You must withdraw {Constants.InvestmentMinAmount} or more of the crypto.");

        string? abbreviation = ResolveAbbreviation(crypto);
        if (abbreviation is null)
            return CommandResult.FromError("That is not a currently accepted currency!");
            
        DbUser user = await MongoManager.FetchUserAsync(Context.User.Id, Context.Guild.Id);
        decimal cryptoBal = (decimal)user[abbreviation];
        if (cryptoBal < Constants.InvestmentMinAmount)
            return CommandResult.FromError($"You have no {abbreviation.ToUpper()}!");
        if (cryptoBal < amount)
            return CommandResult.FromError($"You don't have {amount} {abbreviation.ToUpper()}! You've only got **{cryptoBal:0.####}** of it.");

        decimal cryptoValue = await QueryCryptoValue(abbreviation) * amount;
        decimal finalValue = cryptoValue / 100.0m * (100 - Constants.InvestmentFeePercent);

        CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
        culture.NumberFormat.CurrencyNegativePattern = 2;

        await user.SetCash(Context.User, user.Cash + finalValue);
        user[abbreviation] = (decimal)user[abbreviation] - Math.Round(amount, 4);
        user.AddToStat($"Money Gained from {abbreviation.ToUpper()}", finalValue.ToString("C2", culture));

        await Context.User.NotifyAsync(Context.Channel, $"You withdrew **{amount:0.####}** {abbreviation.ToUpper()}, currently valued at **{cryptoValue:C2}**.\n" +
                                                        $"A {Constants.InvestmentFeePercent}% withdrawal fee was taken from this amount, leaving you **{finalValue:C2}** richer.");
        await MongoManager.UpdateObjectAsync(user);
        return CommandResult.FromSuccess();
    }
}