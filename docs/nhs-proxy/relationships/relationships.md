---
title: Relationships
layout: page
grand_parent: NHS Account Proxy
parent:  Verifiable Credentials
nav_order: 4.1
author: Ross Buggins
last_modified_date: Jul 21 2022 at 11:59 PM
---



<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
1. TOC
{:toc}
</details>

<hr/>

## Relationships for Medical Record Access

|                                                                         | From                                 | To    | Proof                                          | Issuer |
| ----------------------------------------------------------------------- | ------------------------------------ | ----- | ---------------------------------------------- | ------ |
| [🔗]({% link nhs-proxy/relationships/relationships-parent-child.md %}) | Biological Mother                    | Child | NHS "At Birth"                                 | NHS    |
|                                                                         | Parent                               | Child | Birth Certificate                              | GRO    |
|                                                                         | Parent                               | Child | Adoption Certificate                           | GRO    |
|                                                                         | Guardian                             | Child | Guardianship                                   | ???    |
|                                                                         | Married Partner of Biological Mother | Child | Marriage Certificate + (Mother to Child Proof) | GRO    |

# Diagrams of Parental Responsibility
**Key**
- Circle - Holder of rights
- Diamond - Subject of the rights
- Hexagon - Rights gained
- Parallelogram - Not a rights holder  
 ---

```mermaid
flowchart LR
Male[/Male/]
Female((Female))
Conceive
Birth
Child{Child}
MothersRight{{Mothers Parental Responsibility}}

Male-->Conceive
Female-->Conceive
Conceive-->Birth
Birth -.->MothersRight
MothersRight -.->Female
MothersRight -.->Child
```
*Figure [^motherFatherUnmarried].  Unmarried Mother and Father  (child born after 2003)* 


```mermaid
sequenceDiagram
    autonumber
    Father->Mother: Conception

    Mother->>Child: Gives Birth to
    Mother-->>Child: Gains Parental Responsibility of
    
    Midwife->>NHS Birth Service: Registers child to mother
    NHS Birth Service->>Child: Allocated NHS Number
    par Birth Registration
        Child->Registrar: Subject of Birth Certificate 
    and 
        Father->>Registrar: Father added to Birth Certificate
    and
        Mother->>Registrar: Mother added to Birth Certificate
    end  
    Father-->>Child: Gains Parental Responsibility of
```
*Figure [^motherFatherUnmarried].  Sequence Diagram for Unmarried Mother and Father  (child born after 2003)* 



---
```mermaid
flowchart LR
Male[/Male/]
Female[/Female/]
MarriedMale((Married Male))
MarriedFemale((Married Female))
Married
Conceive
Birth
Child{Child}
MothersRight{{Mothers Parental Responsibility}}
FathersRight{{Fathers Parental Responsibility}}

Birth -.->MothersRight
Birth -.->FathersRight
Male-->Married
Female-->Married
Married-->MarriedMale
Married-->MarriedFemale
MarriedMale-->Conceive
MarriedFemale-->Conceive
Conceive-->Birth
MothersRight -.->Child
FathersRight -.->Child

FathersRight -.->MarriedMale
MothersRight -.->MarriedFemale
```
*Figure [^marriedmotherandfather].  Married Mother and married biological Father* 

---
```mermaid
flowchart LR
Male
Female
MarriedMale((Married Male))
MarriedFemale((Married Female))
OtherMale[/Other Male/]
Married
Conceive
Birth
Child{Child}
MothersRight{{Mothers Parental Responsibility}}
FathersRight{{Fathers Parental Responsibility}}

Male-->Married
Female-->Married
Married-->MarriedMale
Married-->MarriedFemale
OtherMale-->Conceive
MarriedFemale-->Conceive
Conceive-->Birth
Birth -.->MothersRight
Birth -.->FathersRight
MothersRight -.->Child
FathersRight -.->Child
FathersRight -.->MarriedMale
MothersRight -.->MarriedFemale
```
*Figure [^marriedmotherandotherfather].  Married Mother and other biological Father* 

---

```mermaid
flowchart LR
Male((Male))
Female((Female))
Conceive
Birth
Child{Child}
MothersRight{{Mothers Parental Responsibility}}
BirthCertificate
FathersRights{{Fathers Parental Responsibility}}

Birth -.->MothersRight
Birth --> BirthCertificate
Male-->Conceive
Female-->Conceive
Conceive-->Birth
MothersRight -.->Female
MothersRight -.->Child
Male-->BirthCertificate
Female-->BirthCertificate
Child-->BirthCertificate
BirthCertificate-.->FathersRights

FathersRights -.->Male
FathersRights -.->Child
```
*Figure [^motherFatherUnmarried-with-birth-cert].  Unmarried Mother and Father with birth certificate* 

---
```mermaid
flowchart LR

Male
Female
AdoptedParent1((AdoptedParent1))
AdoptedParent2((AdoptedParent2))
Adoption
Child{Child}
MothersRight{{Mothers Parental Responsibility}}
FathersRight{{Fathers Parental Responsibility}}

Male -->Child
Female -->Child
MothersRight -.->Child
FathersRight -.->Child
Child -->Adoption
Male -->Adoption
Female -->Adoption

FathersRight -.->AdoptedParent1
MothersRight -.->AdoptedParent2
Adoption-.->MothersRight
Adoption-.->FathersRight
```
*Figure [^adopted].  Child Adopted after birth* 






[^motherFatherUnmarried]: Unmarried Mother and Father (child born after 2003)
[^marriedmotherandfather]: Married Mother and biological Father
[^marriedmotherandotherfather]: Married Mother and other biological Father
[^motherFatherUnmarried-with-birth-cert]:  Unmarried Mother and Father with birth certificate
[^adopted]:  Child adopted after birth, adopted parents have PR