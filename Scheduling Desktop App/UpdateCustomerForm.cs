using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Scheduling_Desktop_App
{
    public partial class UpdateCustomerForm : Form
    {
        List<Country> CountryList;
        List<City> CityList;
        Address address;
        Customer customer;

        public UpdateCustomerForm(Customer customer)
        {
            InitializeComponent();

            this.customer = customer;
        }

        private bool IsFieldEmpty(string fieldValue, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldValue))
            {
                MessageBox.Show($"{fieldName} is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            return false;
        }

        private void LoadCountryList()
        {
            // Fetch countries from the database
            CountryList = DatabaseConnection.FetchCountries();

            // Populate the country combo box
            comboBoxCountry.DisplayMember = "CountryName";
            comboBoxCountry.ValueMember = "CountryId";
            comboBoxCountry.DataSource = CountryList;
        }

        private void LoadCityList()
        {
            // Fetch cities from the database
            CityList = DatabaseConnection.FetchCities();

            // Populate the country combo box
            comboBoxCity.DisplayMember = "CityName";
            comboBoxCity.ValueMember = "CityId";
            comboBoxCity.DataSource = CityList;
        }

        private void PopulateControls(Customer customer)
        {
            address = DatabaseConnection.FetchAddress(customer.AddressId);
            City city = CityList.Find(c => c.CityId == address.CityId);
            Country country = CountryList.Find(c => c.CountryId == city.CountryId);

            textBoxFullName.Text = customer.CustomerName;
            textBoxAddress.Text = address.AddressText;
            textBoxAddress2.Text = address.Address2;
            comboBoxCity.SelectedIndex = CityList.IndexOf(city);
            comboBoxCountry.SelectedIndex = CountryList.IndexOf(country);
            textBoxCode.Text = address.PostalCode;
            textBoxPhone.Text = address.Phone;
            checkBoxActive.Checked = customer.Active;
        }

        private void UpdateCustomerForm_Load(object sender, EventArgs e)
        {
            LoadCountryList();
            LoadCityList();
            PopulateControls(customer);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Retrieve input values from the form's controls
            string fullName = textBoxFullName.Text;
            string addressText = textBoxAddress.Text;
            string address2 = textBoxAddress2.Text;
            string cityName = comboBoxCity.Text;
            string countryName = comboBoxCountry.Text;
            string postalCode = textBoxCode.Text;
            string phone = textBoxPhone.Text;
            bool active = checkBoxActive.Checked;

            // Validate that fields are trimmed and non-empty
            if (IsFieldEmpty(fullName, "Full Name") || IsFieldEmpty(addressText, "Address") || IsFieldEmpty(cityName, "City") ||
                IsFieldEmpty(countryName, "Country") || IsFieldEmpty(postalCode, "Postal Code") || IsFieldEmpty(phone, "Phone"))
            {
                return;
            }

            // Validate that the phone number field allows only digits and dashes
            if (!Regex.IsMatch(phone, @"^[0-9-]+$"))
            {
                MessageBox.Show("Please enter a valid phone number (digits and dashes only).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Country country = CountryList.FirstOrDefault(c => c.CountryName.Equals(countryName, StringComparison.OrdinalIgnoreCase));
            int countryId = country?.CountryId ?? DatabaseConnection.AddCountry(new Country
            {
                CountryName = countryName,
                CreateDate = DateTime.UtcNow,
                CreatedBy = Program.CurrentUser,
                LastUpdate = DateTime.UtcNow,
                LastUpdateBy = Program.CurrentUser
            });

            City city = CityList.FirstOrDefault(c => c.CityName.Equals(cityName, StringComparison.OrdinalIgnoreCase));
            int cityId = city?.CityId ?? DatabaseConnection.AddCity(new City
            {
                CityName = cityName,
                CreateDate = DateTime.UtcNow,
                CountryId = countryId,
                CreatedBy = Program.CurrentUser,
                LastUpdate = DateTime.UtcNow,
                LastUpdateBy = Program.CurrentUser
            });

            int addressId;
            // check if the address has changed
            if (address.AddressText.Equals(addressText, StringComparison.OrdinalIgnoreCase) &&
                address.Address2.Equals(address2, StringComparison.OrdinalIgnoreCase) &&
                address.CityId == cityId &&
                address.PostalCode.Equals(postalCode, StringComparison.OrdinalIgnoreCase) &&
                address.Phone.Equals(phone, StringComparison.OrdinalIgnoreCase))
            {
                addressId = address.AddressId;
            }
            else
            {
                // add updated address to databse if a change was made and get the new addressId
                addressId = DatabaseConnection.AddAddress(new Address
                {
                    AddressText = addressText,
                    Address2 = address2,
                    CityId = cityId,
                    PostalCode = postalCode,
                    Phone = phone,
                    CreateDate = DateTime.UtcNow,
                    CreatedBy = Program.CurrentUser,
                    LastUpdate = DateTime.UtcNow,
                    LastUpdateBy = Program.CurrentUser
                });
            }

            // check if any change was made to the customer
            if (customer.CustomerName.Equals(fullName, StringComparison.OrdinalIgnoreCase) &&
                customer.AddressId == address.AddressId &&
                customer.Active == active)
            {
                MessageBox.Show("No changes detected.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // add updated customer to database if a change was made
                bool isSuccess = DatabaseConnection.UpdateCustomer(new Customer
                {
                    CustomerId = customer.CustomerId,
                    CustomerName = fullName,
                    AddressId = addressId,
                    Active = active,
                    LastUpdate = DateTime.UtcNow,
                    LastUpdateBy = Program.CurrentUser
                });

                if (isSuccess)
                {
                    MessageBox.Show("Customer updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Close the form
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
