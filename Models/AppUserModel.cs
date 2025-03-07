using Microsoft.AspNetCore.Identity;

namespace Shop_HTH.Models
{
	public class AppUserModel : IdentityUser
	{
		public string Occupation {  get; set; }
		public string RoleId { get; set; }
    }
}
