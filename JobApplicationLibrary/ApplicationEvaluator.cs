using JobApplicationLibrary.Enums;
using JobApplicationLibrary.Models;
using JobApplicationLibrary.Services;

namespace JobApplicationLibrary
{
    public class ApplicationEvaluator
    {
        private const int minAge = 18;
        private const int autoAcceptedYearOfExperience = 15;
        private readonly IIdentityValidator identityValidator;
        private List<string> techStackList = new() { "C#", "RabbitMQ", "Microservice", "Visual Studio" };

        public ApplicationEvaluator(IIdentityValidator identityValidator)
        {
            this.identityValidator = identityValidator;
        }

        public ApplicationResult Evaluate(JobApplication form)
        {
            #region Application_WithNullApplicant_ThrowsArgumentNullException
            if (form.Applicant is null)
                throw new ArgumentNullException();
            #endregion

            #region Application_WithUnderAge_TransferredToAutoRejected
            if (form.Applicant.Age < minAge)
                return ApplicationResult.AutoRejected;
            #endregion

            #region Application_WithOver50_ValidationModeToDetailed
            identityValidator.ValidationMode = form.Applicant.Age > 50 ? ValidationMode.Detailed : ValidationMode.Detailed;
            #endregion

            #region Application_WithOfficeLocation_TransferredToCTO
            if (identityValidator.CountryDataProvider.CountryData.Country != "Azerbaijan")
                return ApplicationResult.TransferredToCTO;
            #endregion

            #region Application_WithDefaultValue_IsValidCalled & Application_WithYoungAge_IsValidNeverCalled
            var validIdentity = identityValidator.IsValid(form.Applicant.IdentityNumber);
            #endregion

            #region Application_WithInValidIdentityNumber_TransferredToHR
            if (!validIdentity)
                return ApplicationResult.TransferredToHR;
            #endregion

            var sr = GetTechStackSimilarityRate(form.TechStackList);

            #region Application_WithNoTechStack_TransferredToAutoRejected
            if (sr < 25)
                return ApplicationResult.AutoRejected;
            #endregion

            #region Application_WithTechStackOver75P_TransferredToAutoAccepted
            if (sr > 75 && form.YearsOfExperience >= autoAcceptedYearOfExperience)
                return ApplicationResult.AutoAccepted;
            #endregion

            return ApplicationResult.TryAgain;
        }

        private int GetTechStackSimilarityRate(List<string> techStacks)
        {
            var matchedCount =
                techStacks
                .Where(i => techStackList.Contains(i, StringComparer.OrdinalIgnoreCase))
                .Count();

            return (int)((double)matchedCount / techStackList.Count) * 100;
        }
    }
}
