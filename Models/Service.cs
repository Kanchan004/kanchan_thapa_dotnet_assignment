namespace Appointmentschedular.Models
{
    public class Service
    {
        public int Id { get; set; }
        public required string ServiceName { get; set; }
        public List<Appointment> Appointments { get; set; } = new();
    }

}
