using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriStream.Data;

public class DatabaseTestController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseTestController> _logger;

    public DatabaseTestController(
        ApplicationDbContext context,
        ILogger<DatabaseTestController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            await _context.Database.OpenConnectionAsync();

            var databaseName = await _context.Database
                .SqlQueryRaw<string>(
                    "SELECT DB_NAME() AS Value")
                .SingleAsync();

            ViewBag.Message =
                "Successfully connected to Azure SQL Database!";

            ViewBag.DatabaseName = databaseName;

            ViewBag.Roles = await _context.Roles.ToListAsync();

            await _context.Database.CloseConnectionAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Azure SQL connection failed");

            ViewBag.Message = "Database connection failed.";

            ViewBag.Error = ex.GetBaseException().Message;
        }

        return View();
    }
}