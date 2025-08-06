using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelConstants;

namespace Models
{
    public class Log
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(ValidationConstants.LevelLogMaxLenght)]
        public string Level { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationConstants.MessageLogMaxLenght)]
        public string Message { get; set; } = string.Empty;
    }
}
