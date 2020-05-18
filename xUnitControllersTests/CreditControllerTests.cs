using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using ApplicationCore.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Commands;
using Web.Commands.Credit;
using Web.Controllers;
using Web.ViewModels;
using Xunit;

namespace ControllersUnitTests
{
    public class CreditControllerTests
    {
        private List<BankAccount> GetEmptyListBankAccounts()
        {
            var list = new List<BankAccount>();

            return list;
        }

        private List<BankAccount> GetListBankAccounts()
        {
            var list = new List<BankAccount> {new BankAccount()};

            return list;
        }

        private TakeCreditViewModel GetTestTakeCreditViewModel() => new TakeCreditViewModel
        {
            IdClient = 3, BankAccounts = GetListBankAccounts()
        };

        [Fact]
        public async Task AllCredits_WhenIdClientIsNotNull()
        {
            //Arrage
            var mockMediator = new Mock<IMediator>();
            var mockCreditRepository = new Mock<ICreditRepository>();

            var fakeData = new List<Credit> {new Credit()}.AsQueryable();

            mockCreditRepository.Setup(x => x.Credits).Returns(fakeData);

            var controller = new CreditController(mockCreditRepository.Object, mockMediator.Object);

            //Act
            var result = await controller.AllCredits(idClient: 3);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AllCreditClientViewModel>(viewResult.ViewData.Model);
            Assert.Equal(expected: 3, model.IdClient);
            Assert.NotNull(model.Credits);
        }

        [Fact]
        public async Task AllCredits_WhenIdClientIsNull()
        {
            //Arrage
            var mockCreditRepository = new Mock<ICreditRepository>();
            mockCreditRepository.Setup(x => x.Credits).Returns(new List<Credit>
            {
                new Credit
                {
                    IdAccountNavigation =
                        new BankAccount
                        {
                            IdClient = 3
                        },
                    Status = true
                }
            }.AsQueryable);

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: 3)
                .Verifiable();

            //var controller = new CreditController(mockCreditRepository.Object, mockMediator.Object);

            ////Act
            //var result = await controller.AllCredits();

            ////Assert
            //var viewResult = Assert.IsType<ViewResult>(result);
            //var model      = Assert.IsAssignableFrom<AllCreditClientViewModel>(viewResult.ViewData.Model);
            //Assert.Equal(3, model.IdClient);
            //Assert.NotNull(model.Credits);
        }

        [Fact]
        public async Task TakeCreditForm_Get_WhenCreditsIsNotNull()
        {
            //Arrage
            var mockCreditRepository = new Mock<ICreditRepository>();
            var mockMediator = new Mock<IMediator>();
            mockMediator
                .Setup(m => 
                    m.Send(It.IsAny<GetBankAccountsWithoutActiveCreditQuery>(), 
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(GetListBankAccounts())
                .Verifiable();
            var controller = new CreditController(mockCreditRepository.Object, mockMediator.Object);

            //Act
            var result = await controller.TakeCreditForm(idClient: 3);

            //Assign
            var request = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<TakeCreditViewModel>(request.ViewData.Model);
            Assert.Equal(expected: 3, model.IdClient);
        }

        [Fact]
        public async Task TakeCreditForm_Get_WhenCreditsIsNull()
        {
            //Arrage
            var mockCreditRepository = new Mock<ICreditRepository>();
            var mockMediator = new Mock<IMediator>();
            mockMediator
                .Setup(m => m.Send(It.IsAny<GetBankAccountsWithoutActiveCreditQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(GetEmptyListBankAccounts())
                .Verifiable();
            var controller = new CreditController(mockCreditRepository.Object, mockMediator.Object);

            //Act
            var result = await controller.TakeCreditForm(idClient: 3);

            //Assign
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("NonAccounts", redirect.ActionName);
        }

        [Fact]
        public async Task TakeCreditForm_Post_WhenModelIsInvalid()
        {
            //Arrage
            var mockCreditRepository = new Mock<ICreditRepository>();
            var mockMediator = new Mock<IMediator>();
            var testModel = GetTestTakeCreditViewModel();

            var controller = new CreditController(mockCreditRepository.Object, mockMediator.Object);
            controller.ModelState.AddModelError("IdClient", "Required");

            //Act
            var result = await controller.TakeCreditForm(testModel);

            //Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.Count == 1);
        }

        [Fact]
        public async Task TakeCreditForm_Post_WhenModelIsValid()
        {
            //Arrage
            var mockCreditRepository = new Mock<ICreditRepository>();
            var mockMediator = new Mock<IMediator>();
            var testModel = GetTestTakeCreditViewModel();

            var controller = new CreditController(mockCreditRepository.Object, mockMediator.Object);

            //Act
            var result = await controller.TakeCreditForm(testModel);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AllCredits", redirectToActionResult.ActionName);
        }
    }
}