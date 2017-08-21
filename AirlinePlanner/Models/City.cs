using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace AirlinePlanner.Models
{
  public class City
  {
    private int _id;
    private string _name;

    public City(string name, int id = 0)
    {
      _id = id;
      _name = name;
    }

    public string GetName()
    {
      return _name;
    }
    public int GetId()
    {
      return _id;
    }
    public override bool Equals(System.Object otherCity)
    {
      if(!(otherCity is City))
      {
        return false;
      }
      else
      {
        City newCity = (City) otherCity;
        return this.GetId().Equals(newCity.GetId());
      }
    }
    public override int GetHashCode()
    {
      return this.GetId().GetHashCode();
    }
    public static List<City> GetAll()
    {
      List<City> allCities = new List<City>();
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM cities";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        var city = new City(name,id);
        allCities.Add(city);
      }
      conn.Close();
      return allCities;
    }
    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText= @"INSERT INTO cities(name) VALUES(@name);";

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@name";
      name.Value = this._name;
      cmd.Parameters.Add(name);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
    }
    public static City Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM cities WHERE id =@searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int cityID = 0;
      string cityName = "";

      while(rdr.Read())
      {
        cityId = rdr.GetIt32(0);
        cityName = rdr.GetString(1);
      }
      City newCity= new City(cityName, cityId);
      return newCity;
    }
    //We are not making this methid static and not passing any parameters becoz //when we call this method on an instance of a class, thatinstance will //already have its id, so we dont need to pass it.
    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM cities WHERE id=@thisId; DELETE FROM cities_flights WHERE city_id =@thisId";

      MySqlParameter thisId = new MySqlParameter();
      thisId.ParameterName = "@thisId";
      thisId.Value = this.GetId();
      cmd.Parameters.Add(thisId);

      cmd.ExecuteNonQuery();
      if (conn != null)
      {
        conn.Close();
      }
    }
    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM cities;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
    public List<Flight> GetFlights()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT flight_id FROM cities_flights WHERE city_id = @CityId;";

      MySqlParameter CityId = new MySqlParameter();
      CityId.ParameterName = "@CityId";
      CityId.Value = _id;
      cmd.Parameters.Add(CityId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      List<int> flightIds = new List<int> ();

      while(rdr.Read())
      {
        int flightId = rdr.GetInt32(0);
        FlightIds.Add(flightId);
      }
      rdr.Dispose();

      List<Flight> flights = new List<Flight>();

      foreach(int flightId in flightIds)
      {
        var cmd2 = conn.CreateCommand() as MySqlCommand;
        cmd2.CommandText = @"SELECT * FROM flights WHERE id=@FlightId;";

        MySqlParameter FlightIdParameter = new MySqlParameter();
        FlightIdParameter.ParameterName = "@FlightId";
        FlightIdParameter.Value = flightId;
        cmd2.Parameters.Add(FlightIdParameter);

        var rdr2 = cmd2.ExecuteReader() as MySqlDataReader;
        while(rdr2.Read())
        {
          int flightId = rdr2.GetInt32(0);
          DateTime departureTime = rdr2.GetDateTime(1);
          string departureCity = rdr2.GetString(2);
          string arrivalCity = rdr2.GetString(3);
          string status = rdr2.GetString(4);
          Flight newFlight = Flight(departureTime, departureCity, arrivalCity,status,flightId);
          flights.Add(newFlight);
        }
        rdr2.Dispose();
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return tasks;
    }
    public void AddFlight(Flight newFlight)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText() = @"INSERT INTO cities_flights (city_id, flight_id) VALUES(@cityId, @flightId);";

      MySqlParameter cityId = new MySqlParameter();
      cityId.ParameterName = "@cityId";
      cityId.Value = _id;
      cmd.Parameters.Add(cityId);

      MySqlParameter flightId = new MySqlParameter();
      flightId.ParameterName = "@flightId";
      flightId.Value = newFlight.GetId();
      cmd.Parameters.Add(flightId);

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }
    }
  }
}
