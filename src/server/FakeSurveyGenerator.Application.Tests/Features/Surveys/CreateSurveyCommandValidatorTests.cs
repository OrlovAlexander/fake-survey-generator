﻿using AutoFixture;
using FakeSurveyGenerator.Application.Features.Surveys;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace FakeSurveyGenerator.Application.Tests.Features.Surveys;

public sealed class CreateSurveyCommandValidatorTests
{
    private readonly IFixture _fixture = new Fixture();

    [Fact]
    public void GivenValidCreateSurveyCommand_WhenValidatingCommand_ThenIsValidShouldBeTrue()
    {
        var command = new CreateSurveyCommand
        {
            SurveyTopic = _fixture.Create<string>(),
            NumberOfRespondents = 1,
            RespondentType = _fixture.Create<string>(),
            SurveyOptions = new List<SurveyOptionDto>
            {
                new()
                {
                    OptionText = _fixture.Create<string>(),
                    PreferredNumberOfVotes = _fixture.Create<int>()
                }
            }
        };

        var validator = new CreateSurveyCommandValidator();

        var result = validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void GivenBlankSurveyTopic_WhenValidatingCommand_ThenIsValidShouldBeFalse()
    {
        var command = new CreateSurveyCommand
        {
            SurveyTopic = "",
            NumberOfRespondents = 1,
            RespondentType = _fixture.Create<string>(),
            SurveyOptions = new List<SurveyOptionDto>
            {
                new()
                {
                    OptionText = _fixture.Create<string>(),
                    PreferredNumberOfVotes = 1
                }
            }
        };

        var validator = new CreateSurveyCommandValidator();

        var result = validator.TestValidate(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.SurveyTopic);
    }

    [Fact]
    public void GivenZeroNumberOfRespondents_WhenValidatingCommand_ThenIsValidShouldBeFalse()
    {
        var command = new CreateSurveyCommand
        {
            SurveyTopic = _fixture.Create<string>(),
            NumberOfRespondents = 0,
            RespondentType = _fixture.Create<string>(),
            SurveyOptions = new List<SurveyOptionDto>
            {
                new()
                {
                    OptionText = _fixture.Create<string>(),
                    PreferredNumberOfVotes = 1
                }
            }
        };

        var validator = new CreateSurveyCommandValidator();

        var result = validator.TestValidate(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.NumberOfRespondents);
    }

    [Fact]
    public void GivenEmptyRespondentType_WhenValidatingCommand_ThenIsValidShouldBeFalse()
    {
        var command = new CreateSurveyCommand
        {
            SurveyTopic = _fixture.Create<string>(),
            NumberOfRespondents = 1,
            RespondentType = "",
            SurveyOptions = new List<SurveyOptionDto>
            {
                new()
                {
                    OptionText = _fixture.Create<string>(),
                    PreferredNumberOfVotes = 1
                }
            }
        };

        var validator = new CreateSurveyCommandValidator();

        var result = validator.TestValidate(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.RespondentType);
    }

    [Fact]
    public void GivenEmptySurveyOptions_WhenValidatingCommand_ThenIsValidShouldBeFalse()
    {
        var command = new CreateSurveyCommand
        {
            SurveyTopic = _fixture.Create<string>(),
            NumberOfRespondents = 1,
            RespondentType = _fixture.Create<string>(),
            SurveyOptions = new List<SurveyOptionDto>()
        };

        var validator = new CreateSurveyCommandValidator();

        var result = validator.TestValidate(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.SurveyOptions);
    }

    [Fact]
    public void GivenNullSurveyOptions_WhenValidatingCommand_ThenIsValidShouldBeFalse()
    {
        var command = new CreateSurveyCommand
        {
            SurveyTopic = _fixture.Create<string>(),
            NumberOfRespondents = 1,
            RespondentType = _fixture.Create<string>(),
            SurveyOptions = null
        };

        var validator = new CreateSurveyCommandValidator();

        var result = validator.TestValidate(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.SurveyOptions);
    }

    [Fact]
    public void GivenEmptySurveyOptionText_WhenValidatingCommand_ThenIsValidShouldBeFalse()
    {
        var command = new CreateSurveyCommand
        {
            SurveyTopic = _fixture.Create<string>(),
            NumberOfRespondents = 1,
            RespondentType = _fixture.Create<string>(),
            SurveyOptions = new List<SurveyOptionDto>
            {
                new()
                {
                    OptionText = ""
                }
            }
        };

        var validator = new CreateSurveyCommandValidator();

        var result = validator.TestValidate(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor("SurveyOptions[0].OptionText");
    }
}
