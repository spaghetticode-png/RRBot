namespace RRBot.Modules;

[Summary("Exclusive to Rush Academy")]
public class RushAcademy : ModuleBase<SocketCommandContext>
{
    public static readonly string[] Maps = [
        "A Game",
        "Battle Sands",
        "Blastapopoulos",
        "Cards",
        "Concrete Alley",
        "Dreadbloon",
        "Frozen River",
        "Farmyard",
        "Ghostly Coast",
        "Hydro Dam",
        "Hot Tub",
        "Inlets",
        "Interchange",
        "Mountain Pass",
        "Offside",
        "Pinball Wizard",
        "Riverside",
        "Sprinkler",
        "Super Monkey Lane",
        "Swan Lake",
        "Swamp",
        "Snowy Castle",
        "Temple",
        "Wattle Resorts",
        "Wizard's Keep",
        "Yellow Brick Road",
        "Yin Yang"
    ];

    public static readonly string[] Strats = [
        "Dart Farm Cobra",
        "Cobra Bomb Village",
        "Dart Farm Spac",
        "Tack Farm Spac",
        "Tack Village Spac",
        "Wiz Farm Spac",
        "Dart Chipper Spac",
        "Dart Cobra Chipper",
        "Engi Farm Super",
        "Boat Farm Spac",
        "Boat Farm Chipper",
        "Sub Farm Chipper",
        "Sniper Bomb Mortar",
        "Cobra Mortar Sniper",
        "Dart Farm Boat",
        "Tack Village Cobra",
        "Boomer Farm Village",
        "Glue Farm Spac",
        "Boat Chipper Spac",
        "OG Cobra Boomer Village",
        "Dart Farm Tack",
        "Dart Boat Tack",
        "Tack Spac Boomer",
        "Ice Glue Spac",
        "Dartling Glue Ice",
        "dartling farm village",
        "Chipper Spac Glue",
        "Sub Spac Ace",
        "Glue Boomer Ace",
        "Boomer Farm Ace",
        "Engi Cobra Spac",
        "Glue Ice Spac",
        "Dart Farm Village",
        "Boat Farm Village"
    ];

    [Command("map")]
    public async Task Map()
    {
        await ReplyAsync($"The map is: {RandomUtil.GetRandomElement(Maps)}");
    }

    [Command("memestrat")]
    public async Task MemeStrat()
    {
        await ReplyAsync($"Your meme strategy is: {RandomUtil.GetRandomElement(Strats)}");
    }
}