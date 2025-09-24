using System;
using System.Collections.Generic;

namespace MVC_CORE_Start_Up_.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public int? Salary { get; set; }

    public string? Department { get; set; }
}
