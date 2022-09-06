using System;

namespace nanoFramework.Json.Test.Shared
{
    public class JsonTestTown
    {
        public int TownID { get; set; }
        public string TownName { get; set; }
        public JsonTestCompany[] CompaniesInThisTown { get; set; }
        public JsonTestEmployee[] EmployeesInThisTown { get; set; }
    }
}
