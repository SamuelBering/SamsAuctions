using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SamsAuctions.Models;
using SamsAuctions.Models.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Users.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;

        public AccountController(UserManager<AppUser> usrMgr, SignInManager<AppUser> signinMgr)
        {
            userManager = usrMgr;
            signInManager = signinMgr;
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

        private async Task<IdentityResult> CreateAssociatedAccount(string username, ExternalLoginInfo info)
        {

            AppUser user = new AppUser
            {
                UserName = username,
            };

            IdentityResult result = await userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                await userManager.AddLoginAsync(user, info);
                await userManager.AddToRoleAsync(user, "Regular");
            }

            return result;
        }

        //http://localhost:50278/account/loginfacebookcallback 
        [AllowAnonymous]
        public async Task<IActionResult> LoginFacebookCallback()
        {

            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();

            await signInManager.SignOutAsync();
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (!signInResult.Succeeded)
            {
                var emailClaim = info.Principal.FindFirst(c => c.Type.Contains("emailaddress"));
                var createResult = await CreateAssociatedAccount(emailClaim.Value, info);

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

                return RedirectToAction("Login");
            }

            return RedirectToAction("Index", "Home");

            //info.Principal //the IPrincipal with the claims from facebook
            //info.ProviderKey //an unique identifier from Facebook for the user that just signed in
            //info.LoginProvider //a string with the external login provider name, in this case Facebook

            //to sign the user in if there's a local account associated to the login provider
            //var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);            
            //result.Succeeded will be false if there's no local associated account 

            //to associate a local user account to an external login provider
            //await _userInManager.AddLoginAsync(aUserYoullHaveToCreate, info);        


            //return View(model);
            //return Redirect("~/");
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

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        //[AllowAnonymous]
        //public IActionResult Register()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Register(CreateModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        AppUser user = new AppUser
        //        {
        //            UserName = model.Email,
        //            FirstName = model.FirstName,
        //            LastName = model.LastName,
        //            StreetAddress = model.StreetAddress,
        //            ZipCode = model.ZipCode,
        //            PostTown = model.PostTown,
        //            Email = model.Email,
        //        };

        //        IdentityResult result = await userManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            await userManager.AddToRoleAsync(user, "RegularUser");

        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            foreach (IdentityError error in result.Errors)
        //            {
        //                ModelState.AddModelError("", error.Description);
        //            }
        //        }
        //    }
        //    return View(model);
        //}

        [AllowAnonymous]
        public IActionResult AccessDenied(string returnUrl)
        {

            ViewBag.ReturnUrl = returnUrl;
            return View();
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