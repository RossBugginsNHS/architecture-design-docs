<style type="text/css" rel="stylesheet">
abc { color: pink; }
.slides .proxy-header { color: black; text-shadow: 0.01em 0.01em #ffffff;}
.slides .proxy-sub-header { color: black; text-shadow: 0.01em 0.01em white;}
.slides h3 { color: white; }
.slides .quote-text { color: pink; }
.reveal p, .reveal .slides section {line-height: 1.0;}
.myiframe {width:960px; height: 500px;}
</style>


<!-- .slide: data-background-image="https://images.pexels.com/photos/814544/pexels-photo-814544.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2" -->
## PROXY <!-- .element: class="proxy-header r-fit-text"  -->
Acting on behalf<!-- .element: class="proxy-sub-header"  -->

Note: 
A note here

---

## OED Definition

*"Authority given to a person  
to act for someone else"* <!-- .element: class="quote-text fragment r-fit-text" data-fragment-index="2"  -->
Note: 
A note there

---

## NHS Definition

*"Allow people to act on behalf of the  
patients and dependents they care for,  
and to allow users of NHS digital  
services to share their records with  
those they trust to act on their behalf"* <!-- .element: class="quote-text r-fit-text"  -->

---

## Tech Definition
*"An intermediary between a client  
requesting a resource and the server  
providing that resource,  
**potentially masking the true origin of the  
request to the resource server."***<!-- .element: class="quote-text r-fit-text"  -->

---

## What need answering?
- What can you do with proxy?
- Where does this happen?
- How do you get proxy?
- Who can give proxy?
- How is proxy recorded?
- Masking the true origin?
- How is it verified?
- How is it enforced?
- Hang on, what really is proxy for us?

---

## What can you (or should be able) do with "NHS Proxy"?
- Make decisions (non digital)
- View records (non digital)
- Schedule / book / accept appointments
- View digital records

*A mix of digital and non digital*

---

## Where does this already happen?
- GPs - no standard
- Secondary care?
- Paramedics? (Sec or Pri?)

---

## How do you get proxy? - The Act of "Giving Proxy"
The act of giving someone proof that they can act for you

---

## Burden of responsibility of verification
- Currently on medical professionals
- Paper checks
- **This Burden has to go**

---

## Burden of responsibility of enforcement
- Currently on medical professionals
- On the spot ID checks?
- Digtally - in GP systems - their own way..
- **This Burden has to go**

---

## Tearing down the word Proxy
- Giving Authority to act of behalf
- Recording it
- Proving it
- Acting upon it
  
---

## Splitting up the needs 
- Digitally - record roles and assignments
- Physically - need an easy way to proove
- Legally - Enforcing external "overrides" to set roles

---

## Can we do better, than Authority to "Act on behalf?"
- Parter has authority to act on my behalf to pick up prescriptions
- PA has authority to accept appointments

---

## And how about, what can do things, not just who
- Partner can see in an medical app prescription is ready
- But can't see what it is
- Shared family calendar app can have appointments
- partner can see full details
- children can just see time

---

Isn't this everywhere, without using the word proxy?
**YES**<!-- .element: class="r-fit-text"  -->

It's called roles.

---

And OAuth

---

And UMA

---

## Identity and its "Role"
NHS Login should just prove who someone is
NOT WHAT THEY CAN DO

---

## Ensuring Login is separate to EVERYTHING
Login should prove who someone is, and what their **CURENT** NHS number is
**It should not mean they have access do anything with  
the data associated with that NHS Number**  

---

## Bringing together "Account" & "Proxy"
- Account - this represents all data and demographics related to an NHS Number
- Has a "primary" NHS number (some people do end up with multiple - its FUBAR)
- Has a list of associated users (NHS LOGINS)
- list of Roles
- list of who is assigned to those roles
- list of "clients/apps"
- list of who is allowed to use those apps with this Account
- list of who is allowed to do what with each app

---

## Account Diag
<iframe data-src="/diagrams/proxy1.html" frameBorder="0" data-preload></iframe><!-- .element: class="myiframe"  -->

---

## All citizen interaction through account
- EVEN When acting as "proxy"

---

## So what is account?
Ideal: A citizen facing single API endpoint  
 with granular OAUTH controlled  scopes  
for each API.

Data Model: FHIR **is not** Citizen friendly..

---

## One you have this, everything is easy
- I can "give" access to anyone, myself
- My "proxy(s)" can interact with my account
- Legal "Proxy" enforcements (POA etc).... 
- The "Proxy Service" takes a "Proof"....
- And updates the roles.! 

---

## Reiterate
- NHS Login - who you are...
- **SHOULD NOT** imply **WHAT** you can do 

---

## A Citizen RBAC
... Gulp

---

## How VCs fit in to this - those we trust
- Used for proving to us someone has PoA, Parent Rights etc
- we update roles

---

## How VCs fit in to this - those who trust us
- We issue a "full medical access VC"
- We issue temporary NHS Account access VCs

---

## Ensuring I'm giving access to the correct person
- When adding a "User" to account
- Can require a VC presentation 
- Could include external VCs - ie NHS Number, DVLA number, etc etc

---
## Need a separate VC Session
- However, the takeaway is that VC's can support what we could do
- but there is a lot of underlying change needed first


---
## Scenario A: Partner to have "full" digital access

---

## Scenario B: Doctor wants consent for treatment on Alice
- Alice is incapacitated, Bob is claiming he has authority
- Doctor enters NHS Number on his device
- Scans QR code from Bobs phone (dig wallet, Biometrics)
- Doctors device confirms Bob has authority

---

## Scenario B: Where did that QR code come from?!
- Direct from MoJ - a "PoA" VC, Alice Subject, Bob the holder
- or...
- Issued by NHS - a "Full Access VC" that Alice chose to issue to Bob
- Maybe this has "Extra Constraints?"
- i.e. full decision making, but no authority on CPR? 

---

## Scenario B: Notes
- Doctor "could" look up the Alice to Bob roles in account?
- But needs the VC to prove that person is really Bob
- Takes the responsibilty of "Proof" away from the Doctor

---

## That's all well and good

But... The Data

What resources/data is going to use the RBAC?

---

<div class="mermaid r-stretch">
flowchart LR;
</div>

---

<div class="mermaid r-stretch">
flowchart LR;
SomeExternalProof-->ProxyService;
</div>

---

<div class="mermaid r-stretch">
flowchart LR;
SomeExternalProof-->ProxyService;
ProxyService-->RBAC;
</div>

---

### So why is proxy needed?

When if you have RBAC - someone can just give the roles

---

### Proxy Service

- Forces an update to those roles

---

# The "Proof" is the "easy" part

- Verifyable Creds
- Distributed Ids
- Fairly standard Tech now

---

# A Citizen RBAC on the other hand...

- Assume its been tried before?
- Can we look at Solid Pods to store the Role assignments?

---

# Data Data Data

- "The NHS" - biggest misconception that citizens have?
- Does this illusion lead to high expectations of citizen data access?

---
<!-- .slide: data-background-image="https://images.pexels.com/photos/814544/pexels-photo-814544.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2" -->
## Thank You <!-- .element: class="proxy-header r-fit-text"  -->
ross.buggins@england.nhs.uk<!-- .element: class="proxy-sub-header"  -->

github.com/RossBugginsNHS<!-- .element: class="proxy-sub-header"  -->