namespace dhc;

public readonly record struct Postcode
{
    public string Value {get;init;}

    public Postcode(string value)
    {
        Value = value;
    }

    public static Postcode Parse(string value)
    {
        return new Postcode(value);
    }
}