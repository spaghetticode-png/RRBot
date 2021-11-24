namespace RRBot.Entities.Database
{
    [FirestoreData]
    public class DbConfigChannels
    {
        [FirestoreDocumentId]
        public DocumentReference Reference { get; set; }
        [FirestoreProperty("logsChannel")]
        public ulong LogsChannel { get; set; }
        [FirestoreProperty("pollsChannel")]
        public ulong PollsChannel { get; set; }

        public static async Task<DbConfigChannels> GetById(ulong guildId)
        {
            DocumentReference doc = Program.database.Collection($"servers/{guildId}/config").Document("channels");
            DocumentSnapshot snap = await doc.GetSnapshotAsync();
            if (!snap.Exists)
            {
                await doc.CreateAsync(new { logsChannel = 0UL });
                return await GetById(guildId);
            }

            return snap.ConvertTo<DbConfigChannels>();
        }

        public async Task Write() => await Reference.SetAsync(this);
    }
}