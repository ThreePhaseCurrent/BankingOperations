using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using ApplicationCore.Interfaces;
using BankingSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Web.Controllers;
using Web.ViewModels;
using Xunit;

namespace ControllersUnitTests
{
    public class BankAccountControllerTests
    {
        private PhysicalPerson GetTestPhysicalPerson(int idClient)
        {
            if (idClient != 3)
                return null;

            return new PhysicalPerson
            {
                IdPerson             = 3,
                PassportSeries       = "AA",
                PassportNumber       = "234567",
                IdentificationNumber = "23421322",
                Surname              = "Шпаков",
                Name                 = "Шпак",
                Patronymic           = "Шпакович"
            };
        }

        private Client GetTestClient(int idClient)
        {
            if (idClient == 3)
            {
                return new Client()
                {
                    IdClient = 3,
                    Address = "adasd",
                    PhysicalPerson = GetTestPhysicalPerson(idClient),
                    Login = "asdasdda",
                    TelNumber = "asdadad"
                };
            }
            else
            {
                return new Client()
                {
                    IdClient = 4,
                    Address = "adasd",
                    LegalPerson = GetTestLegalPerson(idClient),
                    Login = "asdasdda",
                    TelNumber = "asdadad"
                };
            }

        }

        private LegalPerson GetTestLegalPerson(int idClient)
        {
            if (idClient == 3)
                return null;

            return new LegalPerson
            {
                IdEdrpou = 4, Name = "ООО Газпром", OwnershipType = "муниципальная", Director = "Тапочков А.Л."
            };
        }
        
        /// <summary>
        ///   Тестовая ViewModel для Post метода
        /// </summary>
        /// <param name="idClient"></param>
        /// <returns></returns>
        private CreateClientAccountViewModel GetTestAccountViewModel(int idClient) => new CreateClientAccountViewModel
        {
            Account = new BankAccount {AccountType = "кредитный", IdClient = 3, IdCurrency = 2}
        };
        
        [Theory]
        [InlineData(3)]
        [InlineData(4)]
        public async Task CreateClientAccountForm_Get_WhenClientExist(int idClient)
        {
            var mockBankAccount = new Mock<IBankAccountRepository>();
            var mockClient = new Mock<IAsyncRepository<Client>>();
            mockClient.Setup(m => m.GetById(idClient))
                .ReturnsAsync(GetTestClient(idClient));

            var controller = new BankAccountController(mockBankAccount.Object, mockClient.Object);
            
            //act
            var result = await controller.CreateClientAccountForm(idClient);
            
            //assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CreateClientAccountViewModel>(viewResult.ViewData.Model);

            if (idClient == 3)
            {
                Assert.Equal(expected: 3, model.PhysicalPerson.IdPerson);
                Assert.Equal("Шпак", model.PhysicalPerson.Name);
                Assert.Equal("Шпаков", model.PhysicalPerson.Surname);
                Assert.Equal("Шпакович", model.PhysicalPerson.Patronymic);
                Assert.Equal("AA", model.PhysicalPerson.PassportSeries);
                Assert.Equal("234567", model.PhysicalPerson.PassportNumber);
                Assert.Equal("23421322", model.PhysicalPerson.IdentificationNumber);
            }
            else
            {
                Assert.Equal(expected: 4, model.LegalPerson.IdEdrpou);
                Assert.Equal("ООО Газпром", model.LegalPerson.Name);
                Assert.Equal("муниципальная", model.LegalPerson.OwnershipType);
                Assert.Equal("Тапочков А.Л.", model.LegalPerson.Director);
            }
        }

        /// <summary>
        ///   Тест при невалидности модели
        /// </summary>
        /// <param name="idClient"></param>
        /// <returns></returns>
        [Theory]
        [InlineData(3)]
        public async Task CreateClientAccountForm_Post_CreatedAccount(int idClient)
        {
            var mockBankAccount = new Mock<IBankAccountRepository>();
            var mockClient = new Mock<IAsyncRepository<Client>>();
            mockClient.Setup(m => m.GetById(idClient))
                .ReturnsAsync(GetTestClient(idClient));

            var controller = new BankAccountController(mockBankAccount.Object, mockClient.Object);
            
            controller.ModelState.AddModelError("Account", "Required");
            var newViewModel = GetTestAccountViewModel(idClient);

            //Act
            var result = await controller.CreateClientAccountForm(newViewModel);

            //Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.Count == 1);
        }
        
        private static DbSet<T> GetQueryableMockDbSet<T>(params T[] sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return dbSet.Object;
        }

        /// <summary>
        ///   Закрытие счета
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task BankAccountClose_Get_WhenAccountExist()
        {
            var mockBankAccount = new Mock<IBankAccountRepository>();

            mockBankAccount.Setup(x => x.Accounts)
                .Returns(GetQueryableMockDbSet<BankAccount>());
            
            var fakeData = new List<BankAccount> {new BankAccount()}.AsQueryable();

            var mock = fakeData.AsQueryable().BuildMock();

            mockBankAccount.Setup(x => x.Accounts)
                .Returns(mock.Object);


            mockBankAccount.Setup(x => x.CloseAccount(3))
                .Returns(new Task<bool>(() => true))
                .Verifiable();
            var mockClient = new Mock<IAsyncRepository<Client>>();

            var controller = new BankAccountController(mockBankAccount.Object, mockClient.Object);
            
            //Act
            var result = await controller.BankAccountClose(idAccount: 3);

            //Assert
            var request = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("GetAccounts", request.ActionName);
        }

        /// <summary>
        ///   Удаление счета
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task BankAccountDelete_Get_WhenAccountExist()
        {
            var mockBankAccount = new Mock<IBankAccountRepository>();
            var mockClient = new Mock<IAsyncRepository<Client>>();

            var controller = new BankAccountController(mockBankAccount.Object, mockClient.Object);
            
            //Act
            var result = await controller.BankAccountDelete(idAccount: 3);

            //Assert
            var request = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("GetAccounts", request.ActionName);
        }

        [Fact]
        public async Task GetAccounts_Get_GetAllAccounts()
        {
            //arrange
            int idClient = 3;
            
            var mockBankAccount = new Mock<IBankAccountRepository>();
            var mockClient = new Mock<IAsyncRepository<Client>>();
            
            var fakeData = new List<BankAccount> {new BankAccount()}.AsQueryable();
            mockBankAccount.Setup(x => x.Accounts).Returns(fakeData);

            var controller = new BankAccountController(mockBankAccount.Object, mockClient.Object);
            
            //Act
            var result = await controller.GetAccounts(idClient: idClient);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model      = Assert.IsAssignableFrom<AccountViewModel>(viewResult.ViewData.Model);
            Assert.Equal(idClient, model.IdClient);
            Assert.NotNull(model.BankAccounts);
        }
    }
}