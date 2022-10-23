﻿using LinqToDB.Mapping;

namespace OSS.Twtr.AccountManagement.Infrastructure;

[Table("Accounts")]
public class AccountEntity
{               
    [PrimaryKey] public Guid Id { get; init; } = Guid.NewGuid();

    [Column] public string Username { get; set; }

    [Column] public string PasswordHash { get; set; }
}