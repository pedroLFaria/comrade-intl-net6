﻿using Comrade.Core.Bases.Interfaces;
using Comrade.Core.Bases.Results;
using Comrade.Core.Messages;
using Comrade.Core.SecurityCore.Commands;
using Comrade.Core.SystemUserCore;
using Comrade.Core.SystemUserCore.Validations;
using Comrade.Domain.Bases;
using Comrade.Domain.Extensions;
using Comrade.Domain.Models;
using MediatR;
using System.Threading;

namespace Comrade.Core.SecurityCore.Handlers;

public class
    ForgotPasswordCoreHandler : IRequestHandler<ForgotPasswordCommand, ISingleResult<Entity>>
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISystemUserRepository _repository;
    private readonly SystemUserForgotPasswordValidation _systemUserForgotPasswordValidation;
    private readonly IMongoDbContext _mongoDbContext;

    public ForgotPasswordCoreHandler(IPasswordHasher passwordHasher,
        ISystemUserRepository repository,
        SystemUserForgotPasswordValidation systemUserForgotPasswordValidation,
        IMongoDbContext mongoDbContext)
    {
        _passwordHasher = passwordHasher;
        _repository = repository;
        _systemUserForgotPasswordValidation = systemUserForgotPasswordValidation;
        _mongoDbContext = mongoDbContext;
    }

    public async Task<ISingleResult<Entity>> Handle(ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var recordExists = await _repository.GetById(request.Id).ConfigureAwait(false);

        if (recordExists is null)
        {
            return new DeleteResult<Entity>(false,
                BusinessMessage.MSG04);
        }

        var result = _systemUserForgotPasswordValidation.Execute(request, recordExists);
        if (!result.Success) return result;

        var obj = recordExists;

        HydrateValues(obj);

        await _repository.BeginTransactionAsync().ConfigureAwait(false);
        _repository.Update(obj);
        await _repository.CommitTransactionAsync().ConfigureAwait(false);

        return new EditResult<Entity>();
    }

    private void HydrateValues(SystemUser target)
    {
        var ruleForgotPassword = "123456";
        target.Password = _passwordHasher.Hash(ruleForgotPassword);
    }
}