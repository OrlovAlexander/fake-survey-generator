﻿using FakeSurveyGenerator.Application.Shared.Identity;

namespace FakeSurveyGenerator.Application.Infrastructure.Identity;

internal sealed class OAuthUser : IUser
{
    public OAuthUser()
    {
    } // Used for System.Text.Json deserialization

    public OAuthUser(string id, string displayName, string emailAddress) : this()
    {
        Id = id;
        DisplayName = displayName;
        EmailAddress = emailAddress;
    }

    public string Id { get; init; } = null!;
    public string DisplayName { get; init; } = null!;
    public string EmailAddress { get; init; } = null!;
}