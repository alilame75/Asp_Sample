using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Asp_Sample.Areas.Account.Models;
using Core.Identity;
using DataLayer.Context;
using DataLayer.Entity.User;
using DataLayer.Entity.Vote;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;

namespace Asp_Sample.Areas.Account.Controllers
{
    [Area("Account")]
    public class HomeController : Controller
    {
        #region IOC

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IVoteService _voteService;
        private readonly AppDbContext _dbContext;

        //private readonly IMessageSender _messageSender;

        public HomeController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, IVoteService voteService, AppDbContext dbContext
            /*, IMessageSender messageSender*/)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _voteService = voteService;
            _dbContext = dbContext;
            //_messageSender = messageSender;
        }

        #endregion

        #region Account Manager

        [Authorize]
        public async Task<IActionResult> Index()
        {
            ViewData["VoteList"] = await _voteService.GetAllVote(User.Identity.GetUserId(), true);
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Vote(Guid id)
        {
            var vote = await _dbContext.Votes.Where(v => v.VoteId == id).Include(a => a.Options).FirstOrDefaultAsync();
            ViewData["Vote"] = vote;
            //var options = await _dbContext.VoteAnswers.Where(a => a.VoteId == id).ToListAsync();
            ViewData["VoteOptions"] = vote.Options;
            var model = new VoteAnswerDto(){};
            return View(model);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VoteAnswer(VoteAnswerDto dto)
        {
            dto.AnswerIds = dto.AnswerIds.Where(c => c != null).ToList();
            dto.AnswerIds = dto.AnswerIds.Distinct().ToList();


            var active = await _dbContext.Votes
                .Include(v=>v.VoteGroups)
                .ThenInclude(g => g.Group)
                .ThenInclude(u => u.UserGroups)
                .Where(v => v.VoteId == new Guid(dto.VoteId))
                .FirstOrDefaultAsync();

            var activeTime = active.FinishTime - DateTime.Now;
            if ((!active.Active) || (activeTime < TimeSpan.Zero))
            {
                ViewData["Error"] = "زمان این رای گیری به اتمام رسیده است";
                ViewData["VoteList"] = await _voteService.GetAllVote(true);
                return View("Index");
            }

            var voted = await _dbContext.UserVoteAnswers.Where(u => u.UserId == User.Identity.GetUserId())
                .Where(u => u.VoteId == new Guid(dto.VoteId)).FirstOrDefaultAsync();

            if (voted != null)
            {
                ViewData["Error"] = "شما قبلا به این نظرسنجی رای داده اید";
                ViewData["VoteList"] = await _voteService.GetAllVote(true);
                return View("Index");
            }


            if (!active.VoteGroups.Any(a => a.Group.UserGroups.Any(u => u.UserId == User.Identity.GetUserId())))
            {
                ViewData["Error"] = "شما دسترسی لازم برای ثبت رای در این نظر سنجی را ندارید";
                ViewData["VoteList"] = await _voteService.GetAllVote(true);
                return View("Index");
            }

            var res = new List<UserVoteAnswer>();

            foreach (var answer in dto.AnswerIds)
            {
                res.Add(new UserVoteAnswer()
                {
                    UserVoteAnswerId = Guid.NewGuid(),
                    VoteId = new Guid(dto.VoteId),
                    UserId = User.Identity.GetUserId(),
                    SubmitTime = DateTime.Now,
                    VoteOptionId = new Guid(answer),
                });
            }
            await _dbContext.UserVoteAnswers.AddRangeAsync(res);
            await _dbContext.SaveChangesAsync();
            ViewData["Success"] = "رای شما با موفقیت ثبت شد";
            ViewData["VoteList"] = await _voteService.GetAllVote(true);
            return View("Index");
        }


        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());

                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    ViewData["ErrorMessage"] = "رمزعبور شما با موفقیت تغییر یافت";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }


        #endregion

        #region Register

        [Route("Account/Register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [Route("Account/Register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    RegistrationTime = DateTime.Now,
                    Active = true,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //var emailConfirmationToken =
                    //    await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var emailMessage =
                    //    Url.Action("ConfirmEmail", "Account",
                    //        new { username = user.UserName, token = emailConfirmationToken },
                    //        Request.Scheme);
                    //await _messageSender.SendEmailAsync(model.Email, "Email confirmation", emailMessage);

                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View(model);
        }

        #endregion

        #region Login

        [Route("Account/Login")]
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("Index", "Home");

            var model = new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (returnUrl != null)
                ViewData["returnUrl"] = returnUrl;
            return View(model);
        }

        [Route("Account/Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null)
            {
                if (user.Active)
                {
                    if (_signInManager.IsSignedIn(User))
                        return RedirectToAction("Index", "Home");

                    model.ReturnUrl = returnUrl;
                    //model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

                    ViewData["returnUrl"] = returnUrl;

                    if (ModelState.IsValid)
                    {
                        var result = await _signInManager.PasswordSignInAsync(
                            model.UserName, model.Password, model.RememberMe, true);

                        if (result.Succeeded)
                        {
                            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                return Redirect(returnUrl);

                            if (User.IsInRole("Admin"))
                            {
                                return RedirectToAction("Index", "Home", new { area = "Admin" });
                            }

                            return RedirectToAction("Index", "Home");

                        }

                        if (result.IsLockedOut)
                        {
                            ViewData["ErrorMessage"] = "اکانت شما به دلیل پنج بار ورود ناموفق به مدت پنج دقیقه قفل شده است";
                            return View(model);
                        }

                        ModelState.AddModelError("", "رمزعبور یا نام کاربری اشتباه است");
                    }
                }
                else
                {
                    ViewData["ErrorMessage"] = "اکانت شما به صلاح دید ادمین غیرفعال شده است میتوانید با ادمین تماس بگیرید";
                }
                return View(model);

            }

            ViewData["ErrorMessage"] = "یا یوزر نیم یا پسورد اشتباه است.";
            return View(model);
    }

        #endregion

        #region LogOut

        [Route("Account/Logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Remote Validation

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Json(true);
            return Json("ایمیل وارد شده از قبل موجود است");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsUserNameInUse(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return Json(true);
            return Json("نام کاربری وارد شده از قبل موجود است");
        }

        #endregion

        #region Email Confirmation

        //[HttpGet]
        //public async Task<IActionResult> ConfirmEmail(string userName, string token)
        //{
        //    if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(token))
        //        return NotFound();
        //    var user = await _userManager.FindByNameAsync(userName);
        //    if (user == null) return NotFound();
        //    var result = await _userManager.ConfirmEmailAsync(user, token);

        //    return Content(result.Succeeded ? "Email Confirmed" : "Email Not Confirmed");
        //}

        #endregion

        #region External Login

        //[HttpPost]
        //public IActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    var redirectUrl = Url.Action("ExternalLoginCallBack", "Account",
        //        new { ReturnUrl = returnUrl });

        //    var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        //    return new ChallengeResult(provider, properties);
        //}

        //public async Task<IActionResult> ExternalLoginCallBack(string returnUrl = null, string remoteError = null)
        //{
        //    returnUrl =
        //        (returnUrl != null && Url.IsLocalUrl(returnUrl)) ? returnUrl : Url.Content("~/");

        //    var loginViewModel = new LoginViewModel()
        //    {
        //        ReturnUrl = returnUrl,
        //        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
        //    };

        //    if (remoteError != null)
        //    {
        //        ModelState.AddModelError("", $"Error : {remoteError}");
        //        return View("Login", loginViewModel);
        //    }

        //    var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
        //    if (externalLoginInfo == null)
        //    {
        //        ModelState.AddModelError("ErrorLoadingExternalLoginInfo", $"مشکلی پیش آمد");
        //        return View("Login", loginViewModel);
        //    }

        //    var signInResult = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider,
        //        externalLoginInfo.ProviderKey, false, true);

        //    if (signInResult.Succeeded)
        //    {
        //        return Redirect(returnUrl);
        //    }

        //    var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);

        //    if (email != null)
        //    {
        //        var user = await _userManager.FindByEmailAsync(email);
        //        if (user == null)
        //        {
        //            var userName = email.Split('@')[0];
        //            user = new IdentityUser()
        //            {
        //                UserName = (userName.Length <= 10 ? userName : userName.Substring(0, 10)),
        //                Email = email,
        //                EmailConfirmed = true
        //            };

        //            await _userManager.CreateAsync(user);
        //        }

        //        await _userManager.AddLoginAsync(user, externalLoginInfo);
        //        await _signInManager.SignInAsync(user, false);

        //        return Redirect(returnUrl);
        //    }

        //    ViewBag.ErrorTitle = "لطفا با بخش پشتیبانی تماس بگیرید";
        //    ViewBag.ErrorMessage = $"دریافت کرد {externalLoginInfo.LoginProvider} نمیتوان اطلاعاتی از";
        //    return View();
        //}

        #endregion

    }
}