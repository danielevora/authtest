using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace coreclassic.Controllers
{
    public class HomeController : Controller
    {
        protected string _connectionString;

        public HomeController()
        {
            _connectionString = @"data source=sqldev\webdb;Integrated Security=true";
        }

        public ActionResult Index()
        {
            var authString = this.Request.Headers["Authorization"].ToString();
            var hasHttpAuth = !string.IsNullOrEmpty(authString);
            var isKerberos = hasHttpAuth &&
                             String.Compare(authString.Substring(10, 1), "Y",
                                 StringComparison.OrdinalIgnoreCase) == 0;

            var iisName = this.User.Identity.Name;//?? WindowsIdentity.GetCurrent().Name;
            var iisAuthType = this.User.Identity.AuthenticationType;//?? WindowsIdentity.GetCurrent().AuthenticationType;
            var sqlName = execute_scalar("SELECT SYSTEM_USER");
            var sqlAuthType = execute_scalar("select auth_scheme from sys.dm_exec_connections where session_id=@@spid");

            ViewData["IIS"] =
                $"IIS says you are <b>{iisName}</b> using <b>{iisAuthType}</b> (Kerberos? {(isKerberos ? "Yes" : "No")})";
            ViewData["SQL"] =
                $"SQL says you are {sqlName} using {sqlAuthType}";

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [NonAction]
        public string execute_scalar(string query)
        {
            try
            {
                var sqlConnection = new SqlConnection(_connectionString);
                sqlConnection.Open();
                var command = new SqlCommand(query, sqlConnection);
                return (string)command.ExecuteScalar();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
