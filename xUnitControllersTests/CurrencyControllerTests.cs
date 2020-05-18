using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Controllers;
using Web.ViewModels;
using Xunit;

namespace ControllersUnitTests
{
    public class CurrencyControllerTests
    { 
        public CurrencyControllerTests() => _currencyViewModelService = new Mock<ICurrencyViewModelService>();
        private readonly Mock<ICurrencyViewModelService> _currencyViewModelService;

        private List<CurrencyViewModel> GetTestSessions()
        {
          var sessions = new List<CurrencyViewModel>();

          sessions.Add(new CurrencyViewModel {Id = 1, Name = "USD", BuyRate = 24.04m, SaleRate = 23.52m});

          sessions.Add(new CurrencyViewModel {Id = 3, Name = "EUR", BuyRate = 27.04m, SaleRate = 29.52m});

          return sessions;
        }

        private EditBankAccountViewModel GetEditViewModel(int target = 0) => new EditBankAccountViewModel
        {
                CurrentCurrencyId   = 1,
                CurrentAmount       = 500m,
                CurrentCurrencyName = "USD",
                CurrencyList  = null,
                TargetCurrencyId    = target
        };

        [Fact]
        public async void Edit_ActionExecutes_ReturnsViewForEdit()
        {
          //Arrange
          var bankAccountId = 1034;
          var viewModel     = GetEditViewModel();
          _currencyViewModelService.Setup(service => service.GetBankAccountViewModel(bankAccountId))
                                   .Returns(Task.FromResult(viewModel));
          var controller = new CurrencyController(_currencyViewModelService.Object);

          //Act
          var result = await controller.Edit(bankAccountId);

          //Assert
          var viewResult = Assert.IsType<ViewResult>(result);
          var model      = Assert.IsAssignableFrom<EditBankAccountViewModel>(viewResult.Model);

          Assert.Equal(viewModel.CurrentCurrencyId, model.CurrentCurrencyId);
          Assert.Equal(viewModel.TargetCurrencyId, model.TargetCurrencyId);
          Assert.Equal(viewModel.CurrentAmount, model.CurrentAmount);
        }

        [Fact]
        public async void Edit_PostVM_ReturnRedirect_WhenModelIsValid()
        {
          //Arrange
          var accountId = 1034;
          var targetId  = 2;

          var viewModel = GetEditViewModel(targetId);

          _currencyViewModelService.Setup(service => service.ChangeAccountCurrency(accountId, targetId))
                                   .Returns(Task.CompletedTask);

          var controller = new CurrencyController(_currencyViewModelService.Object);

          //Act
          var result = await controller.Edit(accountId, viewModel);

          //Assert
          var localRedirectResult = Assert.IsType<LocalRedirectResult>(result);
          Assert.Equal("~/BankAccount/GetAccounts", localRedirectResult.Url);
        }

        [Fact]
        public async void GetInfo_ActionExecutes_ReturnCurrencyViewModel()
        {
          //Arrange
          var list = new List<CurrencyViewModel>(GetTestSessions());
          _currencyViewModelService.Setup(service => service.GetCurrencyRate())
                                   .Returns(Task.FromResult(list));
          var controller = new CurrencyController(_currencyViewModelService.Object);

          //Act
          var result = await controller.GetInfo();

          //Assert
          var viewResult = Assert.IsType<ViewResult>(result);
          var model      = Assert.IsAssignableFrom<IEnumerable<CurrencyViewModel>>(viewResult.ViewData.Model);
          Assert.Equal(expected: 2, model.Count());
        }
    }
}