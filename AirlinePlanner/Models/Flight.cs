using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace AirlinePlanner.Models

{
  public class Flight
  {
    private int _id;
    private DateTime _departureTime;
    private string _departureCity;
    private string _arrivalCity;
    private string _status;

    public Flight(DateTime departureTime, string departureCity, string arrivalCity, string status, int id = 0)
    {
      _id = id;
      _departureTime = departureTime;
      _departureCity  = departureCity;
      _arrivalCity = arrivalCity;
      _status = status;
    }
    
  }
}
