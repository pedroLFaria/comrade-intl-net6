﻿using Comrade.Core.AirplaneCore.Commands;
using Comrade.Core.AirplaneCore.Validations;
using Comrade.Core.Bases.Interfaces;
using Comrade.Core.Bases.Results;
using Comrade.Core.Messages;
using Comrade.Domain.Bases;
using Comrade.Domain.Models;
using MediatR;
using System.Threading;

namespace Comrade.Core.AirplaneCore.Handlers;

public class
    AirplaneEditCoreHandler : IRequestHandler<AirplaneEditCommand, ISingleResult<Entity>>
{
    private readonly AirplaneEditValidation _airplaneEditValidation;
    private readonly IAirplaneRepository _repository;
    private readonly IMongoDbContext _mongoDbContext;

    public AirplaneEditCoreHandler(AirplaneEditValidation airplaneEditValidation,
        IAirplaneRepository repository, IMongoDbContext mongoDbContext)
    {
        _airplaneEditValidation = airplaneEditValidation;
        _repository = repository;
        _mongoDbContext = mongoDbContext;
    }

    public async Task<ISingleResult<Entity>> Handle(AirplaneEditCommand request,
        CancellationToken cancellationToken)
    {
        var recordExists = await _repository.GetById(request.Id).ConfigureAwait(false);

        if (recordExists is null)
        {
            return new EditResult<Entity>(false,
                BusinessMessage.MSG04);
        }

        var validate = await _airplaneEditValidation.Execute(request, recordExists)
            .ConfigureAwait(false);

        if (!validate.Success)
        {
            return validate;
        }

        var obj = recordExists;
        HydrateValues(obj, request);

        await _repository.BeginTransactionAsync().ConfigureAwait(false);
        _repository.Update(obj);
        await _repository.CommitTransactionAsync().ConfigureAwait(false);

        return new EditResult<Entity>(true,
            BusinessMessage.MSG02);
    }

    private static void HydrateValues(Airplane target, Airplane source)
    {
        target.Code = source.Code;
        target.PassengerQuantity = source.PassengerQuantity;
        target.Model = source.Model;
    }
}