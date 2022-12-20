namespace account
{
    [GenerateSerializer]
    [Serializable]
    public readonly record struct Relationship(Guid Subject, Guid OtherSubject, string RelationshipType, string Issuer);
}