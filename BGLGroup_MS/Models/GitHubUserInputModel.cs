using System.ComponentModel.DataAnnotations;

namespace BGLGroup_MS.Models
{
	public class GitHubUserInputModel
	{
		[Required(ErrorMessage = "Please enter a GitHub username")]
		[Display(Name = "GitHub UserName")]
		[StringLength(39, ErrorMessage = "The {0} username you have entered is too long.")]
		[RegularExpression(@"^[^\s\,]+$", ErrorMessage = "Username cannot contain spaces or commas")]
		public string UserName { get; set; }
	}
}