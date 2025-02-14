using System;
using System.ComponentModel.DataAnnotations;

namespace Appointmentschedular.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; } = "Default Description";

        [Required]
        public string FilePath { get; set; } = "Default Path";

        [Required]
        public string User { get; set; } = "Default User";

        [Required]
        public string Service { get; set; } = "Default Service";

        public DateTime Date { get; set; } = DateTime.Now;  // Default value
        public TimeSpan Time { get; set; } = new TimeSpan(10, 0, 0);  // Default time (10:00 AM)

        // Default Constructor
        public Appointment()
        {
            Description = "Default Description";
            FilePath = "Default Path";
            User = "Default User";
            Service = "Default Service";
        }
    }
}
