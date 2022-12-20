using Orleans;

namespace account
{

    [GenerateSerializer]
    [Serializable]
    public class NHSNumber : INHSNumber
    {

        public NHSNumber(string number, DateOnly dateCreated)
        {
            Number = number;
            DateCreated = dateCreated;
        }
        
        [Id(0)]
        public string Number { get; set; }

        [Id(1)]
        public DateOnly DateCreated { get; set; }
    }
}