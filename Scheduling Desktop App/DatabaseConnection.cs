using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Windows.Forms;

namespace Scheduling_Desktop_App
{
    public static class DatabaseConnection
    {
        public static MySqlConnection MySqlConnection { get; set; }
        public static BindingList<Appointment> Appointments { get; set; } = new BindingList<Appointment>();
        public static BindingList<Customer> Customers { get; set; } = new BindingList<Customer>();
        public static BindingList<Appointment> MonthAppointments { get; set; } = new BindingList<Appointment>();
        public static BindingList<AppointmentTypesCount> AppointmentTypesCounts { get; set; } = new BindingList<AppointmentTypesCount>();
        public static BindingList<AppointmentCustomerCount> AppointmentCustomerCounts { get; set; }= new BindingList<AppointmentCustomerCount>();

        public static void StartConnection()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString;
                MySqlConnection = new MySqlConnection(connectionString);

                MySqlConnection.Open();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void StopConnection()
        {
            try
            {
                MySqlConnection?.Close();
                MySqlConnection = null;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static int AuthenticateUser(string username, string password)
        {
            try
            {
                string query = "SELECT userId FROM user WHERE userName = @Username AND password = @Password";

                using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        int userId = Convert.ToInt32(result);
                        return userId;
                    }
                    return -1;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public static void FetchAppointments(DateTime selectedDate, int viewIndex)
        {
            Appointments.Clear();
            

            try
            {
                using (MySqlCommand cmd = new MySqlCommand("", MySqlConnection))
                {
                    string query = "";

                    if (viewIndex == 2)
                    {
                        // Monthly view
                        query = "SELECT * FROM appointment WHERE DATE(start) >= @StartOfMonth AND DATE(start) <= @EndOfMonth ORDER BY start";

                        DateTime startOfMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
                        DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                        cmd.Parameters.AddWithValue("@StartOfMonth", startOfMonth.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@EndOfMonth", endOfMonth.ToString("yyyy-MM-dd"));
                    }
                    else if (viewIndex == 1)
                    {
                        // Weekly view
                        query = "SELECT * FROM appointment WHERE DATE(start) >= @StartOfWeek AND DATE(start) <= @EndOfWeek ORDER BY start";

                        DateTime startOfWeek = selectedDate.Date.AddDays(DayOfWeek.Sunday - selectedDate.Date.DayOfWeek);
                        DateTime endOfWeek = startOfWeek.AddDays(6);

                        cmd.Parameters.AddWithValue("@StartOfWeek", startOfWeek.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@EndOfWeek", endOfWeek.ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        // Daily view
                        query = "SELECT * FROM appointment WHERE DATE(start) = @SelectedDate ORDER BY start";
                        cmd.Parameters.AddWithValue("@SelectedDate", selectedDate.ToString("yyyy-MM-dd"));
                    }

                    cmd.CommandText = query;

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Appointment appointment = new Appointment
                            {
                                AppointmentId = Convert.ToInt32(reader["appointmentId"]),
                                CustomerId = Convert.ToInt32(reader["customerId"]),
                                UserId = Convert.ToInt32(reader["userId"]),
                                Title = reader["title"].ToString(),
                                Description = reader["description"].ToString(),
                                Location = reader["location"].ToString(),
                                Contact = reader["contact"].ToString(),
                                Type = reader["type"].ToString(),
                                Url = reader["url"].ToString(),
                                Start = Convert.ToDateTime(reader["start"]).ToLocalTime(),
                                End = Convert.ToDateTime(reader["end"]).ToLocalTime(),
                                CreateDate = Convert.ToDateTime(reader["createDate"]),
                                CreatedBy = reader["createdBy"].ToString(),
                                LastUpdate = Convert.ToDateTime(reader["lastUpdate"]),
                                LastUpdateBy = reader["lastUpdateBy"].ToString()
                            };

                            Appointments.Add(appointment);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void AlertIfUpcomingAppointment(DateTime selectedDate, DateTime thresholdTime)
        {
            try
            {
                string query = "SELECT * FROM appointment WHERE DATE(start) = @SelectedDate AND start >= @now AND start <= @ThresholdTime ORDER BY start LIMIT 1";

                using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection))
                {
                    cmd.Parameters.AddWithValue("@SelectedDate", selectedDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@now", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@ThresholdTime", thresholdTime.ToUniversalTime());

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string title = reader["title"].ToString();
                            string start = Convert.ToDateTime(reader["start"]).ToLocalTime().ToString("hh:mm tt");
                            string end = Convert.ToDateTime(reader["end"]).ToLocalTime().ToString("hh:mm tt");
                            MessageBox.Show($"You have an upcoming appointment in the next 15 minutes! \n\nTitle: {title}\nStart: {start}\nEnd: {end}", 
                                "Appointment Reminder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static List<DateTime> GetAvailableTimeSlots(DateTime selectedDate)
        {
            FetchAppointments(selectedDate, 0);
            List<DateTime> AvailableTimeSlots = new List<DateTime>();

            TimeSpan increment = TimeSpan.FromMinutes(15);
            DateTime startTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 14, 0, 0).ToLocalTime(); // 9:00 AM EST
            DateTime endTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 22, 15, 0).ToLocalTime(); // 5:00 PM EST

            for (DateTime time = startTime; time <= endTime; time += increment)
            {
                AvailableTimeSlots.Add(time);
            }

            foreach (Appointment appointment in Appointments)
            {
                for (DateTime time = appointment.Start; time < appointment.End; time += increment)
                {
                    AvailableTimeSlots.Remove(time);
                }
            }

            return AvailableTimeSlots;
        }

        public static List<DateTime> GetAvailableTimeSlots(DateTime selectedDate, Appointment selectedAppointment)
        {
            FetchAppointments(selectedDate, 0);
            List<DateTime> AvailableTimeSlots = new List<DateTime>();

            TimeSpan increment = TimeSpan.FromMinutes(15);
            DateTime startTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 14, 0, 0).ToLocalTime(); // 9:00 AM EST
            DateTime endTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 22, 15, 0).ToLocalTime(); // 5:00 PM EST

            for (DateTime time = startTime; time <= endTime; time += increment)
            {
                AvailableTimeSlots.Add(time);
            }

            foreach (Appointment appointment in Appointments)
            {
                if (selectedAppointment.AppointmentId != appointment.AppointmentId)
                {
                    for (DateTime time = appointment.Start; time < appointment.End; time += increment)
                    {
                        AvailableTimeSlots.Remove(time);
                    }
                }
            }

            return AvailableTimeSlots;
        }

        public static void GetMonthAppointments(int year, string month)
        {
            MonthAppointments.Clear();

            try
            {
                DateTime startOfMonth = new DateTime(year, DateTime.ParseExact(month, "MMMM", CultureInfo.InvariantCulture).Month, 1);
                DateTime endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);

                string query = "SELECT * FROM appointment WHERE start >= @StartOfMonth AND start <= @EndOfMonth ORDER BY start";

                using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection))
                {
                    cmd.Parameters.AddWithValue("@StartOfMonth", startOfMonth.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@EndOfMonth", endOfMonth.ToString("yyyy-MM-dd HH:mm:ss"));

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Appointment appointment = new Appointment
                            {
                                AppointmentId = Convert.ToInt32(reader["appointmentId"]),
                                CustomerId = Convert.ToInt32(reader["customerId"]),
                                UserId = Convert.ToInt32(reader["userId"]),
                                Title = reader["title"].ToString(),
                                Description = reader["description"].ToString(),
                                Location = reader["location"].ToString(),
                                Contact = reader["contact"].ToString(),
                                Type = reader["type"].ToString(),
                                Url = reader["url"].ToString(),
                                Start = Convert.ToDateTime(reader["start"]).ToLocalTime(),
                                End = Convert.ToDateTime(reader["end"]).ToLocalTime(),
                                CreateDate = Convert.ToDateTime(reader["createDate"]),
                                CreatedBy = reader["createdBy"].ToString(),
                                LastUpdate = Convert.ToDateTime(reader["lastUpdate"]),
                                LastUpdateBy = reader["lastUpdateBy"].ToString()
                            };

                            MonthAppointments.Add(appointment);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void GetAppointmentTypes(int year, string month)
        {
            AppointmentTypesCounts.Clear();

            try
            {
                string query = "SELECT type, COUNT(*) AS count FROM appointment " +
                    "WHERE YEAR(start) = @Year AND MONTH(start) = @Month " +
                    "GROUP BY type ORDER BY count DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection))
                {
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@Month", DateTime.ParseExact(month, "MMMM", CultureInfo.InvariantCulture).Month);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AppointmentTypesCount appointmentTypesCount = new AppointmentTypesCount()
                            {
                                Type = reader["type"].ToString(),
                                Count = Convert.ToInt32(reader["count"])
                            };

                            AppointmentTypesCounts.Add(appointmentTypesCount);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void GetAppointmentCustomers(int year, string month)
        {
            AppointmentCustomerCounts.Clear();

            try
            {
                string query = "SELECT c.customerName, COUNT(*) AS count " +
                               "FROM appointment a " +
                               "JOIN customer c ON a.customerId = c.customerId " +
                               "WHERE YEAR(a.start) = @Year AND MONTH(a.start) = @Month " +
                               "GROUP BY c.customerName ORDER BY count DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection))
                {
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@Month", DateTime.ParseExact(month, "MMMM", CultureInfo.InvariantCulture).Month);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AppointmentCustomerCount appointmentCustomerCount = new AppointmentCustomerCount()
                            {
                                CustomerName = reader["customerName"].ToString(),
                                Count = Convert.ToInt32(reader["count"])
                            };

                            AppointmentCustomerCounts.Add(appointmentCustomerCount);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static bool AddAppointment(Appointment appointment)
        {
            try
            {
                // SQL INSERT query to add a new appointment
                string query = "INSERT INTO appointment (customerId, userId, title, description, location, contact, type, url, start, end, createDate, createdBy, lastUpdate, lastUpdateBy) " +
                    "VALUES (@CustomerId, @UserId, @Title, @Description, @Location, @Contact, @Type, @Url, @Start, @End, @CreateDate, @CreatedBy, @LastUpdate, @LastUpdateBy)";

                using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", appointment.CustomerId);
                    cmd.Parameters.AddWithValue("@UserId", appointment.UserId);
                    cmd.Parameters.AddWithValue("@Title", appointment.Title);
                    cmd.Parameters.AddWithValue("@Description", appointment.Description);
                    cmd.Parameters.AddWithValue("@Location", appointment.Location);
                    cmd.Parameters.AddWithValue("@Contact", appointment.Contact);
                    cmd.Parameters.AddWithValue("@Type", appointment.Type);
                    cmd.Parameters.AddWithValue("@Url", appointment.Url);
                    cmd.Parameters.AddWithValue("@Start", appointment.Start);
                    cmd.Parameters.AddWithValue("@End", appointment.End);
                    cmd.Parameters.AddWithValue("@CreateDate", appointment.CreateDate);
                    cmd.Parameters.AddWithValue("@CreatedBy", appointment.CreatedBy);
                    cmd.Parameters.AddWithValue("@LastUpdate", appointment.LastUpdate);
                    cmd.Parameters.AddWithValue("@LastUpdateBy", appointment.LastUpdateBy);

                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool UpdateAppointment(Appointment appointment)
        {
            try
            {
                // SQL UPDATE query to update an existing appointment
                string query = "UPDATE appointment SET customerId = @CustomerId, userId = @UserId, title = @Title, " +
                    "description = @Description, location = @Location, contact = @Contact, type = @Type, url = @Url, " +
                    "start = @Start, end = @End, lastUpdate = @LastUpdate, lastUpdateBy = @LastUpdateBy " + 
                    "WHERE appointmentId = @AppointmentId";

                using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", appointment.CustomerId);
                    cmd.Parameters.AddWithValue("@UserId", appointment.UserId);
                    cmd.Parameters.AddWithValue("@Title", appointment.Title);
                    cmd.Parameters.AddWithValue("@Description", appointment.Description);
                    cmd.Parameters.AddWithValue("@Location", appointment.Location);
                    cmd.Parameters.AddWithValue("@Contact", appointment.Contact);
                    cmd.Parameters.AddWithValue("@Type", appointment.Type);
                    cmd.Parameters.AddWithValue("@Url", appointment.Url);
                    cmd.Parameters.AddWithValue("@Start", appointment.Start);
                    cmd.Parameters.AddWithValue("@End", appointment.End);
                    cmd.Parameters.AddWithValue("@LastUpdate", appointment.LastUpdate);
                    cmd.Parameters.AddWithValue("@LastUpdateBy", appointment.LastUpdateBy);
                    cmd.Parameters.AddWithValue("@AppointmentId", appointment.AppointmentId);

                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool DeleteAppointment(int appointmentId)
        {
            try
            {
                // SQL DELETE query to delete an appointment
                string query = "DELETE FROM appointment WHERE appointmentId = @AppointmentId";

                using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection))
                {
                    cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static void FetchCustomers()
        {
            Customers.Clear(); // Clear existing customers before fetching new ones

            try
            {
                string query = "SELECT * FROM customer";

                using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Customer customer = new Customer
                            {
                                // Map database columns to Customer properties
                                CustomerId = Convert.ToInt32(reader["customerId"]),
                                CustomerName = reader["customerName"].ToString(),
                                AddressId = Convert.ToInt32(reader["addressId"]),
                                Active = Convert.ToBoolean(reader["active"]),
                                CreateDate = Convert.ToDateTime(reader["createDate"]),
                                CreatedBy = reader["createdBy"].ToString(),
                                LastUpdate = Convert.ToDateTime(reader["lastUpdate"]),
                                LastUpdateBy = reader["lastUpdateBy"].ToString()
                            };

                            Customers.Add(customer);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static bool AddCustomer(Customer customer)
        {
            try
            {
                string query = "INSERT INTO customer (customerName, addressId, active, createDate, createdBy, lastUpdate, lastUpdateBy) " +
                       "VALUES (@CustomerName, @AddressId, @Active, @CreateDate, @CreatedBy, @LastUpdate, @LastUpdateBy)";

                using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection))
                {
                    cmd.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                    cmd.Parameters.AddWithValue("@AddressId", customer.AddressId);
                    cmd.Parameters.AddWithValue("@Active", customer.Active);
                    cmd.Parameters.AddWithValue("@CreateDate", customer.CreateDate);
                    cmd.Parameters.AddWithValue("@CreatedBy", customer.CreatedBy);
                    cmd.Parameters.AddWithValue("@LastUpdate", customer.LastUpdate);
                    cmd.Parameters.AddWithValue("@LastUpdateBy", customer.LastUpdateBy); // Set lastUpdateBy here

                    cmd.ExecuteNonQuery();
                }

                // Refresh the customer list after adding
                FetchCustomers();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;                                                                                                                                                       
            }
        }

        public static bool UpdateCustomer(Customer customer)
        {
            try
            {
                string query = "UPDATE customer " +
                               "SET customerName = @CustomerName, addressId = @AddressId, active = @Active, " +
                               "lastUpdate = @LastUpdate, lastUpdateBy = @LastUpdateBy " +
                               "WHERE customerId = @CustomerId";

                using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection))
                {
                    cmd.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                    cmd.Parameters.AddWithValue("@AddressId", customer.AddressId);
                    cmd.Parameters.AddWithValue("@Active", customer.Active);
                    cmd.Parameters.AddWithValue("@LastUpdate", customer.LastUpdate);
                    cmd.Parameters.AddWithValue("@LastUpdateBy", customer.LastUpdateBy);
                    cmd.Parameters.AddWithValue("@CustomerId", customer.CustomerId);

                    cmd.ExecuteNonQuery();
                }

                // Refresh the customer list after updating
                FetchCustomers();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static void DeleteCustomer(int customerId)
        {
            try
            {
                string query = "DELETE FROM customer WHERE customerId = @CustomerId";

                using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);

                    cmd.ExecuteNonQuery();
                }

                // Refresh the customer list after deleting
                FetchCustomers();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        public static List<Country> FetchCountries()
        {
            List<Country> countryList = new List<Country>();

            try
            {
                string query = "SELECT * FROM country";

                using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Country country = new Country
                            {
                                // Map database columns to country properties
                                CountryId = Convert.ToInt32(reader["countryId"]),
                                CountryName = reader["country"].ToString(),
                                CreateDate = Convert.ToDateTime(reader["createDate"]),
                                CreatedBy = reader["createdBy"].ToString(),
                                LastUpdate = Convert.ToDateTime(reader["lastUpdate"]),
                                LastUpdateBy = reader["lastUpdateBy"].ToString()
                            };

                            countryList.Add(country);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return countryList;
        }

        public static int AddCountry(Country country)
        {
            try
            {
                string query = "INSERT INTO country (country, createDate, createdBy, lastUpdateBy) " +
                       "VALUES (@CountryName, @CreateDate, @CreatedBy, @LastUpdateBy)";

                using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection))
                {
                    cmd.Parameters.AddWithValue("@CountryName", country.CountryName);
                    cmd.Parameters.AddWithValue("@CreateDate", country.CreateDate);
                    cmd.Parameters.AddWithValue("@CreatedBy", country.CreatedBy);
                    cmd.Parameters.AddWithValue("@LastUpdateBy", country.LastUpdateBy); // Set lastUpdateBy here

                    cmd.ExecuteNonQuery();
                }

                // Retrieve the last inserted ID using LAST_INSERT_ID()
                string lastIdQuery = "SELECT LAST_INSERT_ID()";
                using (MySqlCommand cmd = new MySqlCommand(lastIdQuery, MySqlConnection))
                {
                    int lastInsertedId = Convert.ToInt32(cmd.ExecuteScalar());
                    return lastInsertedId;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public static List<City> FetchCities()
        {
            List<City> cityList = new List<City>();

            try
            {
                string query = "SELECT * FROM city";

                using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            City city = new City 
                            {
                                // Map database columns to country properties
                                CityId = Convert.ToInt32(reader["cityId"]),
                                CityName = reader["city"].ToString(),
                                CountryId = Convert.ToInt32(reader["countryId"]),
                                CreateDate = Convert.ToDateTime(reader["createDate"]),
                                CreatedBy = reader["createdBy"].ToString(),
                                LastUpdate = Convert.ToDateTime(reader["lastUpdate"]),
                                LastUpdateBy = reader["lastUpdateBy"].ToString()
                            };

                            cityList.Add(city);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return cityList;
        }

        public static int AddCity(City city)
        {
            try
            {
                string query = "INSERT INTO city (city, countryId, createDate, createdBy, lastUpdateBy) " +
                       "VALUES (@CityName, @CountryId, @CreateDate, @CreatedBy, @LastUpdateBy)";

                using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection))
                {
                    cmd.Parameters.AddWithValue("@CityName", city.CityName);
                    cmd.Parameters.AddWithValue("@CountryId", city.CountryId);
                    cmd.Parameters.AddWithValue("@CreateDate", city.CreateDate);
                    cmd.Parameters.AddWithValue("@CreatedBy", city.CreatedBy);
                    cmd.Parameters.AddWithValue("@LastUpdateBy", city.LastUpdateBy); // Set lastUpdateBy here

                    cmd.ExecuteNonQuery();
                }

                // Retrieve the last inserted ID using LAST_INSERT_ID()
                string lastIdQuery = "SELECT LAST_INSERT_ID()";
                using (MySqlCommand cmd = new MySqlCommand(lastIdQuery, MySqlConnection))
                {
                    int lastInsertedId = Convert.ToInt32(cmd.ExecuteScalar());
                    return lastInsertedId;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public static Address FetchAddress(int addressId)
        {
            Address address = null;

            try
            {
                string query = "SELECT * FROM address WHERE addressId = @addressId";

                using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection))
                {
                    cmd.Parameters.AddWithValue("@addressId", addressId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            address = new Address
                            {
                                // Map database columns to address properties
                                AddressId = Convert.ToInt32(reader["addressId"]),
                                AddressText = reader["address"].ToString(),
                                Address2 = reader["address2"].ToString(),
                                CityId = Convert.ToInt32(reader["cityId"]),
                                PostalCode = reader["postalCode"].ToString(),
                                Phone = reader["phone"].ToString(),
                                CreateDate = Convert.ToDateTime(reader["createDate"]),
                                CreatedBy = reader["createdBy"].ToString(),
                                LastUpdate = Convert.ToDateTime(reader["lastUpdate"]),
                                LastUpdateBy = reader["lastUpdateBy"].ToString()
                            };
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return address;
        }

        public static int AddAddress(Address address)
        {
            try
            {
                string query = "INSERT INTO address (address, address2, cityId, postalCode, phone, createDate, createdBy, lastUpdateBy) " +
                       "VALUES (@AddressText, @Address2, @CityId, @PostalCode, @Phone, @CreateDate, @CreatedBy, @LastUpdateBy)";

                using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection))
                {
                    cmd.Parameters.AddWithValue("@AddressText", address.AddressText);
                    cmd.Parameters.AddWithValue("@Address2", address.Address2);
                    cmd.Parameters.AddWithValue("@CityId", address.CityId);
                    cmd.Parameters.AddWithValue("@PostalCode", address.PostalCode);
                    cmd.Parameters.AddWithValue("@Phone", address.Phone);
                    cmd.Parameters.AddWithValue("@CreateDate", address.CreateDate);
                    cmd.Parameters.AddWithValue("@CreatedBy", address.CreatedBy);
                    cmd.Parameters.AddWithValue("@LastUpdateBy", address.LastUpdateBy); // Set lastUpdateBy here

                    cmd.ExecuteNonQuery();
                }

                // Retrieve the last inserted ID using LAST_INSERT_ID()
                string lastIdQuery = "SELECT LAST_INSERT_ID()";
                using (MySqlCommand cmd = new MySqlCommand(lastIdQuery, MySqlConnection))
                {
                    int lastInsertedId = Convert.ToInt32(cmd.ExecuteScalar());
                    return lastInsertedId;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
