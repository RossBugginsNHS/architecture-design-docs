---
title: Azure B2C Verifiable Credentials
layout: page
parent: NHS Account Proxy
nav_order: 1
author: Ross Buggins
last_modified_date: Jul 26 2022 at 10:06 AM
tags: 
    - research
    - proxy
    - azure
    - verifiable credentials
description: A discovery into the possibilities for NHS Account Proxy to use Azure AD B2C Verifiable Credentials
todo:
    - complete research
---




<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
1. TOC
{:toc}
</details>

# Abstract

# Summary

## Aim of this research

## Why is needed?

## Other possible outcomes / benefits 

## Future Work

# Hypothesis

# Methodology

**2022-08-08**

MS Confirmed that Azure AD B2C cannot use VC directly, as it can't access Key Vault in "parent" tenant and can not have a key vault directly in the B2C tenant.

It *should be* possible via using Azure Lighthouse configuration to allow the B2C to have access to a subscription in the parent.

## Azure Access

## Creation of Azure AD B2C

## Key Vault

## Azure AD B2C Verified Credentials Initial Setup

## External Identity Providers

## Configuring Client Application



# Results

# Conclusion

# Evaluation

# References

[^example]: Example.com

    - Reference: [http://www.example.com](http://www.example.com)
    - Type: Website
    - Last Checked: 15/07/2022
