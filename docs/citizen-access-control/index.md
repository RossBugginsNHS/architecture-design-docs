---
title: Citizen Data Context and Consent (CDC<sup>2</sup>)
sub_title: Making Data Accessible with Common Context and Consent
layout: page
nav_order: 2.001
has_children: true
---

{% include githubrawurl.html %}


```mermaid
C4Context
title System Context diagram for Citizen Data Context and Consent (CDCC)

Person(citizenContext, "Citizen", "The Citizen / Customer / Patient.")
Person(customerB, "Banking Customer B")
Person_Ext(customerC, "Banking Customer C")
System(SystemAA, "NHS App", "Allows customers to view information about their bank accounts, and make payments.")

Person(customerD, "Banking Customer D", "A customer of the bank, <br/> with personal bank accounts.")

Enterprise_Boundary(b1, "NHS Services") {

  SystemDb_Ext(SystemE, "Mainframe Banking System", "Stores all of the core banking information about customers, accounts, transactions, etc.")

  System_Boundary(b2, "NHS Login") {
    System(SystemA, "Identity Service")
    System(SystemB, "Banking System B", "A system of the bank, with personal bank accounts.")
  }

  System_Ext(SystemC, "E-mail system", "The internal Microsoft Exchange e-mail system.")
  SystemDb(SystemD, "Banking System D Database", "A system of the bank, with personal bank accounts.")

  Boundary(b3, "BankBoundary3", "boundary") {
    SystemQueue(SystemF, "Banking System F Queue", "A system of the bank, with personal bank accounts.")
    SystemQueue_Ext(SystemG, "Banking System G Queue", "A system of the bank, with personal bank accounts.")
  }
}

BiRel(citizenContext, SystemAA, "Uses")
BiRel(SystemAA, SystemE, "Uses")
Rel(SystemAA, SystemC, "Sends e-mails", "SMTP")
Rel(SystemC, citizenContext, "Sends e-mails to")           
```

# Preface

- Identity
- Access Control
- Claims based Access Control
- Relationships
- "Proxy"
- PODs / Data Store
- Digital Verifiable Credentials
- API / Scopes 
- OAuth 2