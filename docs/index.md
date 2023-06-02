---
title: Home
sub_title: Citizen Experience Architecture
layout: home
nav_order: 1
last_modified_date: Jul 15 2022 at 03:39 PM
---


{% capture pgs %}
  {% for pg in site.pages %}
    {% assign ext = pg.url | split: "." | last %}
    {% if ext == "html" %}
      |{{ pg.content | number_of_words  | prepend: '00000000' | slice: -8, 8 }}#{{ pg.title }}#{{ pg.content | number_of_words }}#{{pg.url}}#{{ext}}
    {% endif %}
  {% endfor %}
{% endcapture %}

{% assign sortedpgs = pgs | split: '|' | sort %}

# NHS Citizen Experience Architecture

{% if
    site.gh_edit_link and
    site.gh_edit_link_text and
    site.gh_edit_repository and
    site.gh_edit_branch and
    site.gh_edit_view_mode
%}
[Get started now](/contributing.html){: .btn .btn-primary .fs-5 .mb-4 .mb-md-0 .mr-2 } [View it on GitHub]({{ site.gh_edit_repository }}/{{ site.gh_edit_view_mode }}/{{ site.gh_edit_branch }}{% if site.gh_edit_source %}/{{ site.gh_edit_source }}{% endif %}/{{ page.path }}){: .btn .fs-5 .mb-4 .mb-md-0 }
{% endif %}


## What is Citizen Experience?

## Projects

<table>
<tr><th>Name</th><th>Status</th><th>Start Date</th><th>End Date</th></tr>

{% for project in site.projects %}
<tr>
<td>
    <a href="{% link {{project.link}} %}">
      {{ project.name }}
    </a>
</td>
<td>
      {{ project.status }}
</td>
<td>
      {{ project.start-date }}
</td>
<td>
      {{ project.end-date }}
</td>
</tr>
{% endfor %} 
</table>

## Project Source

<table>
<tr><th>Name</th><th>Try It</th><th>DockerHub</th><th>Docker Build</th></tr>

{% for project in site.projectlibs %}
<tr>
<td>
    <a href="{{ site.gh_edit_repository }}/tree/{{ site.gh_edit_branch }}/{{project.link}}">
      {{ project.name }}
    </a>
</td>
<td>
    <a href="{{ project.deployedlink }}">
      {{ project.deployedlink }}
    </a>
</td>
<td>
    <a href="https://hub.docker.com/r/{{ project.docker }}">
      {{ project.docker }}
    </a>

</td>
<td>
  ![Azure CI/CD]({{ site.gh_edit_repository }}/actions/workflows/dhc-aplha-dhcapi-docker.yml/badge.svg?branch={{ site.gh_edit_branch }})
</td>
</tr>
{% endfor %} 
</table>



  
---
# Posts
<ul>
    {% for post in site.posts %}
      <li>
        <a href="{{ post.url }}">{{post.date| date: "%-d %B %Y" }}: {{post.author }} - {{ post.title }}</a>
      </li>
    {% endfor %}
</ul>

# Page List
<div>
<ul>
{% for pg in sortedpgs reversed %}
    {% assign pgitems = pg | split: '#' %}
    <li><a href="{{pgitems[3]}}">{{ pgitems[2] }} words : {{ pgitems[1] }}</a></li>
{% endfor %}
</ul>
</div>

# Source and build details
  
  
[View it on GitHub]({{ site.gh_edit_repository }}/{{ site.gh_edit_view_mode }}/{{ site.gh_edit_branch }}/samples/nhs-login-client/Client){: .btn .fs-5 .mb-4 .mb-md-0 }


## Open Source Location


|                           | Source and build details                                                             |
| :------------------------ | :----------------------------------------------------------------------------------- |
| Repo                      | {{ site.gh_edit_repository }}                                                        |
| Branch                    | {{ site.gh_edit_branch }}                                                            |
| Link                      | {{ site.gh_edit_repository }}/{{ site.gh_edit_view_mode }}/{{ site.gh_edit_branch }} |
| Site                      | {{ site.published_url }}                                                             |
| Latest Release            | Test release v0.1.0-alpha                                                            |
| Latest Release Tag        | v0.1.0-alpha                                                                         |
| Latest Stable Release     | n/a                                                                                  |
| Latest Stable Release Tag | n/a                                                                                  |

- [Latest Release]({{ site.gh_edit_repository }}/releases/latest)
- ![Azure CI/CD]({{ site.gh_edit_repository }}/actions/workflows/{{ site.gh_action_build_file_name }}/badge.svg?branch={{ site.gh_edit_branch }})

## Upstream Open Source Location

|                           | Source and build details                              |
| :------------------------ | :---------------------------------------------------- |
| Owner                     | nhsexperience                                         |
| Repo                      | nhs-experience                                        |
| Branch                    | main                                                  |
| Link                      | https://github.com/nhsexperience/nhs-experience       |
| Site                      | https://zealous-beach-05e189903.1.azurestaticapps.net |
| Latest Release            | Test release v0.1.0-alpha                             |
| Latest Release Tag        | v0.1.0-alpha                                          |
| Latest Stable Release     | n/a                                                   |
| Latest Stable Release Tag | n/a                                                   |

- [Latest Release](https://github.com/nhsexperience/nhs-experience/releases/latest)
- ![Azure CI/CD](https://github.com/nhsexperience/nhs-experience/actions/workflows/azure-static-web-apps-zealous-beach-05e189903.yml/badge.svg?branch=main)