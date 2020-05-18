using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using BankingSystem.ApplicationCore.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Controllers;
using Xunit;

namespace ControllersUnitTests
{
    public class AdminControllerTests
    {
        private List<Client> GetFakeClients()
        {
            var clients = new List<Client>
            {
                new Client
                {
                    IdClient  = 1,
                    Address   = "Address 1",
                    Login     = "client1@gmail.com",
                    TelNumber = "380111111111"
                },
                new Client
                {
                    IdClient  = 2,
                    Address   = "Address 2",
                    Login     = "client2@gmail.com",
                    TelNumber = "380222222222"
                },
                new Client
                {
                    IdClient  = 3,
                    Address   = "Address 3",
                    Login     = "client3@gmail.com",
                    TelNumber = "380333333333"
                }
            };

            return clients;
        }
        
        [Fact]
        public async Task Index_Get_GetClients()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<IdentityUser>>();
            var mockLegalPerson    = new Mock<IAsyncRepository<LegalPerson>>();
            var mockPhysicalPerson = new Mock<IAsyncRepository<PhysicalPerson>>();
            var mockMediator       = new Mock<IMediator>();

            var mockClient = new Mock<IAsyncRepository<Client>>();
            mockClient.Setup(m => m.GetAll()).ReturnsAsync(GetFakeClients());
            
            
            mockUserManager.Setup(x=>x.FindByIdAsync(It.IsAny<string>()))
                .Returns(() => null);

            var controller = new AdminController(mockClient.Object, mockMediator.Object, mockUserManager.Object, 
                mockPhysicalPerson.Object, mockLegalPerson.Object);

            // // Act
            // var result = controller.Index();
            //
            // // Assert
            // var viewResult = Assert.IsType<ViewResult>(result);
            // var model      = Assert.IsAssignableFrom<IEnumerable<Client>>(viewResult.Model);
            // Assert.Equal(GetFakeClients().Count, model.Count());
        }
        
        
    }
}