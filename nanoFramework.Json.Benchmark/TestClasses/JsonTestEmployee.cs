using System;
using System.Text;

namespace nanoFramework.Json.Benchmark.TestClasses
{
    class JsonTestEmployee
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public JsonTestCompany CurrentEmployer { get; set; }
        public JsonTestCompany[] FormerEmployers { get; set; }
    }
}
