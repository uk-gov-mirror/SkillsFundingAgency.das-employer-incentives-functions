﻿namespace SFA.DAS.EmployerIncentives.Functions.LegalEntities.Services.Withdrawals.Types
{
    public class WithdrawRequest
    {
        public WithdrawalType WithdrawalType { get; set; }
        public long AccountLegalEntityId { get; set; }
        public long ULN { get; set; }
        public ServiceRequest ServiceRequest { get; set; }
    }
}
