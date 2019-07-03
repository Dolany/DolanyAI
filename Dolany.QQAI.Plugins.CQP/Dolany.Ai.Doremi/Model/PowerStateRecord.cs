using Dolany.Database;

namespace Dolany.Ai.Doremi.Model
{
    public class PowerStateRecord : DbBaseEntity
    {
        public string AiName { get; set; }

        public bool IsPowerOn { get; set; }

        public static PowerStateRecord Get(string AiName)
        {
            var record = MongoService<PowerStateRecord>.GetOnly(p => p.AiName == AiName);
            if (record != null)
            {
                return record;
            }

            record = new PowerStateRecord(){AiName = AiName, IsPowerOn = true};
            MongoService<PowerStateRecord>.Insert(record);
            return record;
        }

        public void Update()
        {
            MongoService<PowerStateRecord>.Update(this);
        }

        public override string ToString()
        {
            return $"AiName: {AiName};IsPowerOn:{IsPowerOn}";
        }
    }
}
