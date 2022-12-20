---
title: Architecture
layout: page
parent: NHS Digital Health Check
nav_order: 7
has_children: true
todo:
  - interaction flow diagram (user proccess)
  - high level component view diagram
---





# Architecture for Citizen Experience
- Ensure all exposed endpoint are easy for Citizen consumption
  - i.e. APIs that are simple and straight forward - i.e. don't assume data standards such as FHIR are useful for Citizens to use
  - OAuth / API management
  - User Consent for Data Use

# Digital Health Check Architecture - Alpha Targets

- Focus on Data and Process 
- Alpha should Focus on clear RESTful API design
- OpenAPI documented API
- API to be 
- API performance parameters to be defined
- User interface "secondary"
- Make data captured available in a User consent Driven way
- Utilizing existing data where possible? *What does use research say on this?*
- CIS2 and NHS L as Identity Providers with standardized authorisation
- API required due to hard GP integration - allow data out via CIS2 oauth.
- Event Driven backend

## Unknown
- Data preloading
- Central Cohort / Invite management

# Whats still needed from Discovery for this?
- Getting data to GP - needs to not be complex - so API driven is way forward
- Make sure Architectural goals align with outcomes of discovery user research

# Teams / squds required
- API, Authorisation & Data Design
- Backend service / event infra 
- APP/UI integration
- GP Integration

## System Context diagram

```mermaid
C4Context
  title NHS Digital Health Check
    Boundary(b0, "Invitation", "System") {
      System(CitizenA, "Invite Management", "NHS Login User.")
    }
    Boundary(sb2, "Cohorting", "System") {  
      System(SystemA, "Identify Citizens")  
    }
    Boundary(dhcpreloadboundary, "DHC Preload", "System") {  
      System(dhcpreload, "Read data from GP Record")       
    }        
    Boundary(b3, "GP Integration", "System") {  
      System(S3ystemA, "GP access to data")       
    }    
    Boundary(b23, "DHC Process", "System") {   
      System(dhcapi, "DHC API Process")       
      System(DHCProcess, "Complete a DHC")
    }   
    Boundary(dhctoolsb, "DHC Tools", "System") {          
      System(dhcapitool, "DHC API Tool")
      System(dhctool, "DHC Alogorithm/Logic ")
    }     

    Boundary(apigwb, "API Gateway", "System") {          
      System(apigw, "API Gateway")
    }       
    Boundary(authBoundary, "Authentication", "System") {          
      System(nhsl, "NHSlogin")
      System(cis2, "CIS2")
      System(auth, "Authentication")
    }                         
```

## System User Interaction
```mermaid
C4Context
title NHS Digital Health Check
Person(CitizenA, "Citizen", "NHS Login User.")
Container_Ext(NHSlogin, "NHS Login", "")
Container(NHSDHC, "NHS Digital Health Check", "Health Check App")
Rel(CitizenA, NHSlogin, "Uses", "OpenId")
Rel(CitizenA, NHSDHC, "Uses", "HTTPS")
UpdateRelStyle(CitizenA, NHSlogin,  $offsetY="-40", $offsetX="-20")    
UpdateRelStyle(CitizenA, NHSDHC, $offsetY="20")
```
