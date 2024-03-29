﻿using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record TweetPosted(Guid TweetId, Guid ByUserId) : DomainEvent;