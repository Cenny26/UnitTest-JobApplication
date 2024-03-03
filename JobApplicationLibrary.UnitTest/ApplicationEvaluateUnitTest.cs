using Moq;
using JobApplicationLibrary.Models;
using JobApplicationLibrary.Services;

namespace JobApplicationLibrary.UnitTest
{
    public class ApplicationEvaluateUnitTest
    {
        [Test]
        // UnitOfWork_Condition_ExceptedResult
        public void Application_WithUnderAge_TransferredToAutoRejected()
        {
            // Arrange
            var evaluator = new ApplicationEvaluator(null);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 17
                }
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            Assert.AreEqual(ApplicationResult.AutoRejected, appResult);
        }

        [Test]
        public void Application_WithNoTechStack_TransferredToAutoRejected()
        {
            // Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Azerbaijan");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19
                },
                TechStackList = new List<string> { "" }
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            Assert.AreEqual(ApplicationResult.AutoRejected, appResult);
        }

        [Test]
        public void Application_WithTechStackOver75P_TransferredToAutoAccepted()
        {
            // Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Azerbaijan");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19
                },
                TechStackList = new List<string> { "C#", "RabbitMQ", "Microservice", "Visual Studio" },
                YearsOfExperience = 16
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            Assert.AreEqual(ApplicationResult.AutoAccepted, appResult);
        }

        [Test]
        public void Application_WithInValidIdentityNumber_TransferredToHR()
        {
            // Arrange
            var mockValidator = new Mock<IIdentityValidator>(MockBehavior.Strict);
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(false);
            mockValidator.Setup(i => i.CheckConnectionToRemoteServer()).Returns(false);
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Azerbaijan");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19
                }
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            Assert.AreEqual(ApplicationResult.TransferredToHR, appResult);
        }

        [Test]
        public void Application_WithOfficeLocation_TransferredToCTO()
        {
            // Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Turkey");

            //var mockCountryData = new Mock<ICountryData>();
            //mockCountryData.Setup(i => i.Country).Returns("Turkey");

            //var mockProvider = new Mock<ICountryDataProvider>();
            //mockProvider.Setup(i => i.CountryData).Returns(mockCountryData.Object);

            //mockValidator.Setup(i => i.CountryDataProvider).Returns(mockProvider.Object);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19
                }
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            Assert.AreEqual(ApplicationResult.TransferredToCTO, appResult);
        }
    }
}