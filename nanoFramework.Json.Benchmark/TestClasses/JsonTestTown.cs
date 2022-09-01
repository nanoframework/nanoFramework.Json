using System;
using System.Text;

namespace nanoFramework.Json.Benchmark.TestClasses
{
    class JsonTestTown
    {
        public int TownID { get; set; }
        public string TownName { get; set; }
        public JsonTestCompany[] CompaniesInThisTown { get; set; }
        public JsonTestEmployee[] EmployeesInThisTown { get; set; }
    }
}
