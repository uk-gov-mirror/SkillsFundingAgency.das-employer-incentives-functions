﻿using Microsoft.Azure.WebJobs;
using SFA.DAS.EmployerIncentives.Functions.LegalEntities.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Infrastructure;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Messages.Events;

namespace SFA.DAS.EmployerIncentives.Functions.LegalEntities
{
    public class HandleUpdateLegalEntityVrfCaseDetailsEvent
    {
        private readonly ILegalEntitiesService _legalEntitiesService;

        public HandleUpdateLegalEntityVrfCaseDetailsEvent(ILegalEntitiesService legalEntitiesService)
        {
            _legalEntitiesService = legalEntitiesService;
        }

        [FunctionName("HandleGetLegalEntityVrfCaseDetailsEvent")]
        public Task RunEvent([NServiceBusTrigger(Endpoint = QueueNames.UpdateLegalEntityVrfCaseDetailsEvent)] UpdateLegalEntityVrfCaseDetailsEvent message)
        {
            return _legalEntitiesService.UpdateVrfCaseDetails(message.LegalEntityId);
        }
    }
}