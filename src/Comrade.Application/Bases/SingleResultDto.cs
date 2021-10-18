﻿using AutoMapper;
using Comrade.Application.Bases.Interfaces;
using Comrade.Core.Bases.Interfaces;
using Comrade.Core.Bases.Results;
using Comrade.Core.Messages;
using Comrade.Domain.Bases;
using Comrade.Domain.Enums;
using FluentValidation.Results;

namespace Comrade.Application.Bases;

public class SingleResultDto<TDto> : ResultDto, ISingleResultDto<TDto>
    where TDto : Dto
{
    public SingleResultDto(TDto? data)
    {
        Code = data == null ? (int)EnumResponse.NotFound : (int)EnumResponse.Ok;
        Success = data != null;
        Message = data == null
            ? BusinessMessage.MSG04
            : string.Empty;
        Data = data;
    }

    public SingleResultDto(ValidationResult validationResult)
    {
        Code = (int)EnumResponse.ErrorBusinessValidation;
        Success = false;
        Messages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
        ValidationResult = validationResult;
    }

    public SingleResultDto(SecurityResult errorSecurity)
    {
        Code = errorSecurity.Code;
        Success = false;
        Message = errorSecurity.ErrorMessage;
    }


    public SingleResultDto(Exception ex)
    {
        Code = (int)EnumResponse.InternalServerError;
        Success = false;
        Message = ex.Message;
        ExceptionMessage = ex.Message;
    }

    public SingleResultDto(IEnumerable<string> listErrors)
    {
        Code = (int)EnumResponse.ErrorBusinessValidation;
        Success = false;
        Messages = listErrors.ToList();
    }

    public SingleResultDto(IResult result)
    {
        Code = result.Code;
        Success = result.Success;
        Message = result.Message;
    }

    public ValidationResult? ValidationResult { get; }

    public TDto? Data { get; private set; }

    public void SetData<TEntity>(ISingleResult<TEntity> result, IMapper mapper)
        where TEntity : Entity
    {
        Data = mapper.Map<TDto>(result.Data);
    }
}