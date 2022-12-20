---
title: Posts
layout: page
nav_order: 600
---

There are {{ site.posts.size }} posts.

## Latest Posts
<ul>
    {% for post in site.posts %}
      <li>
        <a href="{{ post.url }}">{{post.date| date: "%-d %B %Y" }}: {{post.author }} - {{ post.title }}</a>
        {{ post.excerpt }}
      </li>
    {% endfor %}
</ul>

## Tags
{% for tag in site.tags %}
  <h3>{{ tag[0] }}</h3>
  <ul>
    {% for post in tag[1] %}
      <li><a href="{{ post.url }}">{{ post.title }}</a></li>
    {% endfor %}
  </ul>
{% endfor %}

## Categories
{% for category in site.categories %}
  <h3>{{ category[0] }}</h3>
  <ul>
    {% for post in category[1] %}
      <li><a href="{{ post.url }}">{{ post.title }}</a></li>
    {% endfor %}
  </ul>
{% endfor %}