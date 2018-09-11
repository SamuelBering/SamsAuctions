using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamsAuctions.Infrastructure;
using SamsAuctions.Models;
using SamsAuctions.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Users.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;
        private AppIdentityDbContext appIdentityDbContext;

        public AccountController(UserManager<AppUser> usrMgr, SignInManager<AppUser> signinMgr, AppIdentityDbContext appIdentityDbC)
        {
            userManager = usrMgr;
            signInManager = signinMgr;
            appIdentityDbContext = appIdentityDbC;

        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public IActionResult LoginWithFaceBook()
        {
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Facebook", Url.Action("LoginFacebookCallback", "Account"));
            return Challenge(properties, "Facebook");
        }

        private async Task<IdentityResult> CreateAssociatedAccount(AssociateAccountViewModel associateAccountViewModel,
                                                                    ExternalLoginInfo info)
        {

            AppUser user = new AppUser
            {
                UserName = associateAccountViewModel.Email,
                FirstName=associateAccountViewModel.FirstName,
                LastName=associateAccountViewModel.LastName,
                Email=associateAccountViewModel.Email,        
            };

            IdentityResult result = await userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                await userManager.AddLoginAsync(user, info);
                await userManager.AddToRoleAsync(user, "Regular");
            }

            return result;
        }

        [AllowAnonymous]
        public async Task<IActionResult> AssociateAccountCallback()
        {
            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();

            var associateAccountVM = HttpContext.Session.GetJson<AssociateAccountViewModel>("AssociateAccountViewModel");

            var createResult = await CreateAssociatedAccount(associateAccountVM, info);

            if (createResult.Succeeded)
            {
                await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (IdentityError error in createResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View("AssociateAccount", associateAccountVM);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult AssociateAccount(AssociateAccountViewModel associateAccountVM)
        {
            if (ModelState.IsValid)
            {
                var properties = signInManager.ConfigureExternalAuthenticationProperties("Facebook", Url.Action("AssociateAccountCallback", "Account"));

                HttpContext.Session.SetJson("AssociateAccountViewModel", associateAccountVM);


                return Challenge(properties, "Facebook");
                //ExternalLoginInfo info = ViewData["ExternalLoginInfo"] as ExternalLoginInfo;
                //var debug = ViewData["Samuel"] as string;

                //var info = HttpContext.Session.GetJson<ExternalLoginInfo>("ExternalLoginInfo");
            }

            return View("AssociateAccount", associateAccountVM);
        }

        //http://localhost:50278/account/loginfacebookcallback 
        [AllowAnonymous]
        public async Task<IActionResult> LoginFacebookCallback()
        {

            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();
            //HttpContext.Session.SetJson("ExternalLoginInfo", info);

            await signInManager.SignOutAsync();
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (!signInResult.Succeeded)
            {
                var emailClaim = info.Principal.FindFirst(c => c.Type.Contains("emailaddress"));
                var givenNameClaim = info.Principal.FindFirst(c => c.Type.Contains("givenname"));
                var surnameClaim = info.Principal.FindFirst(c => c.Type.Contains("surname"));
                var associateAccountVM = new AssociateAccountViewModel
                {
                    FirstName = givenNameClaim.Value,
                    LastName = surnameClaim.Value,
                    Email = emailClaim.Value
                };

                return View("AssociateAccount", associateAccountVM);


            }

            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel details, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByEmailAsync(details.Email);
                if (user != null)
                {
                    await signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result =
                    await signInManager.PasswordSignInAsync(
                    user, details.Password, false, false);
                    if (result.Succeeded)
                    {
                        return Redirect(returnUrl ?? "/");
                    }
                }
                ModelState.AddModelError(nameof(LoginViewModel.Email),
                "Invalid user or password");
            }
            return View(details);
        }

        [AllowAnonymous]
        public IActionResult AccessDenied(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<JsonResult> DeleteAllRegularUsers()
        {

            using (var transaction = appIdentityDbContext.Database.BeginTransaction())
            {
                var allUsers = userManager.Users.ToList();

                foreach (var user in allUsers)
                {
                    try
                    {
                        var rolesForUser = await userManager.GetRolesAsync(user) as List<string>;
                        var isRegularUser = rolesForUser.Any(r => r == "Regular");

                        if (isRegularUser)
                        {
                            foreach (var role in rolesForUser)
                                await userManager.RemoveFromRoleAsync(user, role);
                            //List Logins associated with user
                            var logins = await userManager.GetLoginsAsync(user);
                            foreach (var login in logins)
                                await userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);

                            //Delete User
                            await userManager.DeleteAsync(user);
                        }
                    }
                    catch (Exception e)
                    {
                        return Json(new { Status = "Error", e.Message, e.StackTrace });
                    }
                }

                transaction.Commit();
            }

            return Json(new { Status = "ok" });
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                };

                IdentityResult result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Regular");

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(EditModel editModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        AppUser user = await userManager.GetUserAsync(User);

        //        var isValidPassword = await userManager.CheckPasswordAsync(user, editModel.Password);
        //        if (!isValidPassword)
        //        {
        //            ModelState.AddModelError(nameof(EditModel.Password), "Felaktigt lösenord");
        //            return View(editModel);
        //        }

        //        if (editModel.NewPassword != null)
        //        {
        //            if (editModel.Password == null)
        //            {
        //                ModelState.AddModelError(nameof(EditModel.Password), "Ange ditt nuvarande lösenord");
        //                return View(editModel);
        //            }

        //            var result = await userManager.ChangePasswordAsync(user, editModel.Password, editModel.NewPassword);

        //            if (!result.Succeeded)
        //            {
        //                AddModelError("", result);
        //                return View(editModel);
        //            }
        //        }

        //        user.FirstName = editModel.FirstName;
        //        user.LastName = editModel.LastName;
        //        user.Email = editModel.Email;
        //        user.StreetAddress = editModel.StreetAddress;
        //        user.PostTown = editModel.PostTown;
        //        user.ZipCode = editModel.ZipCode;

        //        var updateResult = await userManager.UpdateAsync(user);

        //        if (!updateResult.Succeeded)
        //        {
        //            AddModelError("", updateResult);
        //            return View(editModel);
        //        }

        //        return RedirectToAction(nameof(ConfirmAccountUpdate));
        //    }

        //    return View(editModel);
        //}

        //public ViewResult ConfirmAccountUpdate()
        //{
        //    return View();
        //}

        private void AddModelError(string key, IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
                ModelState.AddModelError(key, error.Description);
        }

        //public async Task<ViewResult> Edit()
        //{
        //    AppUser user = await userManager.GetUserAsync(User);

        //    EditModel userVM = new EditModel
        //    {
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        Email = user.Email,
        //        PostTown = user.PostTown,
        //        StreetAddress = user.StreetAddress,
        //        ZipCode = user.ZipCode
        //    };

        //    return View(userVM);
        //}
    }
}