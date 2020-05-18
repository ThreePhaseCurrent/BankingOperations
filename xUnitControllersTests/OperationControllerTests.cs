using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using BankingSystem.ApplicationCore.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Commands.Operation;
using Web.Controllers;
using Web.ViewModels;
using Xunit;

namespace ControllersUnitTests
{
    public class OperationControllerTests
    {
        private IList<Operation> GetFakeOperations()
        {
            return new List<Operation>()
            {
                new Operation(),
                new Operation()
            };
        }
        
        
        [Fact]
        public async Task Index_Get_WhenIdAndFormModelNotNull()
        {
            //arrange
            var id = 3;
            var testViewModel = new AccountOperationViewModel()
            {
                Operations = GetFakeOperations(),
                Amount = 20000
            };

            var loggerMock = new Mock<ILogger<OperationController>>();
            var accountMock = new Mock<IAsyncRepository<BankAccount>>();
            var mediatorMock = new Mock<IMediator>();
            
            mediatorMock.Setup(m => 
                    m.Send(It.IsAny<GetAccountOperationQuery>(), 
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(testViewModel)
                .Verifiable();
            
            mediatorMock.Setup(m => 
                    m.Send(It.IsAny<TransferValidationCommand>(), 
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .Verifiable();
            
            var controller = new OperationController(mediatorMock.Object, accountMock.Object, loggerMock.Object);

            //act
            var result = await controller.Index(id, testViewModel);
            
            //assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AccountOperationViewModel>(viewResult.Model);
            Assert.Equal(expected: 20000, model.Amount);
            Assert.Equal(2, model.Operations.Count);
        }

        [Fact]
        public async Task Transfer_Get_WhenIdNotNull()
        {
            //arrange
            var id = 3;
            
            var loggerMock = new Mock<ILogger<OperationController>>();
            var accountMock = new Mock<IAsyncRepository<BankAccount>>();
            var mediatorMock = new Mock<IMediator>();

            var controller = new OperationController(mediatorMock.Object, accountMock.Object, loggerMock.Object);
            
            //act
            var result = controller.Transfer(id);
            
            //assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<TransferViewModel>(viewResult.Model);
            Assert.Equal(3, model.IdFrom);
        }

        [Fact]
        public async Task Transfer_Post_WhenModelIsValid()
        {
            //arrange
            var testTransferModel = new TransferViewModel()
            {
                IdFrom = 3,
                IdTo = 4,
                Amount = 2000
            };
            
            var loggerMock = new Mock<ILogger<OperationController>>();
            var accountMock = new Mock<IAsyncRepository<BankAccount>>();
            var mediatorMock = new Mock<IMediator>();

            accountMock.Setup(x => x.GetById(testTransferModel.IdFrom))
                .ReturnsAsync(new BankAccount() {IdAccount = 3, IdCurrency = 1, Amount = 500000});
            accountMock.Setup(x => x.GetById(testTransferModel.IdTo))
                .ReturnsAsync(new BankAccount() {IdAccount = 4, IdCurrency = 1});
            
            mediatorMock.Setup(x => x.Send(It.IsAny<TransferAmountCommand>(), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .Verifiable();
            
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(), 
                It.IsAny<ValidationStateDictionary>(), 
                It.IsAny<string>(), 
                It.IsAny<Object>()));
            

            var controller = new OperationController(mediatorMock.Object, accountMock.Object, loggerMock.Object);
            controller.ObjectValidator = objectValidator.Object;
            //act
            var result = await controller.Transfer(testTransferModel);
            
            //assert
            var redirect = Assert.IsAssignableFrom<RedirectToActionResult>(result);

            Assert.Equal("GetAccounts", redirect.ActionName);
            Assert.Equal("BankAccount", redirect.ControllerName);
        }

        [Fact]
        public async Task AccountExist_Get_WhenAccountExist()
        {
            //arrange
            var IdTo = 3;
            
            var loggerMock = new Mock<ILogger<OperationController>>();
            var accountMock = new Mock<IAsyncRepository<BankAccount>>();
            var mediatorMock = new Mock<IMediator>();

            mediatorMock.Setup(x => x.Send(It.IsAny<CheckAccountExistQuery>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .Verifiable();

            var controller = new OperationController(mediatorMock.Object, accountMock.Object, loggerMock.Object);
            
            //act
            var result = await controller.AccountExist(IdTo);
            
            //assert
            var viewResult = Assert.IsType<JsonResult>(result);
            Assert.True((bool)viewResult.Value);
        }

        [Fact]
        public async Task ActivateAccount_WhenIdNotNull()
        {
            //arrange
            var id = 3;
            
            var loggerMock = new Mock<ILogger<OperationController>>();
            var accountMock = new Mock<IAsyncRepository<BankAccount>>();
            var mediatorMock = new Mock<IMediator>();
            
            accountMock.Setup(x => x.GetById(id))
                .ReturnsAsync(new BankAccount()
                {
                    IdAccount = 3, IdCurrency = 1, Amount = 500000, DateClose = DateTime.Now
                });
            
            var controller = new OperationController(mediatorMock.Object, accountMock.Object, loggerMock.Object);
            
            //act
            var result = await controller.ActivateAccount(id);
            
            //assert
            var redirect = Assert.IsAssignableFrom<RedirectToActionResult>(result);
            Assert.Equal("GetAccounts", redirect.ActionName);
            Assert.Equal("BankAccount", redirect.ControllerName);
        }
    }
}