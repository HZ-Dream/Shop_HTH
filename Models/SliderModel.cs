using Shop_HTH.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_HTH.Models
{
	public class SliderModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Yêu cầu nhập tên thương hiệu")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Yêu cầu nhập Mô tả Thương hiệu")]
		public string Description { get; set; }

		public int? Status { get; set; }
		public string Image { get; set; }
		[NotMapped]
		[FileExtension]
		public IFormFile? ImageUpload { get; set; }
	}
}
