using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using AirlinePlanner.Models;

namespace AirlinePlanner.Tests
{
  [TestClass]
  public class CityTest
  {
    public CityTest()
    {
        DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=AirlinePlanner_test;";
    }

    // public Void Dispose()
    // {
    //
    // }
    [TestMethod]
    public void GetAll_GetAllItemsInDatabase_CityList()
    {
      int count = City.GetAll().Count;
      Assert.AreEqual(0,count);
    }
    [TestMethod]
    public void Save_SaveRecordInTheDB_City()
    {
      City city = new City("seattle");
      city.Save();

      List<City> result = City.GetAll();

      List<City> testList = new List<City>{city};

      foreach(var City in result)
      {
        Console.WriteLine("GET ALL LIST: " + City.GetName());
      }
      foreach(var City in testList)
      {
        Console.WriteLine("TEST LIST: " + City.GetName());
      }

      CollectionAssert.AreEqual(testList,result);
    }
  }

}
