---
title: Southwark
layout: page
parent: DHC Discovery - Technical
grand_parent: NHS Digital Health Check
nav_order: 20
---



## Pilot Review
[Southwark Staging Area DHC UI](https://stagingsouthwarkdhctest.qxlva.io) can be accessed publicly for UI review.

### Summary of Code Review
- Razor UI Components for each UI stage
- Core calculation code base provides an indication of what is required to calculate a Health Check result
- Core code is not easily extensible to changes or additions in the process
- UI and process are tightly coupled - should be separated 
- Areas of code use non standard dotnet patterns, should be corrected
- State and processing should be separated out from the Web App process, should utilise command bus and eventing
- After separation, deployment should aided with docker & docker compose or kubernetes files 
- Requires version upgrades to frameworks and dependencies
- Invite management is manual process 
- No use of NHS Login for proof of identity, custom urls used instead
- No direct integration to GPs
- No integration with Labs or Appointment management
- No CMS for UI customisation, all hard coded values
- Current version is likely suitable for a local pilot, not for a national production service
- Discussions in place over future open sourcing of the Southwark code base, however, before this a code review should be completed to ensure code and secrets management are suitable.  

## Background 

Sourced from "220405 Technical Specification DHC Tool v1.docx"

NHS Health Checks are currently performed in GP surgeries only; however, a review of the service has demonstrated that this service model limits access. In Southwark, the uptake of NHS Health Checks is around 50%. Consequently, Southwark Public Health division is now developing a Digital NHS Health Check (DHC), offering a fully remote service alongside face to face appointments, to enable residents to have their risk of CVD and Type 2 Diabetes assessed without having to visit a GP surgery. The DHC will be an important part in tackling local inequalities by improving access to residents who may not traditionally visit their GP. Further, with an ambition of designing out inequity of outcome, the design of this service has been focused on co-creation with residents from Black, Asian and Minority Ethnic backgrounds, as well as those from the most disadvantaged communities.

A key component of the DHC is the digital tool. This has already been developed by Quality Medical Solutions Ltd (QMS); however, Southwark Council now require a new provider to take over the hosting of the application, as well as the ongoing maintenance and enhancement of it. This specification describes the technical requirements for ongoing support of the DHC tool.


In 2018, Southwark commissioned QMS, a digital company, to develop a digital tool that aimed to increase uptake of the NHS Health Check face-to-face service by non-responders. Between July 2019 and March 2020, Southwark residents were signposted to the digital tool if they had not responded to the offer of a face-to-face NHS Health Check. The use of the tool was paused alongside the pause of the NHS Health Check programme in March 2020. 

Whilst the tool was being developed in 2019, concurrently, Public Health England (PHE) conducted extensive research to understand the challenges faced in the delivery of the entire NHS Health Check customer journey and explore how digital could improve delivery. In 2019/2020 Greater Manchester then built on PHE’s work, working with clinicians and residents to design a digital NHS Health Check prototype. 

More recently, Southwark Public Health division, working with QMS, has built on the findings from PHE and Greater Manchester, along with local research with residents to improve the DHC tool in Southwark. This tool has been developed in partnership with local partners, clinicians and residents, and is currently being tested in a pilot to determine the feasibility and acceptability of the DHC as an additional option alongside the face-to-face service. 

In early 2022, QMS announced the closure of their organisation, effective from June 2022. Consequently, hosting and management of the DHC tool will cease in April 2022, hence there is an urgent need for a new provider to take over the management of the tool following completion of the pilot. 

### Current Use Process

1.	GP selects a group of patients eligible for the service
2.	GP (or their representative) uses the DHC Link Generator to generate a unique link per patient
3.	An SMS is sent to each patient with their personal link
4.	The patient uses the link to visit the DHC website
5.	The DHC asks a series of questions to determine:
    1.	Patients Health Priorities
    2.	Patient demographic details
    3.	Health measurements (height, weight, blood pressure, cholesterol, alcohol and exercise profile, mental health screening questions)
    4.	The patient is offered personalised recommendations, considering their health priorities and potential obstacles
    5.	The patient can confirm their identity and contact details to facilitate data transfer to other parties, as follows
6.	The details of the assessment are retained in a central database, and the patient can return later to complete an incomplete check.
7.	The patient’s results (if patient authenticates) are returned to their GP for inclusion in the GP clinical record
8.	The patient can optionally accept a referral to a Behaviour Change Provider, which is sent to the provider by email.  
9.	The Behaviour Change Provider contacts the patient to arrange follow-up
10.	The patient (optionally) receives a copy of their results via email
11.	The patent email may reinforce the need for them to contact their GP for a face-to-face appointment if they seem to be at high risk
12.	The central database provides a number of reports to allow monitoring of the DHC website usage


## Technical Requirements

The application has to date been hosted in a Microsoft Azure environment using IIS .NET Core 3.0 and SQL Server on 2 virtual Windows servers.  It is likely that the .NET Core application could be hosted in other environments, but this has not been tested. 

The source code for the system is held in a Git repository and allows developers to build a skeleton copy of the Digital Health Check Application. Customisation and configuration are required for each new client installation, and documentation on the configurable options is provided in the repository.

The DHC application consists of a website with database backend. The database uses Microsoft SQL Server, and the website/application is hosted in Microsoft IIS using .NET Core.  These are hosted in separate servers in the Microsoft Azure environment.  The webserver is internet-accessible and connected via a private network to the database back-end, each with appropriate firewall rules.

### web server
- Windows 10+ / Windows Server 2018+
- .NET Framework 5+
- IIS 10 with .NET Core Hosting Bundle installed
- IIS 6 for SMTP relay (if needed)
- Windows Service for background scheduled tasks
- 
### database server
- One SQL server 2018+ database server containing two databases:
- The DHC database, which can be generated from scratch using Entity Framework Migration scripts.
- The Townsend deprivation index database, which is static and can be generated from public data

### Technologies

- C# .NET 5+
- ASP.NET Core 5.0
- Blazor/Razor pages
- SQL Server / T-SQL
- Entity Framework Core


