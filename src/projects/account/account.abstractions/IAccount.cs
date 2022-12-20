using System.Threading.Tasks;
using Orleans;

namespace account
{
    public interface IAccount : IGrainWithGuidKey
    {
        Task<IEnumerable<IAccountUser>> GetAccountUsers();
        Task<IAccountUser> AddAccountUser(IAccountUser user);
        Task<INHSNumber> SetNhsNumber(INHSNumber nhsNumber);
        Task<INHSNumber> GetNhsNumber();
        Task<Relationship> AddRelationship(Relationship relationship);
    }

    public interface IAccountUser
    {
        string ? ExternalUserId{get;set;}
        Guid UserId {get;set;}
        string UserAlias {get;set;}
    }
}