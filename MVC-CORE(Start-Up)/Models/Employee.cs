using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVC_CORE_Start_Up_.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    [Display(Name = "First Name")]
    [Required]
    public string FirstName { get; set; } = null!;

    [Display(Name = "Last Name")]
    [Required]
    public string? LastName { get; set; }

    [Display(Name = "Salary")]
    [Required]
    public int? Salary { get; set; }

    [Display(Name = "Department")]
    [Required]
    public string? Department { get; set; }
}
