---
title: NHS Account
layout: page
has_children: true
nav_order: 2
---



![alt text](/nhs-account/nhs-account.svg)

- [NHS Account](#nhs-account)
- [Defining Problem Statement](#defining-problem-statement)
  - [What's the current state?](#whats-the-current-state)
  - [Whats the desired state?](#whats-the-desired-state)
    - [Problem Definition](#problem-definition)
      - [Key measurables](#key-measurables)
  - [What is envisioned for NHS Account?](#what-is-envisioned-for-nhs-account)
  - [User Controlled Consent - Empower the patient](#user-controlled-consent---empower-the-patient)
  - [How to best Model Access to Data](#how-to-best-model-access-to-data)
  - [API Design](#api-design)
  - [Back End Design](#back-end-design)
  - [Authentication](#authentication)
  - [Authorisation](#authorisation)
  - [Decentralised Identity / Verifiable Credentials](#decentralised-identity--verifiable-credentials)
  - [API Management](#api-management)
  - [API Resource Design](#api-resource-design)
  - [Resource User Access Control](#resource-user-access-control)
  - [Application Management](#application-management)
  - [Distributed Identity](#distributed-identity)
  - [Proxy Delegated Access](#proxy-delegated-access)
  - [Service Application Design](#service-application-design)
  - [Eventual Consistency](#eventual-consistency)
  - [Idempotency](#idempotency)
  - [Commands](#commands)
  - [Event Sourcing / CQRS](#event-sourcing--cqrs)
    - [Event Store](#event-store)
  - [Message Bus](#message-bus)
  - [Distributed Actor](#distributed-actor)
  - [Micro Services](#micro-services)
  - [Tartget API Times](#tartget-api-times)
    - [Support RESTful Endeavour (Promise) Pattern](#support-restful-endeavour-promise-pattern)
  - [Monitoring and Metrics](#monitoring-and-metrics)
    - [The Four Golden Signals](#the-four-golden-signals)
  - [Logging](#logging)

# NHS Account

Github

[What is account?][what-is-account]


Web

[What is account?]({% link nhs-account/what-is-account.md %})


# Defining Problem Statement

## What's the current state?

## Whats the desired state?

### Problem Definition

#### Key measurables

## What is envisioned for NHS Account?



## User Controlled Consent - Empower the patient

## How to best Model Access to Data 

## API Design

## Back End Design

## Authentication

## Authorisation

## Decentralised Identity / Verifiable Credentials

## API Management

## API Resource Design
- FHIR vs not FHIR

## Resource User Access Control

## Application Management

## Distributed Identity

## Proxy Delegated Access

## Service Application Design

## Eventual Consistency

## Idempotency

## Commands

## Event Sourcing / CQRS

### Event Store

## Message Bus

## Distributed Actor

## Micro Services

## Tartget API Times
- Max for any GET - xx ms
- Max for any POST - xx ms
- Max for and PUT - xx ms

### Support RESTful Endeavour (Promise) Pattern
- Utilise Event Sourcing and 202 / 201 HTTP Status codes instead of just a POST returning a 200.

## Monitoring and Metrics

### The Four Golden Signals
The four golden signals of monitoring are latency, traffic, errors, and saturation. If you can only measure four metrics of your user-facing system, focus on these four.

- Latency
- Traffic
- Errors
- Saturation

https://sre.google/sre-book/monitoring-distributed-systems/#xref_monitoring_golden-signals

## Logging 


[what-is-account]: what-is-account.md "What is account"