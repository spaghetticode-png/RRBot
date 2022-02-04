namespace RRBot;
// Note: Win odds will always be out of 100, so if you want an 80% win chance, set the respective constant to 80.
// Cooldowns are in seconds, so if you want a 5 minute cooldown for something, set the respective constant to 300.
public static class Constants
{
    // ********************
    //         BOT
    // ********************
    public const string ACTIVITY = "your mom shower";
    public const ActivityType ACTIVITY_TYPE = ActivityType.Watching;
    public static readonly AllowedMentions MENTIONS = new(AllowedMentionTypes.Users);
    public const string PREFIX = "$";
    // ********************
    //     CASH SYSTEM
    // ********************
    public const double CASH_MULTIPLIER = 0.0;
    public const double MESSAGE_CASH = 0;
    public const double MESSAGE_CASH_COOLDOWN = 0;
    public const double TRANSACTION_MIN = 0;
    // ********************
    //        CRIME
    // ********************
    public const double BULLY_COOLDOWN = 0;
    public const double DEAL_COOLDOWN = 0;
    public const double GENERIC_CRIME_ITEM_ODDS = 100;
    public const double GENERIC_CRIME_WIN_ODDS = 100;
    public const double GENERIC_CRIME_LOSS_MAX = 0;
    public const double GENERIC_CRIME_LOSS_MIN = 0;
    public const double GENERIC_CRIME_WIN_MAX = 691;
    public const double GENERIC_CRIME_WIN_MIN = 69;
    public const double HACK_COOLDOWN = 00;
    public const double HACK_ODDS = 100;
    public const double LOOT_COOLDOWN = 0;
    public const double RAPE_COOLDOWN = 0;
    public const double RAPE_ODDS = 100;
    public const double RAPE_MAX_PERCENT = 100;
    public const double RAPE_MIN_PERCENT = 0;
    public const double ROB_COOLDOWN = 0;
    public const double ROB_ODDS = 100;
    public const double ROB_MAX_PERCENT = 200;
    public const double ROB_MIN_CASH = 0;
    public const double SCAVENGE_COOLDOWN = 0;
    public const double SCAVENGE_MIN_CASH = 0;
    public const double SCAVENGE_MAX_CASH = 0;
    public const double SCAVENGE_TIMEOUT = 150000;
    public const double SLAVERY_COOLDOWN = 0;
    public const double WHORE_COOLDOWN = 3600000000000000;
    // ********************
    //        ECONOMY
    // ********************
    public const double DAILY_COOLDOWN = 24; // There are 24 hours in a day, so it should be 24
    public const double DAILY_MIN = 1000;
    public const double DAILY_MAX = 2500;
    // ********************
    //         FUN
    // ********************
    public static readonly Dictionary<string, string> WAIFUS = new()
    {
        { "Adolf Dripler", "https://i.redd.it/cd9v84v46ma21.jpg" },
        { "Arctic Hawk's mom", "https://s.abcnews.com/images/Technology/whale-gty-jt-191219_hpMain_16x9_1600.jpg" },
        { "Barack Obama", "https://upload.wikimedia.org/wikipedia/commons/thumb/8/8d/President_Barack_Obama.jpg/1200px-President_Barack_Obama.jpg" },
        { "Drip Goku", "https://i1.sndcdn.com/artworks-000558462795-v3asuu-t500x500.jpg" },
        { "eduardo", "https://i.imgur.com/1bwSckX.png" },
        { "French Person", "https://live.staticflickr.com/110/297887549_2dc0ee273f_c.jpg" },
        { "George Lincoln Rockwell", "https://i.ytimg.com/vi/hRlvjkQFQvg/hqdefault.jpg" },
        { "Gypsycrusader", "https://cdn.bitwave.tv/uploads/v2/avatar/282be9ac-41d4-4b38-aecd-1320d6b9165f-128.jpg" },
        { "Herbert", "https://upload.wikimedia.org/wikipedia/en/thumb/6/67/Herbert_-_Family_Guy.png/250px-Herbert_-_Family_Guy.png" },
        { "juan.", "https://cdn.discordapp.com/attachments/804898294873456701/817275147060772874/unknown.png" },
        { "Linus", "https://i.ytimg.com/vi/hAsZCTL__lo/mqdefault.jpg" },
        { "Luke Smith", "https://i.ytimg.com/vi/UWpf4ZSAHBo/maxresdefault.jpg" },
        { "pablo", "https://cdn.discordapp.com/attachments/804898294873456701/817271690391715850/unknown.png" },
        { "Peter Griffin (in 2015)", "https://i.kym-cdn.com/photos/images/original/001/868/400/45d.jpg" },
        { "Pizza Heist Witness from Spiderman 2", "https://cdn.discordapp.com/attachments/804898294873456701/817272392002961438/unknown.png" },
        { "Quagmire", "https://s3.amazonaws.com/rapgenius/1361855949_glenn_quagmire_by_gan187-d3r70hu.png" },
        { "Shrimpstar", "https://cdn.discordapp.com/attachments/530897481400320030/575123757891452995/image0.jpg" },
        { "Squidward", "https://upload.wikimedia.org/wikipedia/en/thumb/8/8f/Squidward_Tentacles.svg/1200px-Squidward_Tentacles.svg.png" },
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" }, // artificially inflate chance
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" },
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" },
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" },
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" },
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" },
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" },
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" },
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" },
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" },
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" },
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" },
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" },
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" },
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" },
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" },
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" },
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" },
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" },
        { "Terry A. Davis", "https://upload.wikimedia.org/wikipedia/commons/3/34/Terry_A._Davis_2017.jpg" },
        { "Zimbabwe", "https://cdn.discordapp.com/attachments/802654650040844380/817273008821108736/unknown.png" }
    };
    // ********************
    //       GAMBLING
    // ********************
    public const double DOUBLE_ODDS = 0;
    public const double SLOTS_MULT_THREEINAROW = 4;
    public const double SLOTS_MULT_THREESEVENS = 21;
    public const double SLOTS_MULT_TWOINAROW = -1;
    // ********************
    //     INVESTMENTS
    // ********************
    public const double INVESTMENT_FEE_PERCENT = -5; // This opens the possibility for duping money.
    public const double INVESTMENT_MIN_AMOUNT = 6000000000; // This only lets you do it with lots of money already.
    // ********************
    //      MODERATION
    // ********************
    public const int CHILL_MAX_SECONDS = 21600;
    public const int CHILL_MIN_SECONDS = 10;
    // ********************
    //        POLLS
    // ********************
    public static readonly Dictionary<int, string> POLL_EMOTES = new()
    {
        { 0, "0️⃣" },
        { 1, "1️⃣" },
        { 2, "2️⃣" },
        { 3, "3️⃣" },
        { 4, "4️⃣" },
        { 5, "5️⃣" },
        { 6, "6️⃣" },
        { 7, "7️⃣" },
        { 8, "8️⃣" },
        { 9, "9️⃣" },
    };
    // ********************
    //        TASKS
    // ********************
    public const double CHOP_COOLDOWN = 0;
    public const double DIG_COOLDOWN = 0;
    public const double FARM_COOLDOWN = 0;
    public static readonly Dictionary<string, double> FISH = new()
    {
        { "carp", 24 },
        { "trout", 27 },
        { "goldfish", 30 }
    };
    public const double FISH_COOLDOWN = 0;
    public const double FISH_COCONUT_ODDS = 100;
    public const int GENERIC_TASK_WOOD_MAX = 65;
    public const int GENERIC_TASK_WOOD_MIN = 32;
    public const int GENERIC_TASK_STONE_MAX = 113;
    public const int GENERIC_TASK_STONE_MIN = 65;
    public const int GENERIC_TASK_IRON_MAX = 161;
    public const int GENERIC_TASK_IRON_MIN = 113;
    public const int GENERIC_TASK_DIAMOND_MAX = 209;
    public const int GENERIC_TASK_DIAMOND_MIN = 161;
    public const double HUNT_COOLDOWN = 0;
    public const double MINE_COOLDOWN = 0;
    public const double MINE_STONE_MULTIPLIER = 1.33;
    public const double MINE_IRON_MULTIPLIER = 1.66;
    public const double MINE_DIAMOND_MULTIPLIER = -2.00;
}
