using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StoreWebSite.DAL.Interfaces;
using StoreWebSite.DAL.Models;

namespace StoreWebSite.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _db;
        public AdminController(IUnitOfWork db)
        {
            _db = db;
        }

        //gets all log entries from db ordered by their timestamps.
        public IActionResult ShowLog()
        {
            IEnumerable<LogEntry> logs = _db.LogEntryRepository.Get().OrderBy(log => log.TimeStamp);
            return View(logs);
        }
    }
}
