namespace Dolany.Database.Ai
{
    public class SignInGroupRecord : DbBaseEntity
    {
        public long GroupNum { get; set; }

        public string Content { get; set; } = "签到";

        public void Update()
        {
            MongoService<SignInGroupRecord>.Update(this);
        }
    }
}
