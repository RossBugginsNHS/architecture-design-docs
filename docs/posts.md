---
title: Posts
layout: page
nav_order: 600
---

# Latest Posts
<ul>
    {% for post in site.posts %}
      <li>
        <a href="{{ post.url }}">{{post.date| date: "%-d %B %Y" }}: {{post.author }} - {{ post.title }}</a>
      </li>
    {% endfor %}
</ul>