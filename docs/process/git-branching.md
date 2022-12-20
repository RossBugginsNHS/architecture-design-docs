---
title: Git Branching Strategy
layout: home
nav_order: 100
parent: Contributing

---



Main Repo
https://github.com/nhsexperience/nhs-experience

## Suggested use of fork branches

<div class="mermaid">
flowchart LR;
    nhsexperience/nhs-experience:main-->|Fork|myuser/nhs-experience:main;
    myuser/nhs-experience:main-->|upstream fetch, upstream pull|myuser/nhs-experience:main;
    myuser/nhs-experience:main-->|Branch|myuser/nhs-experience:my-edit-branch;
    myuser/nhs-experience:my-edit-branch-->|Pull Request|nhsexperience/nhs-experience:main;
</div>

# Main Branch
- Follow CI/CD - all Pull requests are made to Main, this is constantly evolving.
- 
# Releases
- Will be tagged with v[M].[m].[r]-[prerelease] in accordance with semantic versioning

# Feature branches
?? If join collaboration on a specific area is in progress, please request a branch to be created in the upstream repo for this development work.??
