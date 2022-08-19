---
title: API Design - BP
layout: page
grand_parent: NHS Digital Health Check
parent: Architecture
nav_order: 10.1
---

> ⚠️ **Warning**
>  
> **Draft Documents**: May not represent real world scenarios, may not be fully accurate or complete.
>
> Please contact the author for more information.

<div id="swagger-ui"></div>
<script src="/swagger-ui/swagger-ui-bundle.js" charset="UTF-8"> </script>
<script src="/swagger-ui/swagger-ui-standalone-preset.js" charset="UTF-8"> </script>

<script>
    window.onload = function() {
        window.ui = SwaggerUIBundle({
        url: "https://nhs-bp.azurewebsites.net/swagger/v0.2/swagger.json",
        dom_id: '#swagger-ui',
        deepLinking: true,
        presets: [
            SwaggerUIBundle.presets.apis,
            SwaggerUIStandalonePreset
        ],
        plugins: [
            SwaggerUIBundle.plugins.DownloadUrl
        ],
        layout: "BaseLayout"
        });
    };
</script>