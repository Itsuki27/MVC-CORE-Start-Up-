using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using MVC_CORE_Start_Up_.Models;

namespace MVC_CORE_Start_Up_.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly MvcCoreContext _context;

        public EmployeesController(MvcCoreContext context)
        {
            _context = context;
        }

        // GET: Employees
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetEmployees()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var employeeData = _context.Employees.AsQueryable();

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                employeeData = employeeData.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            if (!string.IsNullOrEmpty(searchValue))
            {
                employeeData = employeeData.Where(m => m.FirstName.Contains(searchValue)
                                            || m.LastName.Contains(searchValue)
                                            || m.Department.Contains(searchValue));
            }

            recordsTotal = await employeeData.CountAsync();
            var data = await employeeData.Skip(skip).Take(pageSize).ToListAsync();
            var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };

            return Ok(jsonData);
        }


        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,FirstName,LastName,Salary,Department")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                #region Duplicate Check
                // Prevent adding employees with duplicate first names (case insensitive and ignoring spaces)
                bool duplicateFirst = await _context.Employees
                    .AnyAsync(e => e.FirstName.Trim().ToLower() == employee.FirstName.Trim().ToLower());

                if (duplicateFirst)
                {
                    ModelState.AddModelError("FirstName", "An employee with this first name already exists.");
                }

                // Prevent adding employees with duplicate last names (case insensitive and ignoring spaces)
                bool duplicateLast = await _context.Employees
                    .AnyAsync(e => e.LastName.Trim().ToLower() == employee.LastName.Trim().ToLower());

                if (duplicateLast)
                {
                    ModelState.AddModelError("LastName", "An employee with this last name already exists.");
                }

                // Prevent adding employees with duplicate departments (case insensitive and ignoring spaces)
                bool duplicateDept = await _context.Employees
                    .AnyAsync(e => e.Department.Trim().ToLower() == employee.Department.Trim().ToLower());

                if (duplicateDept)
                {
                    ModelState.AddModelError("Department", "An employee with this department already exists.");
                }

                #endregion

                // If there are any validation errors, return the view with the current employee data
                if (!ModelState.IsValid)
                {
                    return View(employee);
                }


                _context.Add(employee);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Employee created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,FirstName,LastName,Salary,Department")] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "Employee updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Employee deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
