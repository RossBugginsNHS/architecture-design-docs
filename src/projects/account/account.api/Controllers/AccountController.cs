using System.Collections.ObjectModel;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace account.api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{

    private readonly ILogger<AccountController> _logger;
   readonly IBus _bus;

    public AccountController(
        IBus bus,
        ILogger<AccountController> logger)
    {
        _logger = logger;
          _bus = bus;
    }

    [HttpPost("{nhsNumber}", Name = "PostCreateNewAccount")]
    public async Task<CreateNewAccount> Post(string nhsNumber, CancellationToken stoppingToken = default)
    {
         var endpoint = await _bus.GetSendEndpoint(new Uri("queue:create-new-account"));

         var payload =  new CreateNewAccount {NHSNumber = nhsNumber, AccountId = Guid.NewGuid()};

          await endpoint.Send<CreateNewAccount>(
                    payload,
                    stoppingToken);
        
        _logger.LogInformation("Send Command Create Account for {account} and {nhsnumber}", 
            payload.AccountId, payload.NHSNumber);

        return payload;
    }

    [HttpGet("{nhsNumber}", Name = "GetAccount")]
    public async Task<Account> Get(Guid accountId, CancellationToken stoppingToken = default)
    { 
        var account = new Account();
        var a = account with {AccountInfo  = new (accountId, accountId.ToString())};
        return a;
    }  

    
}

/// <summary>
/// 
/// </summary>
/// <param name="AccountInfo"></param>
/// <param name="Users"></param>
/// <param name="UserIdentites"></param>
/// <param name="Roles"></param>
/// <param name="UserRoles"></param>
/// <returns></returns>
public record Account(
    AccountInfo AccountInfo = default(AccountInfo), 
    ReadOnlyCollection<User> Users = null, 
    ReadOnlyCollection<UserIdentity> UserIdentites = null, 
    ReadOnlyCollection<Role> Roles = null, 
    ReadOnlyCollection<UserRole> UserRoles = null,
    ReadOnlyCollection<Client> Clients = null,
    ReadOnlyCollection<UserClient> UserClients = null,
    ReadOnlyCollection<Scope> Scopes = null,
    ReadOnlyCollection<UserScope> UserScopes = null,
    ReadOnlyCollection<ClientScope> ClientScopes = null);

public readonly record struct AccountInfo (Guid AccountId, string NhsNumber);


public readonly record struct User (Guid AccountId, Guid UserId, string UserAliasName);


public readonly record struct UserIdentity (Guid AccountId, Guid UserId, string IdentityId, string IdentityProviderName);


public readonly record struct Role (Guid AccountId, Guid RoleId, string RoleName);


public readonly record struct UserRole (Guid AccountId, Guid UserId, Guid RoleId);


public readonly record struct Client (Guid AccountId, Guid ClientId, string ClientName);


public readonly record struct UserClient (Guid AccountId, Guid ClientId, Guid UserId);

/// <summary>
/// A scope that is avaliable in the account context
/// </summary>
public readonly record struct Scope (Guid AccountId, string ScopeName);

/// <summary>
/// A Scope that a user is able to grant to an application.
/// </summary>
public readonly record struct UserScope (Guid AccountId, Guid UserId, string ScopeName);

/// <summary>
/// 
/// </summary>
public readonly record struct ClientScope (Guid AccountId, Guid ClientId, string ScopeName);




