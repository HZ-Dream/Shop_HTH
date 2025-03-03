﻿using System.ComponentModel.DataAnnotations;

namespace Shop_HTH.Models.ViewModel
{
	public class LoginViewModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Làm ơn nhập Username")]
		public string Username { get; set; }
		[DataType(DataType.Password), Required(ErrorMessage = "Làm ơn nhập Password")]
		public string Password { get; set; }
		public string ReturnUrl { get; set; }
	}
}
