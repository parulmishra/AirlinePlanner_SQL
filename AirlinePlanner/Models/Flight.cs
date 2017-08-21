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

    public override bool Equals(System.Object otherFlight)
    {
      if(!(otherFlight is Flight))
      {
        return false;
      }
      else
      {
        Flight newFlight = (Flight) otherFlight;
        return this.GetId().Equals(newFlight.GetId());
      }
    }
    public override int GetHashCode()
    {
      return this.GetId().GetHashCode();
    }
    public int GetId()
    {
      return _id;
    }
    public DateTime GetDepartureTime()
    {
      return _departureTime;
    }
    public string GetDepartureCity()
    {
      return _departureCity;
    }
    public string GetArrivalCity()
    {
      return _arrivalCity;
    }
    public string GetStatus()
    {
      return _status;
    }
    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO flights(departure_time,departure_city,arrival_city,status) VALUES(,@departure_time,@departure_city,@arrival_city,@status);";

      MySqlParameter departure_time = new MySqlParameter();
      departure_time.ParameterName = "@departure_time";
      departure_time.Value = this._departureTime;
      cmd.Parameters.Add(departure_time);

      MySqlParameter departure_city= new MySqlParameter();
      departure_city.ParameterName = "@departure_city";
      departure_city.Value = this._departureCity;
      cmd.Parameters.Add(departure_city);

      MySqlParameter arrival_city = new MySqlParameter();
      arrival_city.ParameterName = "@arrival_city";
      arrival_city.Value = this._arrivalCity;
      cmd.Parameters.Add(arrival_city);

      MySqlParameter status = new MySqlParameter();
      status.ParameterName = "@status";
      status.Value = this._status;
      cmd.Parameters.Add(status);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
    public static List<Flight> GatAll()
    {
      List<Flight> allFlights = new List<Flight>();

      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      if (sortType == "departure_time")
      {
        cmd.CommandText = @"SELECT * FROM flights ORDER BY departure_time ASC;";
      }
      else if(sortType == "departure_city")
      {
        cmd.CommandText = @"SELECT * FROM flights ORDER BY departure_city ASC;";
      }
      else if(sortType == "arrival_city")
      {
        cmd.CommandText = @"SELECT * FROM flights ORDER BY arrival_city ASC;";
      }
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int flightId = rdr.GetInt32(0);
        DateTime departureTime = rdr.GetDateTime(1);
        string DepartureCity = rdr.GetString(2);
        string arrivalCity = rdr.GetString(3);
        string status = rdr.GetString(4);
        Flight newFlight = Flight(departureTime,departureCity,arrivalCity,status,flightId);
        allFlights.Add(newFlight);
      }
      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }
      return allFlights;
    }
    public static Flight Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM flights WHERE id = (@searchId);";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int flightId = 0;
      DateTime departureTime = DateTime.Now;
      string departureCity = "";
      string arrivalCity = "";

      while(rdr.Read())
      {
        flightId = rdr.GetInt32(0);
        departureTime = rdr.GetDateTime(1);
        departureCity = rdr.GetString(2);
        arrivalCity = rdr.GetString(3);
        status = rdr.GetString(4);
      }
      Flight newFlight = Flight(departureTime,departureCity,arrivalCity,status,flightId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newFlight;
    }

    public void Update(DateTime newDepartureTime,string newDepartureCity,string newArrivalCity,string newStatus)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE tasks SET departure_time = @newDepartureTime, departure_city = @newDepartureCity, arrival_city = @newArrivalCity status = @newStatus WHERE id = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = _id;
      cmd.Parameters.Add(searchId);

      MySqlParameter departureTime = new MySqlParameter();
      departureTime.ParameterName = "@newDepartureTime";
      departureTime.Value = newDepartureTime;
      cmd.Parameters.Add(departureTime);

      MySqlParameter arrivalCity = new MySqlParameter();
      arrivalCity.ParameterName = "@newArrivalCity";
      arrivalCity.Value = newArrivalCity;
      cmd.Parameters.Add(arrivalCity);

      MySqlParameter departureCity = new MySqlParameter();
      departureCity.ParameterName = "@newDepartureCity";
      departureCity.Value = newDepartureCity;
      cmd.Parameters.Add(departureCity);

      MySqlParameter status = new MySqlParameter();
      status.ParameterName = "@newStatus";
      status.Value = newStatus;
      cmd.Parameters.Add(status);

      cmd.ExecuteNonQuery();
      _departureTime = newDepartureTime;
      _departureCity = newDepartureCity;
      _arrivalCity = newArrivalCity;
      _status = status;
      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }
    }
    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = new MySqlCommand("DELETE FROM flights WHERE id = @FlightId; DELETE FROM cities_flights WHERE flight_id = @FlightId;");

      MySqlParameter flightIdParameter = new MySqlParameter();
      flightIdParameter.ParameterName = "@FlightId";
      flightIdParameter.Value = this.GetId();

      cmd.Parameters.Add(flightIdParameter);
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
      cmd.CommandText = @"DELETE FROM flights;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }
    }
    public void AddCategory(Category newCity)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO cities_flights (city_id, flight_id) VALUES (@CityId, @FlightId);";

      MySqlParameter city_id = new MySqlParameter();
      city_id.ParameterName = "@CityId";
      city_id.Value = newCity.GetId();
      cmd.Parameters.Add(city_id);

      MySqlParameter flight_id = new MySqlParameter();
      flight_id.ParameterName = "@FlightId";
      flight_id.Value = _id;
      cmd.Parameters.Add(flight_id);

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }
    }
        public List<Category> GetCities()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT flight_id FROM categories_tasks WHERE task_id = @taskId;";

            MySqlParameter taskIdParameter = new MySqlParameter();
            taskIdParameter.ParameterName = "@taskId";
            taskIdParameter.Value = _id;
            cmd.Parameters.Add(taskIdParameter);

            var rdr = cmd.ExecuteReader() as MySqlDataReader;

            List<int> categoryIds = new List<int> {};
            while(rdr.Read())
            {
                int categoryId = rdr.GetInt32(0);
                categoryIds.Add(categoryId);
            }
            rdr.Dispose();

            List<Category> categories = new List<Category> {};
            foreach (int categoryId in categoryIds)
            {
                var categoryQuery = conn.CreateCommand() as MySqlCommand;
                categoryQuery.CommandText = @"SELECT * FROM categories WHERE id = @CategoryId;";

                MySqlParameter categoryIdParameter = new MySqlParameter();
                categoryIdParameter.ParameterName = "@CategoryId";
                categoryIdParameter.Value = categoryId;
                categoryQuery.Parameters.Add(categoryIdParameter);

                var categoryQueryRdr = categoryQuery.ExecuteReader() as MySqlDataReader;
                while(categoryQueryRdr.Read())
                {
                    int thisCategoryId = categoryQueryRdr.GetInt32(0);
                    string categoryName = categoryQueryRdr.GetString(1);
                    Category foundCategory = new Category(categoryName, thisCategoryId);
                    categories.Add(foundCategory);
                }
                categoryQueryRdr.Dispose();
            }
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
            return categories;
        }

  }
}
