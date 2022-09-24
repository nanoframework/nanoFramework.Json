using System;

namespace nanoFramework.Json.Test.Shared
{
    public class JsonTestTown
    {
        public int TownID { get; set; }
        public string TownName { get; set; }
        public JsonTestCompany[] CompaniesInThisTown { get; set; }
        public JsonTestEmployee[] EmployeesInThisTown { get; set; }

        internal static JsonTestTown CreateTestClass()
        {
            return new JsonTestTown()
            {
                TownID = 1,
                TownName = "myTown",
                CompaniesInThisTown = new JsonTestCompany[]
                {
                    new JsonTestCompany { CompanyID = 1, CompanyName = "AAA Amalgamated Industries" },
                    new JsonTestCompany { CompanyID = 2, CompanyName = "BBB Amalgamated Industries" },
                    new JsonTestCompany { CompanyID = 3, CompanyName = "CCC Amalgamated Industries" },
                    new JsonTestCompany { CompanyID = 4, CompanyName = "DDD Amalgamated Industries" },
                    new JsonTestCompany { CompanyID = 5, CompanyName = "EEE Amalgamated Industries" },
                    new JsonTestCompany { CompanyID = 6, CompanyName = "FFF Amalgamated Industries" },
                    new JsonTestCompany { CompanyID = 7, CompanyName = "GGG Amalgamated Industries" },
                    new JsonTestCompany { CompanyID = 8, CompanyName = "HHH Amalgamated Industries" }
                },
                EmployeesInThisTown = new JsonTestEmployee[]
                {
                    new JsonTestEmployee
                    {
                        EmployeeID = 1,
                        EmployeeName = "John Smith",
                        CurrentEmployer = new JsonTestCompany { CompanyID = 3, CompanyName = "CCC Amalgamated Industries" },
                        FormerEmployers = new JsonTestCompany[]
                        {
                            new JsonTestCompany { CompanyID = 2, CompanyName = "BBB Amalgamated Industries" },
                            new JsonTestCompany { CompanyID = 5, CompanyName = "EEE Amalgamated Industries" },
                        }
                    },
                    new JsonTestEmployee
                    {
                        EmployeeID = 1,
                        EmployeeName = "Jim Smith",
                        CurrentEmployer = new JsonTestCompany { CompanyID = 7, CompanyName = "GGG Amalgamated Industries" },
                        FormerEmployers = new JsonTestCompany[]
                        {
                            new JsonTestCompany { CompanyID = 4, CompanyName = "DDD Amalgamated Industries" },
                            new JsonTestCompany { CompanyID = 1, CompanyName = "AAA Amalgamated Industries" },
                            new JsonTestCompany { CompanyID = 6, CompanyName = "FFF Amalgamated Industries" },
                        }
                    }
                }
            };
        }
    }
}
