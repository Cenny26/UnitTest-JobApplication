using JobApplicationLibrary.Enums;

namespace JobApplicationLibrary.Services
{
    public interface IIdentityValidator
    {
        bool IsValid(string identityNumber);
        bool CheckConnectionToRemoteServer();
        ICountryDataProvider CountryDataProvider { get; }
        public ValidationMode ValidationMode { get; set; }
    }
}