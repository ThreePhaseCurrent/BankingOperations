using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using ApplicationCore.Interfaces;
using BankingSystem.ApplicationCore.Interfaces;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Commands.Admin;
using Web.ViewModels;

namespace Web.Controllers
{
    [Authorize(Roles = AuthorizationConstants.Roles.ADMINISTRATOR)]
    public class AdminController : Controller
    {
        private readonly IAsyncRepository<Client> _clientRepository;
        private readonly IMediator _mediator;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAsyncRepository<PhysicalPerson> _physicalPersonRepository;
        private readonly IAsyncRepository<LegalPerson> _legalPersonRepository;

        public AdminController(IAsyncRepository<Client> clientRepository, IMediator mediator,
            UserManager<IdentityUser> userManager, IAsyncRepository<PhysicalPerson> physicalPersonRepository,
            IAsyncRepository<LegalPerson> legalPersonRepository)
        {
            _clientRepository = clientRepository;
            _mediator = mediator;
            _userManager = userManager;
            _physicalPersonRepository = physicalPersonRepository;
            _legalPersonRepository = legalPersonRepository;
        }

        public async Task<IActionResult> Test()
        {
            var adminUserName = "admin@gmail.com";
            var adminUser     = new IdentityUser() {UserName = adminUserName, Email = adminUserName};
            await _userManager.CreateAsync(adminUser, AuthorizationConstants.DEFAULT_PASSWORD);

            return RedirectToAction("Index");
        }

        // GET
        public async Task<IActionResult> Index()
        {
            return View(await _clientRepository.GetAll());
        }

        private bool ClientExists(int id)
        {
            return _clientRepository.GetAll().Result.Any(predicate: e => e.IdClient == id);
        }

        public async Task<IActionResult> ShowInfo(int idClient)
        {
            var client = await _clientRepository.GetById(id: idClient);

            if (client == null) return NotFound();

            if (client.PhysicalPerson != null)
            {
                return View(viewName: "PhysicalPersonInfo", model: new PhysicalPersonViewModel
                {
                    Client = client,
                    PhysicalPerson = client.PhysicalPerson
                });
            }

            return View(viewName: "LegalPersonInfo", model: new LegalPersonViewModel
            {
                Client = client,
                LegalPerson = client.LegalPerson
            });
        }

        /// <summary>
        /// Создание клиента
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult CreateClient() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CreateClient(ClientCreateViewModel clientCreateViewModel)
        {
            if (!ModelState.IsValid) return View(model: clientCreateViewModel);

            var result =
                await _mediator.Send(
                    request: new GetPasswordValidationQuery(user: null, password: clientCreateViewModel.Password));
            if (result.Succeeded)
            {
                HttpContext.Session.Set("NewClientData", clientCreateViewModel.Client);
                HttpContext.Session.Set(key: "PassClient", value: clientCreateViewModel.Password);

                //if pass is valid

                if (clientCreateViewModel.IsPhysicalPerson)

                    //PhysicalPerson
                    return RedirectToAction(actionName: nameof(CreatePhysicalPerson),
                        routeValues: clientCreateViewModel);

                //LegalPerson
                return RedirectToAction(actionName: nameof(CreateLegalPerson), routeValues: clientCreateViewModel);
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(key: string.Empty, errorMessage: error.Description);

            return View(model: clientCreateViewModel);
        }
        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult CreatePhysicalPerson()
        {
            var client = HttpContext.Session.Get<Client>(key: "NewClientData");

            var physicalPersonCreateViewModel = new PhysicalPersonViewModel
            {
                Client = client
            };

            return View(model: physicalPersonCreateViewModel);
        }

        // POST: Clients/CreatePhysicalPerson
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CreatePhysicalPerson(PhysicalPersonViewModel physicalPersonViewModel)
        {
            if (ModelState.IsValid)
            {
                var client = physicalPersonViewModel.Client;
                client.PhysicalPerson = physicalPersonViewModel.PhysicalPerson;

                //Add new User to Identity
                var user = new IdentityUser()
                {
                    UserName = physicalPersonViewModel.Client.Login,
                    Email = physicalPersonViewModel.Client.Login,
                    PhoneNumber = physicalPersonViewModel.Client.TelNumber
                };

                var password = HttpContext.Session.Get<string>(key: "PassClient");
                var result = await _userManager.CreateAsync(user: user, password: password);

                if (result.Succeeded)
                {
                    //Set Roles CLIENT to new User
                    await _userManager.AddToRoleAsync(user: user, role: AuthorizationConstants.Roles.CLIENT);

                    //Add to tables Client and PhysicalPerson
                    await _clientRepository.AddAsync(entity: client);

                    if (HttpContext.User.IsInRole(AuthorizationConstants.Roles.ADMINISTRATOR))
                    {
                        return RedirectToAction(actionName: nameof(Index));
                    }

                    return RedirectToAction("UserRegistered", "Account");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(key: string.Empty, errorMessage: error.Description);
            }

            return View(model: physicalPersonViewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult CreateLegalPerson()
        {
            var client = HttpContext.Session.Get<Client>(key: "NewClientData");

            var legalPersonCreateViewModel = new LegalPersonViewModel
            {
                Client = client
            };

            return View(model: legalPersonCreateViewModel);
        }

        // POST: Clients/CreatePhysicalPerson
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CreateLegalPerson(LegalPersonViewModel legalPersonViewModel)
        {
            if (ModelState.IsValid)
            {
                var client = legalPersonViewModel.Client;
                client.LegalPerson = legalPersonViewModel.LegalPerson;

                //Add new User to Identity
                var user = new IdentityUser()
                {
                    UserName = legalPersonViewModel.Client.Login,
                    Email = legalPersonViewModel.Client.Login,
                    PhoneNumber = legalPersonViewModel.Client.TelNumber
                };

                var password = HttpContext.Session.Get<string>(key: "PassClient");
                var result = await _userManager.CreateAsync(user: user, password: password);

                if (result.Succeeded)
                {
                    //Set Roles CLIENT to new User
                    await _userManager.AddToRoleAsync(user: user, role: AuthorizationConstants.Roles.CLIENT);

                    //Add to tables Client and LegalPerson
                    await _clientRepository.AddAsync(entity: client);

                    if (HttpContext.User.IsInRole(AuthorizationConstants.Roles.ADMINISTRATOR))
                    {
                        return RedirectToAction(actionName: nameof(Index));
                    }

                    return RedirectToAction("UserRegistered", "Account");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(key: string.Empty, errorMessage: error.Description);
            }

            return View(model: legalPersonViewModel);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int idClient)
        {
            var client = await _clientRepository.GetById(id: idClient);

            if (client == null) return NotFound();

            var user = await _userManager.FindByNameAsync(userName: client.Login);

            HttpContext.Session.Set(key: "IdIdentity", value: user.Id);

            if (client.PhysicalPerson != null)
            {
                return View(viewName: "EditPhysicalPerson", model: new PhysicalPersonViewModel
                {
                    Client = client,
                    PhysicalPerson = client.PhysicalPerson
                });
            }

            return View(viewName: "EditLegalPerson", model: new LegalPersonViewModel
            {
                Client = client,
                LegalPerson = client.LegalPerson
            });
        }

        // POST: Clients/EditPhysicalPerson/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPhysicalPerson(PhysicalPersonViewModel physicalPersonViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var client = physicalPersonViewModel.Client;
                    var physicalPerson = new PhysicalPerson
                    {
                        IdPerson = physicalPersonViewModel.Client.IdClient,
                        PassportSeries = physicalPersonViewModel.PhysicalPerson.PassportSeries,
                        PassportNumber = physicalPersonViewModel.PhysicalPerson.PassportNumber,
                        IdentificationNumber = physicalPersonViewModel.PhysicalPerson.IdentificationNumber,
                        Name = physicalPersonViewModel.PhysicalPerson.Name,
                        Surname = physicalPersonViewModel.PhysicalPerson.Surname,
                        Patronymic = physicalPersonViewModel.PhysicalPerson.Patronymic
                    };

                    await _clientRepository.UpdateAsync(entity: client);
                    await _physicalPersonRepository.UpdateAsync(physicalPerson);

                    var userId = HttpContext.Session.Get<string>(key: "IdIdentity");

                    var user = await _userManager.FindByIdAsync(userId: userId);

                    user.UserName = physicalPersonViewModel.Client.Login;
                    user.Email = physicalPersonViewModel.Client.Login;
                    user.PhoneNumber = physicalPersonViewModel.Client.TelNumber;

                    await _userManager.UpdateAsync(user: user);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(id: physicalPersonViewModel.Client.IdClient)) return NotFound();

                    throw;
                }

                return RedirectToAction(actionName: nameof(Index));
            }

            return View(model: physicalPersonViewModel);
        }

        // POST: Clients/EditPhysicalPerson/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLegalPerson(LegalPersonViewModel legalPersonViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var client = legalPersonViewModel.Client;
                    var legalPerson = new LegalPerson
                    {
                        IdEdrpou = legalPersonViewModel.Client.IdClient,
                        Director = legalPersonViewModel.LegalPerson.Director,
                        Name = legalPersonViewModel.LegalPerson.Name,
                        OwnershipType = legalPersonViewModel.LegalPerson.OwnershipType
                    };

                    await _clientRepository.UpdateAsync(entity: client);
                    await _legalPersonRepository.UpdateAsync(legalPerson);

                    var userId = HttpContext.Session.Get<string>(key: "IdIdentity");

                    var user = await _userManager.FindByIdAsync(userId: userId);

                    user.UserName = legalPersonViewModel.Client.Login;
                    user.Email = legalPersonViewModel.Client.Login;
                    user.PhoneNumber = legalPersonViewModel.Client.TelNumber;

                    await _userManager.UpdateAsync(user: user);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(id: legalPersonViewModel.Client.IdClient)) return NotFound();

                    throw;
                }

                return RedirectToAction(actionName: nameof(Index));
            }

            return View(model: legalPersonViewModel);
        }

        public async Task<IActionResult> ChangePassword(string email)
        {
            var user = await _userManager.FindByNameAsync(userName: email);

            if (user == null)
                return NotFound();

            var model = new ChangePasswordViewModel
            {
                Email = user.Email
            };

            return View(model: model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model: model);

            var user = await _userManager.FindByNameAsync(userName: model.Email);

            if (user != null)
            {
                var resultPassValid =
                    await _mediator.Send(
                        request: new GetPasswordValidationQuery(user: null, password: model.NewPassword));

                if (resultPassValid.Succeeded)
                {
                    var resultChange = await _userManager.ChangePasswordAsync(user: user,
                        currentPassword: model.OldPassword, newPassword: model.NewPassword);

                    if (resultChange.Succeeded)
                        return RedirectToAction(actionName: "Index");

                    foreach (var error in resultChange.Errors)
                        ModelState.AddModelError(key: string.Empty, errorMessage: error.Description);
                }
                else
                    foreach (var error in resultPassValid.Errors)
                        ModelState.AddModelError(key: string.Empty, errorMessage: error.Description);
            }
            else ModelState.AddModelError(key: string.Empty, errorMessage: "Пользователь не найден");

            return View(model: model);
        }
        
        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int idClient)
        {
            var client = await _clientRepository.GetById(id: idClient);

            if(client == null) return NotFound();

            var user = await _userManager.FindByNameAsync(userName: client.Login);

            HttpContext.Session.Set(key: "IdIdentity", value: user.Id);

            if(client.PhysicalPerson != null)
            {
                return View(viewName: "DeletePhysicalPerson", model: new PhysicalPersonViewModel
                {
                    Client = client,
                    PhysicalPerson = client.PhysicalPerson
                });
            }
            return View(viewName: "DeleteLegalPerson", model: new LegalPersonViewModel
            {
                Client = client,
                LegalPerson = client.LegalPerson
            });
        }

        // POST: Clients/Delete/5
        public async Task<IActionResult> DeleteConfirmed(int idClient)
        {
            //Delete client from Clients and (PhysicalPerson or LegalPerson)
            var client = await _clientRepository.GetById(id: idClient);
            await _clientRepository.DeleteAsync(entity: client);

            //Delete user from Identity
            var userId = HttpContext.Session.Get<string>(key: "IdIdentity");
            var user = await _userManager.FindByIdAsync(userId: userId);
            await _userManager.DeleteAsync(user: user);

            return RedirectToAction(actionName: nameof(Index));
        }
    }
}