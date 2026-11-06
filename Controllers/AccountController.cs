
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace AgriStream.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<string> _hasher;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
            _hasher = new PasswordHasher<string>();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter your username and password.";
                return View();
            }

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "DefaultConnection is missing from configuration.");
            }

            try
            {
                using var connection =
                    new SqlConnection(connectionString);

                await connection.OpenAsync();

                string sql = @"
                    SELECT
                        u.Username,
                        u.PasswordHash,
                        r.RoleName
                    FROM Users u
                    INNER JOIN Roles r
                        ON u.RoleID = r.RoleID
                    WHERE u.Username = @Username";

                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@Username", username);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    ViewBag.Error = "Invalid username or password.";
                    return View();
                }

                string storedHash = reader.GetString(1);
                string role = reader.GetString(2);
                string actualUsername = reader.GetString(0);

                var verification = _hasher.VerifyHashedPassword(
                    actualUsername,
                    storedHash,
                    password);

                if (verification == PasswordVerificationResult.Failed)
                {
                    ViewBag.Error = "Invalid username or password.";
                    return View();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, actualUsername),
                    new Claim(ClaimTypes.Role, role)
                };

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                return RedirectToAction("Index", "Home");
            }
            catch (SqlException)
            {
                ViewBag.Error =
                    "Unable to connect to the user database. Please try again later.";

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
