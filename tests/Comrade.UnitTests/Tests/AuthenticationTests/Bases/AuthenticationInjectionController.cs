﻿#region

using Comrade.Api.UseCases.V1.LoginApi;
using Comrade.Persistence.DataAccess;
using Comrade.UnitTests.Helpers;

#endregion

namespace Comrade.UnitTests.Tests.AuthenticationTests.Bases;

public class AuthenticationInjectionController
{
    private readonly AuthenticationInjectionService _authenticationInjectionService = new();

    public AuthenticationController GetAuthenticationController(ComradeContext context)
    {
        var mapper = MapperHelper.ConfigMapper();

        var authenticationCommand =
            _authenticationInjectionService.GetAuthenticationCommand(context, mapper);

        return new AuthenticationController(authenticationCommand);
    }
}
