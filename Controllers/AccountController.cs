using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop_HTH.Models;
using Shop_HTH.Models.ViewModel;

namespace Shop_HTH.Controllers
{
	public class AccountController : Controller
	{
		private UserManager<AppUserModel> _userManage;
		private SignInManager<AppUserModel> _signInManager;
		public AccountController(SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManage)
		{
			this._userManage = userManage;
			this._signInManager = signInManager;
		}
		public IActionResult Login(string returnUrl)
		{
			return View(new LoginViewModel { ReturnUrl = returnUrl});
		}
		public IActionResult Create()
		{
			return View();
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
			await _signInManager.SignOutAsync();
			return Redirect(returnUrl);
		}
	}
}
