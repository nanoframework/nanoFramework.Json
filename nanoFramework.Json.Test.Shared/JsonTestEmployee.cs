using System;

namespace nanoFramework.Json.Test.Shared
{
    public class JsonTestEmployee
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public JsonTestCompany CurrentEmployer { get; set; }
        public JsonTestCompany[] FormerEmployers { get; set; }
    }
}
