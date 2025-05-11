using System;
namespace Cutify.Models
{
	public class ExceptionLog
	{
        public int Id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Source { get; set; }
    }
}

