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
using Web.Commands.Deposit;
using Web.Controllers;
using Web.ViewModels;
using Xunit;

namespace ControllersUnitTests
{
    public class DepositControllerTests
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

        private MakeDepositViewModel GetTestTakeDepositViewModel() => new MakeDepositViewModel()
        {
            IdClient = 3, BankAccounts = GetListBankAccounts()
        };
        
        [Fact]
        public async Task AllDeposits_WhenIdClientIsNotNull()
        {
            //Arrage
            var mockMediator = new Mock<IMediator>();
            var mockDepositRepository = new Mock<IDepositRepository>();

            var fakeData = new List<Deposit> {new Deposit()}.AsQueryable();

            mockDepositRepository.Setup(x => x.Deposits).Returns(fakeData);

            var controller = new DepositController(mockMediator.Object, mockDepositRepository.Object);

            //Act
            var result = await controller.AllDeposits(idClient: 3);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AllDepositsClientViewModel>(viewResult.ViewData.Model);
            Assert.Equal(expected: 3, model.IdClient);
            Assert.NotNull(model.Deposits);
        }
        
        [Fact]
        public async Task AllCredits_WhenIdClientIsNull()
        {
            //Arrage
            var mockCreditRepository = new Mock<IDepositRepository>();
            mockCreditRepository.Setup(x => x.Deposits).Returns(new List<Deposit>
            {
                new Deposit()
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
            mockMediator.Setup(m => 
                    m.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
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
        public async Task MakeDeposit_Get_WhenDepositIsNotNull()
        {
            //Arrage
            var mockDepositRepository = new Mock<IDepositRepository>();
            var mockMediator = new Mock<IMediator>();
            mockMediator
                .Setup(m => 
                    m.Send(It.IsAny<GetBankAccountsWithoutActiveDepositQuery>(), 
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(GetListBankAccounts())
                .Verifiable();
            
            var controller = new DepositController(mockMediator.Object, mockDepositRepository.Object);

            //Act
            var result = await controller.MakeDeposit(idClient: 3);
            
            //Assign
            var request = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<MakeDepositViewModel>(request.ViewData.Model);
            Assert.Equal(expected: 3, model.IdClient);
        }
        
        [Fact]
        public async Task MakeDeposit_Get_WhenDepositIsNull()
        {
            //Arrage
            var mockDepositRepository = new Mock<IDepositRepository>();
            var mockMediator = new Mock<IMediator>();
            mockMediator
                .Setup(m => 
                    m.Send(It.IsAny<GetBankAccountsWithoutActiveDepositQuery>(), 
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(GetEmptyListBankAccounts())
                .Verifiable();
            
            var controller = new DepositController(mockMediator.Object, mockDepositRepository.Object);

            //Act
            var result = await controller.MakeDeposit(idClient: 3);

            //Assign
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("NonAccounts", redirect.ActionName);
        }
        
        [Fact]
        public async Task MakeDeposit_Post_WhenModelIsInvalid()
        {
            //Arrage
            var mockCreditRepository = new Mock<IDepositRepository>();
            var mockMediator = new Mock<IMediator>();
            var testModel = GetTestTakeDepositViewModel();

            var controller = new DepositController(mockMediator.Object, mockCreditRepository.Object);
            controller.ModelState.AddModelError("IdClient", "Required");

            //Act
            var result = await controller.MakeDeposit(testModel);

            //Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.Count == 1);
        }
        
        [Fact]
        public async Task MakeDeposit_Post_WhenModelIsValid()
        {
            //Arrage
            var mockCreditRepository = new Mock<IDepositRepository>();
            var mockMediator = new Mock<IMediator>();
            var testModel = GetTestTakeDepositViewModel();

            var controller = new DepositController(mockMediator.Object, mockCreditRepository.Object);

            //Act
            var result = await controller.MakeDeposit(testModel);

            //Assert
            var redirectToActionResult = Assert.IsType<ViewResult>(result);
            //TODO:
        }
    }
}