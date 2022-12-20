---
layout: post
title:  RabbitMq Stream Client v1.0.0 Released
author: Ross Buggins
tags: dotnet rabbitmq
categories: Development
---

The first non RC RabbitMq Stream Client has been released.

Easily install into dotnet with nuget:

```
dotnet add package RabbitMQ.Stream.Client --version 1.0.0
```

[https://github.com/rabbitmq/rabbitmq-stream-dotnet-client/releases/tag/v1.0.0](https://github.com/rabbitmq/rabbitmq-stream-dotnet-client/releases/tag/v1.0.0)

[https://www.nuget.org/packages/RabbitMQ.Stream.Client/](https://www.nuget.org/packages/RabbitMQ.Stream.Client/
)


## Breaking changes

- Adding ILogger abstraction and replacing LogEventSource with it by @ricardSiliuk in #190 (See the documentation)
- Rename the SuperStreamConsumer to RawSuperStreamConsumer by @Gsantomaggio in #192

## Enhancements
- Add SuperStream consumer info by @Gsantomaggio in #187
- Make the AddressResolver more generic by @Gsantomaggio in #189
- Add documentation for logging by @Gsantomaggio in #193
- Complete Super Stream Single Active consumer example by @Gsantomaggio in #197
- Change the error messages by @Gsantomaggio in #198
- Reduce the class's visibility by @Gsantomaggio in #199
- Sync-up editorconfig with RabbitMQ .NET client by @lukebakken in #194
- Add TaskCompletionSource to wait the test instead by @Gsantomaggio in #201
- Full Changelog: v1.0.0-rc.8...v1.0.0