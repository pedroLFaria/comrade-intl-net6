﻿using Comrade.Api.UseCases.V1.AirplaneApi;
using Comrade.Persistence.DataAccess;
using Comrade.UnitTests.Helpers;
using MediatR;

namespace Comrade.UnitTests.Tests.AirplaneTests.Bases;

public class AirplaneInjectionController
{
    public static AirplaneController GetAirplaneController(ComradeContext context,
        IMediator mediator)
    {
        var mapper = MapperHelper.ConfigMapper();

        var logger = Mock.Of<ILogger<AirplaneController>>();

        var airplaneCommand =
            AirplaneInjectionService.GetAirplaneCommand(context!, mapper, mediator);
        var airplaneQuery = AirplaneInjectionService.GetAirplaneQuery(context!, mapper);
        return new AirplaneController(airplaneCommand, airplaneQuery, logger);
    }
}