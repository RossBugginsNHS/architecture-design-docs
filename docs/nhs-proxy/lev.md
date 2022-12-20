---
title: Life Event Verification (LEV)
layout: page
parent: NHS Account Proxy
nav_order: 4
---




A ReSTful API for verifying 'life events' such as births, deaths and marriages.

https://docs.api.lev.homeoffice.gov.uk/life-event-verification-lev-api/reference

# Features

- easy restful API for returning certificates.
  
# Missing features

- No unique id link between birth records and the citizens referenced. I.E. from a child's birth certificate, you cannot easily return the birth certificate of either parents. Or, from a marriage certificate you cannot easily get the birth certificate of the partners.

# Possible next steps

- Discuss with LEV team how digital ID's / verifiable credentials could be utilised for HMPO / GRO data

# API Specification

<div id="swagger-ui"></div>
<script src="/swagger-ui/swagger-ui-bundle.js" charset="UTF-8"> </script>
<script src="/swagger-ui/swagger-ui-standalone-preset.js" charset="UTF-8"> </script>
<script src="api.js" charset="UTF-8"> </script>
