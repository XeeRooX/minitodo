using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace minitodo.Dtos
{
	public class TaskCreate
	{
		[Required]
		public int GroupId { get; set; }
		[Required]
		public string TaskTitle { get; set; }
	}
}
