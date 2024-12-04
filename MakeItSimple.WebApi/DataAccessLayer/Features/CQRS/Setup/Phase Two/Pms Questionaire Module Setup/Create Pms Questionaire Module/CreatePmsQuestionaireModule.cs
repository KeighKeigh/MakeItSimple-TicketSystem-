﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup
{
    public partial class CreatePmsQuestionaireModule
    {

        public class Handler : IRequestHandler<CreatePmsQuestionaireModuleCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(CreatePmsQuestionaireModuleCommand command, CancellationToken cancellationToken)
            {


                var pmsFormIdNotExist = await unitOfWork.PmsForm.PmsFormIdNotExist(command.PmsFormId);
                if (pmsFormIdNotExist is null)
                    return Result.Failure(PmsFormError.PmsFormIdNotExist());

                var questionaireModuleNameAlreadyExist = await unitOfWork.PmsQuestionaireModules
                    .QuestionaireModuleNameAlreadyExist(command.Questionaire_Module_Name);
                if (questionaireModuleNameAlreadyExist)
                    return Result.Failure(PmsQuestionaireModuleError.PmsQuestionaireModuleAlreadyExist());

                await unitOfWork.PmsQuestionaireModules.CreateQuestionaireModule(command);
               
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}