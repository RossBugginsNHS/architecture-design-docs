---
title: Auth
sub_title: Understanding Auth
layout: page
nav_order: 2.0001
has_children: true
---

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
1. TOC
{:toc}
</details>


## Notes for Author - Aims / Objectives / Direction / Plan

- Making Auth accessible to wider audience
- Deep dive sections 
- Also appeal to those with tech background
- Include a case study - how could / should be done
- Need simple diagrams / images
- Need simple example scenarios
- Need a short summary version, and then a longer version with much more detail

### Objectives

- Provide non tech staff with a understanding 
- Provide details for tech staff to find interesting and to enhance existing knowledge
- Outline architectural design for a NHS Scenario, how to best use tech? "Proxy?"
- Use dev background, provide working code examples and deployments of how this fits together is "best case" scenario

## Summary

Authentication, Authorisation, OAuth, OpenId, Enforcement, Decisions, Credentials, Claims, JWT, Grants, Flows, Bearer, Tokens, Scopes, Policies, Roles, Attributes - the list could go on. Auth is hard. It is hard at times for people with a technical background, developers, engineers and architects. It's just as hard for project managers, business analystics and product teams. 

The aim of this paper is to remove some of the magic and mystery that surrounds Auth, hoping to allow a winder audience to have a better understanding of common Auth terms and processes.

It will then take a look at how NHS could utilise these best practises and follow set Enterprise Architect standards to achieve better outcomes for citizens and staff.

### Mind map thinking

```mermaid
mindmap
  root((Auth))
    Authentication
      Identity Providers
      Federation
      MFA
    Authorisation
      Policy Decision
      Policy Enforcement
      Attribs vs Roles
      Relationship Sources of Truth
        External Issuers
      
    Key Principles
      Separation of concerns
      Ease of expansion / integration
      Abstraction of IdP and Client App
    Tech
      OAuth
      OpenId
      STS 
      PDP
      PEP
      PKCE
      JWT
    Involvement
      Client Apps
      Data Providers
      Identity Providers
      Sources of Truth

```

## Introduction

People today have an identity crisis. They may not know it, but most will feel it in their every day interactions with technology. 



## Authentication

### What is an Identity Provider (IdP) should an Identity Provider be doing?


## Authorisation

#### Authorization Code Flow With Proof Key of Code Exchange (PKCE)
```mermaid
sequenceDiagram
autoNumber
participant User
participant Client application
participant Authorization server
participant Resource server (API)
activate User
User->>Client application: Access
activate Client application
Client application->>Client application: Generate code_verifier and transform it to code_challenge
activate Authorization server
Client application->>Authorization server: Authorization code request + code_challenge to /authorize
deactivate Client application
Authorization server->>User:Display consent
User->>Authorization server: Authenticate and give consent
deactivate User
Authorization server->>Client application: Issue authorization code
activate Client application
Client application->>Authorization server: Authorization code + code_verifier to /token
Authorization server->>Authorization server: Verify the authorization code, code_verifier, and code_challenge
Authorization server->>Client application: Return token
deactivate Authorization server
Client application->>Resource server (API): Call API with token
activate Resource server (API)
Resource server (API)->>Client application: Return data
deactivate Resource server (API)
deactivate Client application
```

Understanding PKCE flow [^auth-flow-pkce]

## How this could look

```mermaid
flowchart LR
NHSLogin((NHS Login))
CIS2((CIS 2))
OneGov((One Gov))
NHSMail((NHS Mail))
NHSAuth[NHS Auth]
NHSPDP[NHS PDP]
NHSApp[NHS App]
NHSPEP1[NHS PEP 1]
NHSAPI1[NHS API 1]
NHSPEP2[NHS PEP 2]
NHSAPI2[NHS API 2]
IdentityFederation[NHS Identity Federation]
NHSRelationshipService[Relationship Service]
NHSAccessService
NHSRolesAttributes[Roles and Attribs Service]
ExternalRelationship1[Ext Service 1]
ExternalRelationship2[Ext Service 2]
NHSAPIM[NHS APIM]

NHSLogin-->|Identity Provider|IdentityFederation
CIS2-->|Identity Provider|IdentityFederation
OneGov-->|Identity Provider|IdentityFederation
NHSMail-->|Identity Provider|IdentityFederation
IdentityFederation --> NHSAuth
NHSAuth-.->NHSPDP
NHSApp-.->NHSAuth

NHSPDP---NHSPEP1--->NHSPDP
NHSPEP1-.->NHSAPI1

NHSPDP---NHSPEP2--->NHSPDP
NHSPEP2-.->NHSAPI2

ExternalRelationship1-->NHSRelationshipService
ExternalRelationship2-->NHSRelationshipService
NHSRelationshipService<-->NHSAccessService
NHSAccessService-->NHSRolesAttributes
NHSPDP-->NHSRolesAttributes

NHSApp --> NHSAPIM
NHSAPIM -->NHSPEP1
NHSAPIM -->NHSPEP2

NHSAuth-.->IdentityFederation

linkStyle 7 stroke:none
linkStyle 10 stroke:none

```






## Key Technologies and Processes

### OAuth

### Tokens

#### Id Token

#### Access Token

#### Refresh Token

### OpenId

### Policy Decision and Enforcement

### Claims, Roles, Attributes

### Identity Federation





## How can this be implemented successfully?


### Testing a Sankey Diagram


```mermaid
sankey-beta

NHS Login,NHS App,6000000
NHS Login, NHS Web,100000
CIS2,System1,1000000
NHS App,PDS,2000000
System1,PDS,600000
```

## References

[^auth-flow-pkce]:Authorization Code Flow With Proof Key of Code Exchange (PKCE)

    - Reference: [Authorization Code Flow With Proof Key of Code Exchange (PKCE)][auth-flow-pkce-url]
    - Type: Website
    - Last Checked: 15/08/2023


[auth-flow-pkce-url]:https://cloudentity.com/developers/basics/oauth-grant-types/authorization-code-with-pkce/ "Authorization Code Flow With Proof Key of Code Exchange (PKCE)"