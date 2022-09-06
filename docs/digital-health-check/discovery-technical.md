---
title: DHC Discovery - Technical
layout: page
parent: NHS Digital Health Check
nav_order: 2
has_children: true

---

> ⚠️ **Warning**
>  
> **Draft Documents**: May not represent real world scenarios, may not be fully accurate or complete.
>
> Please contact the author for more information.

# Digital Health Check - Discovery - Technical


## Key Items to address
- Identify possible technical hurdles
- How to ensure DHC discovery/future alpha/beta work is reusable easily
- Any key patterns & practices to follow
- Identify any components of existing system that could be utilised / reused in alpha
- Overview of data model
- GDPR / Data - NHS-E as data controller for PII???? (not technical?)

## Status

| Todo                                                                | Status        |
| ------------------------------------------------------------------- | ------------- |
| Previous System's Architecture Review                               | ⚠️ in progress |
| Scope Required Data Model                                           | ⚠️ in progress |
| Identity / Authorisation Options                                    | ⚠️ in progress |
| Any Future Questions for User Research                              | 🕐 Pending     |
| Identify any technical hurdles                                      | 🕐 Pending     |
| Propose any technical patterns that could /should be followed       | 🕐 Pending     |
| Identify how DHC could be broken down into separate systems/modules | 🕐 Pending     |

### Technical Hurdles

| Todo                          | Status        |
| ----------------------------- | ------------- |
| GP Integration Options        | ⚠️ in progress |
| Cohort & Invite Management    | 🕐 Pending     |
| Pre load from GP record       | 🕐 Pending     |
| Self load data from wearables | 🕐 Pending     |


# Discovery Outcomes Summary

- Component Structure for Alpha - National scalable concerns
- Good UI from previous work
- GP Integration = hard
- Question still to be answered - how can digital improve upon and give more benefit that manual health check - instead of just trying to make a carbon copy of existing process
- Question still to be answered - preloading of data
- Question still to be answered - Data in GP record vs data available to GP - what would GP's want / expect
- Question still to be answered - Invite and cohort management

# DHC Technical Discovery Review

**TODO - Write a summary encompassing all work that has been completed as part of the Technical Discovery**

## Summary

The technical requirements for a Digital Health Check are not novel, or overly complex. A well architected technological implementation should not be a blocker for the progression of this programme.

There are a number of clearly defined boundaries within the overall scope of the programme, that will allow for Agile delivery through incremental and iterative feature development.

#### Components Identified
- Digital Health Check Library Code
- API for DHC tool
- API for DHC state management
- API for invite management
- Authorisation
- End user UI
- Health Care Professional UI
- Pre load service
- Export to GP Service
- Cohorting / Invite Service
- Blood Test Labs / appointment booking integration

Some of these components will be OHID specific, others could be available for other providers who wish to use the same Digital Health Check "Engine".

#### Other considerations for beta
- Storage Platform
- Inter service communication / event bus / command handlers etc

Many NHS digital programme requirements start with statements such as "must integrate with NHS App", or " must integrate with NHS.uk". Naive requirements such of this can be appreciated from non technical authors, however architectural and development work should take this and expand to really see the requirement for what it is. Focus on digital solutions should not just be on where they envisaged to be used right now. The NHS App in it's current form will not be around for ever, neither will the nhs web site. Digital solutions MUST be developed with a clear API first focus, that can then be integrated with the NHS app, or any other app.

### Component Interaction Considerations for Alpha

```mermaid
C4Context
  title NHS Digital Health Check Alpha Components

    Boundary(ohidboundary, "OHID DHC", "System")
    {
        Boundary(ohiduiboundary, "OHID DHC User Interface", "System") {     
        System(ohidui, "OHID DHC User Interface")       
        }      

        Boundary(ohidprocess, "OHID DHC State Management", "System") {     
        System(ohiddhc, "OHID DHC")       
        }   

        Boundary(hpinterfaceboundary, "Health Professional Interface", "System") {    
            System(hpinterface, "HP System")      
            System(bimisystem, "BI / MI System")              
        }
    } 

    Boundary(ohidintegratboundary, "OHID Integrations", "System")
    {
        Boundary(dhcpreload, "DHC Preload", "System") {  
            System(dhcpreload, "Read data from GP Record")       
        }   

        Boundary(gpintegration, "GP Integration", "System") {  
            System(gpint, "GP access to data")       
        } 

        Boundary(labsandappointments, "Labs and Appointments", "System") {          
            System(labsapi, "Labs API integration")
            System(appointmentapi, "Appointment booking integration ")
        }            

        Boundary(inviteandcohort, "Inivite and Elegability", "System")
        {
            Boundary(b0, "Invitation", "System") {
            System(invitemanagement, "Invite Management")
            }

            Boundary(cohorting, "Cohorting", "System") {  
            System(identifycitzens, "Identify Citizens")  
            }
        }
    }                                       
     
    Boundary(dhc, "Digital Health Check", "System") { 

        Boundary(dhcstatemanagement, "DHC Process and State Management", "System") {   
          System(dhcapi, "DHC API Process")
        } 

        Boundary(dhctools, "DHC Tools", "System") {          
        System(dhcapitool, "DHC API Tool")
        System(dhctool, "DHC Alogorithm/Logic library")
        Rel(dhcapitool, dhctool, "Uses")
        }     

        Boundary(generaltools, "Other Tools API", "System") {          
        System(bpapi, "BP API Tool")
        System(bmiapi,  "BMI API Tool")
        System(qriskapi, "Qrisk API Tool")
        }         
    }
   
    Boundary(auth, "Authentication", "System") {          
      System(nhsl, "NHSlogin")
      System(cis2, "CIS2")
      System(auth, "Authentication")
    }  

    Rel(invitemanagement, identifycitzens, "Gets data from")
    Rel(dhcapi, dhcpreload, "Gets data from")
    Rel(ohiddhc, gpint, "Sends data to")
    Rel(dhcapi, dhcapitool, "Uses")
    Rel(dhcapitool, bpapi, "Uses")
    Rel(dhcapitool, bmiapi, "Uses")
    Rel(dhcapitool, qriskapi, "Uses")

    Rel(ohidui, ohiddhc, "")
    Rel(ohiddhc, dhcapi, "")
    Rel(ohiddhc, invitemanagement, "")    
    Rel(ohiddhc, labsapi, "")        
    Rel(ohiddhc, appointmentapi,"")     
    Rel(ohiddhc, hpinterface,"")     
           
```


## Digital Health Check Library Code
At its core, a Digital Health Check is nothing more than:

```mermaid
flowchart LR;
    In[Data In]
    Process{Process Data}
    Out[Data Out]
    In -->Process
    Process -->Out
```

Can be broken down into:

```mermaid
flowchart LR;
    ObsDataIn[Demographics & Observation Data In]
    RawProcessData{Prepare Data}
    Data[HC Data]
    Calculate{Calculate Result}
    Out[Result Out]
    ObsDataIn -->RawProcessData
    RawProcessData -->Data
    Data -->Calculate
    Calculate -->Out
```

Event if an alpha achieves nothing more than this process being made available in a open source digital form (whether an API, or  just a code library that can be used in a CLI) the something positive has been achieved that future work can cleanly and easily build upon.

The perceived complexities for a Digital health Check come from where the "Data In" will come from, and where the "Data Out" will go.

At its most basic, it should be a simple idempotent library. 
Note for idempotent methods, thought should be given to not using variable types such as Dates, instead age in days should be used - ensuring that the same data payload always returns the same result, no matter what the date is today.

```csharp
public static HealthCheckResult CalclateHealthCheck(HealthCheckData value)
{
    //Calculate health check and return result.
    throw new NotImplementedException();
}
```

> **Step 1** 
> 
> Idempotent Library for Calculating DHC results from provided prepared data
>
>
> 
> .

### Incremental Health Check Library Development, Possible Routes
- Start with just basic checks - add more data over time?
- Require all data for a result vs Can give result with partial data?
- Should some be separate libraries / APIs? BMI for example? Blood pressure result?
- 

## Digital Health Check Tool API 
The development of an API first designed system that is just a simple Tool for returning the results of a full set of Health Check data. This will be stateless and can be used by the Digital Health Check Service, both internally and externally. This is a key concept to allowing the DHC work to be reused, and re worked, in the future.

```mermaid
flowchart LR;
    In(Data In & Out)
    API(API)
    Process{Process Data}
    In <----> API
    API --> Process
```

## Key considerations for all APIs
- Easy data model, including for those who are not Health IT Professionals 
- RESTful
- Asynchronous, use of Location Header
- Clear metric and performance requirements for all API calls
- OAuth claims scoping
- Versioning
- End of Life

## Data Modelling
The tooling APIs should accessible and be intuitive for all to use. I, as a citizen with some technical understanding, should be able to call the Digital Health Check tool API and receive a result.

As such, there must be though given to if a FHIR data model is the best way to go forward for public APIs that are focussed on the Citizen consuming them.

FHIR has a key place in inter health system communication, but it is likely to be perceived as bloated and overly complex for exposing APIs to Citizens.

The DHC API will require .....

> **Step 2** 
> 
> API for consuming observation and demographic data, preparing data, and calculating DHC result 
>
> Including separate APIs for any preparation - ie BMI calculation and QRisk required data conversions
> 
> .





## Example Microservice Design for DHC 

```mermaid
C4Context
  title NHS Digital Health Check Alpha Components

    System_Ext(SomeSystem, "Example System", "A system consuming DHC API.") 

    Boundary(dhcmicroservice, "Digtial Health Check", "Microservice Namespace")
    {
        Boundary(dhcapi, "DHC API")
        {
            System(dhcapicmds, "DHC Cmd API", "RESTful API")   
            System(dhcapiqueries, "DHC Query API", "RESTful API")   
        }
        Boundary(cmdsubsys, "Commands Sub System")
        {
            SystemQueue(cmdbus, "Commands")     
            System(cmdhandlers, "Command Handlers")     
        }

        Boundary(statesubsys, "Domain and State Sub System")
        {        
            System(domainandstate, "Domain & State") 
            SystemQueue(eventsourcing, "Event Sourcing State")
            System(internaleventsourcinghandling, "Event Sourcing Hanlder")    
        }
        
        Boundary(eventsubsys, "Event Sub System")
        {                
            SystemQueue(internalevents, "Events")        
            System(internaleventhandling, "Event Handlers ")        
            System(extEventhandler, "Event Handler forwarding")          
        }

        Boundary(statesubsysd, "Caching Sub System")
        {
            SystemDb(statecache, "State Cache Handing")                
        }

        Rel(dhcapicmds, cmdbus, "Sends Commands")
        Rel(cmdbus, cmdhandlers, "Handles Commands")
        Rel(cmdhandlers, domainandstate, "Executes State")
        Rel(domainandstate, eventsourcing, "Raised Event")
        Rel(eventsourcing, internaleventsourcinghandling, "Handles")
        Rel(internaleventsourcinghandling, internalevents, "Raised Event")       
        Rel(internalevents, internaleventhandling, "Handles")         
        Rel(internaleventhandling, statecache, "Stores State")
        Rel(domainandstate, statecache, "Gets State")
        Rel(dhcapiqueries, domainandstate, "Gets State")
        Rel(internalevents, extEventhandler, "Handles")
        

        UpdateRelStyle(dhcapi, cmdbus, $textColor="black", $lineColor="black", $offsetY="10", $offsetX="-50")
        UpdateRelStyle(domainandstate, statecache, $textColor="black", $lineColor="black", $offsetY="-100", $offsetX="230")

    }

    Boundary(sharedbus, "Inter Service Event Bus", "Microservice Namespace")
    {
        SystemQueue(interserviceeventbus, "Inter Service Events")
    }

    System_Ext(SomeConsuming, "Example External Integration System", "A system consuming events, eg GP integration.") 

    Rel(extEventhandler, interserviceeventbus, "Raises Event")
    Rel(SomeSystem, dhcapicmds, "Calls API")
    Rel(SomeSystem, dhcapiqueries, "Calls API")
    Rel(interserviceeventbus, SomeConsuming, "Handles Event")
```


## Capacity and Scale Discovery
NEED TO :
- Find out how many health checks per day
- How many NHS Logins
- How much use if only invite vs accessible to all

## Open Source
All development work should be open source from the very start of any beta stage.

## Development Recommendations
- VSCode remote development containers for each project
- Docker build and deploy files
- API first, use swagggerhub for api design and bolilerplate 