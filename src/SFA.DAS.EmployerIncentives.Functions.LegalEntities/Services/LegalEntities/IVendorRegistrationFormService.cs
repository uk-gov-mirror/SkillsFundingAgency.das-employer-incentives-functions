﻿using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Functions.LegalEntities.Services.LegalEntities
{
    public interface IVendorRegistrationFormService
    {
        Task UpdateVrfCaseDetails();
        Task UpdateVrfCaseDetails(long legalEntityId);
        Task UpdateVrfCaseStatus();
    }
}