using Appointmentschedular.Data;
using Appointmentschedular.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Appointmentschedular.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context; // Inject the ApplicationDbContext into the controller
        }

        // Index action: Retrieves all appointments and sends them to the view
        public async Task<IActionResult> Index()
        {
           
            var appointments = await _context.Appointments.ToListAsync();
            return View(appointments); // Make sure appointments are passed to the view
        }
        

        // GET: Appointment/Create
        public IActionResult Create()
        {
            // Return the Create view to allow the user to add an appointment
            return View(new Appointment());
        }

        // POST: Appointment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description, FilePath, User, Service, Date, Time")] Appointment appointment, IFormFile file)
        {
            // Check if the file is provided
            if (file != null)
            {
                // Ensure the uploads directory exists
                var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory); // Create the directory if it doesn't exist
                }

                // Generate the file path and save the file to the server
                var filePath = Path.Combine(uploadDirectory, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Set the file path in the appointment model
                appointment.FilePath = filePath;
            }

            // Set default values for the appointment if they are not provided
            if (string.IsNullOrEmpty(appointment.Description)) appointment.Description = "Description here";
            if (string.IsNullOrEmpty(appointment.User)) appointment.User = "UserName";
            if (string.IsNullOrEmpty(appointment.Service)) appointment.Service = "Requested Service";
            if (appointment.Date == default) appointment.Date = DateTime.Now; // Default to current date
            if (appointment.Time == default) appointment.Time = new TimeSpan(10, 0, 0); // Default to 10:00 AM


            // Check if the model is valid before saving to the database
            if (ModelState.IsValid)
            {
                // Add the appointment to the database
                _context.Add(appointment);
                await _context.SaveChangesAsync();

                // Redirect to the Index action to see the newly added appointment
                return RedirectToAction(nameof(Index));
            }

            // Return the Create view with the current appointment data if validation fails
            return View(appointment);
        }
        // GET: Appointment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the appointment using the provided ID
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
            {
                return NotFound();  // Return NotFound if the appointment doesn't exist
            }

            // Return the appointment to the view for confirmation
            return View(appointment);
        }
        // POST: Appointment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find the appointment by its ID
            var appointment = await _context.Appointments.FindAsync(id);

            // If the appointment exists, remove it from the context
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }

            // Redirect the user back to the appointment index page after deletion
            return RedirectToAction(nameof(Index));
        }
        // GET: Appointment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }
        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }

        // POST: Appointment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Description, FilePath, User, Service, Date, Time")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // Redirect back to the appointments list
            }
            return View(appointment);
        }
        


    }
}
