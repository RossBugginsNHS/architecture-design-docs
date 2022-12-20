//public readonly record struct NHSNumber(string number, DateOnly dateCreated);
namespace account
{

    public interface INHSNumber
    {
        string Number { get; set; }
        DateOnly DateCreated { get; set; }
    }
}
