using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_HTH.Models;
using Shop_HTH.Models.ViewModel;
using Shop_HTH.Repository;
using Shopping_HTH.Areas.Admin.Repository;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Shop_HTH.Controllers
{
	public class AccountController : Controller
	{
		private UserManager<AppUserModel> _userManage;
		private SignInManager<AppUserModel> _signInManager;
		private readonly IEmailSender _emailSender;
        private readonly DataContext _dataContext;
        public AccountController(IEmailSender emailSender,SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManage, DataContext context)
		{
			this._userManage = userManage;
			this._signInManager = signInManager;
            this._emailSender = emailSender;
			this._dataContext = context;
        }
		public IActionResult Login(string returnUrl)
		{
			return View(new LoginViewModel { ReturnUrl = returnUrl});
		}
		public async Task<IActionResult> UpdateAccount()
		{
			if ((bool)!User.Identity?.IsAuthenticated)
			{
				return RedirectToAction("Login", "Account");
			}
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var userEmail = User.FindFirstValue(ClaimTypes.Email);

			var user = await _userManage.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
			if (user == null)
			{
				return NotFound();
			}	
			return View(user);
		}
		[HttpPost]
		public async Task<IActionResult> UpdateInforAccount(AppUserModel user, string PasswordHash)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var userByID = await _userManage.Users.FirstOrDefaultAsync(x => x.Id == userId);
			if (userByID == null)
			{
				return NotFound();
			}
			if (PasswordHash != null)
			{
				var passwordHasher = new PasswordHasher<AppUserModel>();
				var passwordHash = passwordHasher.HashPassword(userByID, user.PasswordHash);
				userByID.PasswordHash = passwordHash;
			}	
			userByID.PhoneNumber = user.PhoneNumber;
	
			_dataContext.Update(userByID);

			await _dataContext.SaveChangesAsync();
			TempData["success"] = "Update Account Successfully.";
			return RedirectToAction("UpdateAccount", "Account");
		}
		[HttpPost]
		public async Task<IActionResult> SendMailForgotPass(AppUserModel user)
		{
			var checkMail = await _userManage.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

			if (checkMail == null)
			{
				TempData["error"] = "Email not found";
				return RedirectToAction("ForgotPass", "Account");
			}
			else
			{
				string token = Guid.NewGuid().ToString();
				checkMail.Token = token;
				_dataContext.Update(checkMail);
				await _dataContext.SaveChangesAsync();
				var receiver = checkMail.Email;
				var subject = "Thay đổi mật khẩu cho người dùng " + checkMail.Email;
				var message = "Click vào đường link này để đổi mật khẩu " +
					"<a href='" + $"{Request.Scheme}://{Request.Host}/Account/NewPass?email=" + checkMail.Email + "&token=" + token + "'>";

				await _emailSender.SendEmailAsync(receiver, subject, message);
			}


			TempData["success"] = "Email cập nhật mật khẩu đã được gửi đến Email đăng ký";
			return RedirectToAction("ForgotPass", "Account");
		}
		public async Task<IActionResult> ForgotPass()
		{
			return View();
		}
		public async Task<IActionResult> NewPass(AppUserModel user, string token)
		{
			var checkuser = await _userManage.Users
				.Where(u => u.Email == user.Email)
				.Where(u => u.Token == user.Token).FirstOrDefaultAsync();

			if (checkuser != null)
			{
				ViewBag.Email = checkuser.Email;
				ViewBag.Token = token;
			}
			else
			{
				TempData["error"] = "Email không tồn tại";
				return RedirectToAction("ForgotPass", "Account");
			}
			return View();
		}
		public async Task<IActionResult> UpdateNewPassword(AppUserModel user, string token)
		{
			var checkuser = await _userManage.Users
				.Where(u => u.Email == user.Email)
				.Where(u => u.Token == user.Token).FirstOrDefaultAsync();

			if (checkuser != null)
			{
				string newtoken = Guid.NewGuid().ToString();
				var passwordHasher = new PasswordHasher<AppUserModel>();
				var passwordHash = passwordHasher.HashPassword(checkuser, user.PasswordHash);

				checkuser.PasswordHash = passwordHash;
				checkuser.Token = newtoken;

				await _userManage.UpdateAsync(checkuser);
				TempData["success"] = "Cập nhật password thành công.";
				return RedirectToAction("Login", "Account");
			}
			else
			{
				TempData["error"] = "Email không tồn tại";
				return RedirectToAction("ForgotPass", "Account");
			}
		}
		public IActionResult Create()
		{
			return View();
		}
        public async Task<IActionResult> History()
        {
            if ((bool)!User.Identity?.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var Orders = await _dataContext.Orders
                .Where(od => od.UserName == userEmail).OrderByDescending(od => od.Id).ToListAsync();
            ViewBag.UserEmail = userEmail;
            return View(Orders);
        }

        public async Task<IActionResult> CancelOrder(string ordercode)
        {
            if ((bool)!User.Identity?.IsAuthenticated)
            {
                // User is not logged in, redirect to login
                return RedirectToAction("Login", "Account");
            }
            try
            {
                var order = await _dataContext.Orders.Where(o => o.OrderCode == ordercode).FirstAsync();
                order.Status = 3;
                _dataContext.Update(order);
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                return BadRequest("An error occurred while canceling the order.");
            }


            return RedirectToAction("History", "Account");
        }

        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel loginVM)
		{
			if (ModelState.IsValid)
			{
				Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, false, false);
				if (result.Succeeded)
				{
					return Redirect(loginVM.ReturnUrl ?? "/");
				}
				ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu");
			} 
				
			return View(loginVM);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(UserModel user)
		{
			if (ModelState.IsValid)
			{
				AppUserModel newUser = new AppUserModel
				{
					UserName = user.Username,
					Email = user.Email,
				};
				IdentityResult result = await _userManage.CreateAsync(newUser, user.Password);
				if (result.Succeeded)
				{
					TempData["success"] = "Tạo user thành công";
					return Redirect("/account/login");
				}
				foreach(IdentityError error in result.Errors)
				{
					ModelState.AddModelError("",error.Description);
				}
					
			} 
			return View();
		}
		public async Task<IActionResult> Logout(string returnUrl ="/")
		{
			await HttpContext.SignOutAsync();
			await _signInManager.SignOutAsync();

			return Redirect(returnUrl);
		}
		public async Task LoginByGoogle()
		{
			// Use Google authentication scheme for challenge
			await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
				new AuthenticationProperties
				{
					RedirectUri = Url.Action("GoogleResponse")
				});
		}
		public async Task<IActionResult> GoogleResponse()
		{
			var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

			if (!result.Succeeded)
			{
				return RedirectToAction("Login");
			}

			var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
			{
				claim.Issuer,
				claim.OriginalIssuer,
				claim.Type,
				claim.Value
			});

			var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
			string emailName = email.Split('@')[0];
			var existingUser = await _userManage.FindByEmailAsync(email);

			if (existingUser == null)
			{
				var passwordHasher = new PasswordHasher<AppUserModel>();
				var hashedPassword = passwordHasher.HashPassword(null, "123456789");
				var newUser = new AppUserModel { UserName = emailName, Email = email };
				newUser.PasswordHash = hashedPassword;

				var createUserResult = await _userManage.CreateAsync(newUser);
				if (!createUserResult.Succeeded)
				{
					TempData["error"] = "Đăng ký tài khoản thất bại. Vui lòng thử lại sau.";
					return RedirectToAction("Login", "Account");

				}
				else
				{
					await _signInManager.SignInAsync(newUser, isPersistent: false);
					TempData["success"] = "Đăng ký tài khoản thành công.";
					return RedirectToAction("Index", "Home");
				}

			}
			else
			{
				await _signInManager.SignInAsync(existingUser, isPersistent: false);
			}

			return RedirectToAction("Login");
		}

	}

}
