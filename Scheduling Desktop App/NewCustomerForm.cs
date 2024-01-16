using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Scheduling_Desktop_App
{
    public partial class NewCustomerForm : Form
    {
        List<Country> CountryList;
        List<City> CityList;
        public NewCustomerForm()
        {
            InitializeComponent();
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

        private void ClearFormControls()
        {
            // Reset values of input controls to their default state
            textBoxFullName.Text = "";
            textBoxAddress.Text = "";
            textBoxAddress2.Text = "";
            comboBoxCity.SelectedIndex = -1; // Set to -1 for no selection
            comboBoxCountry.SelectedIndex = -1; // Set to -1 for no selection
            textBoxCode.Text = "";
            textBoxPhone.Text = "";
            checkBoxActive.Checked = false;
        }

        private void NewCustomerForm_Load(object sender, EventArgs e)
        {
            LoadCountryList();
            LoadCityList();
            ClearFormControls();
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

            int addressId = DatabaseConnection.AddAddress(new Address
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

            // add customer to database
            bool isSuccess = DatabaseConnection.AddCustomer(new Customer
            {
                CustomerName = fullName,
                AddressId = addressId,
                Active = active,
                CreateDate = DateTime.UtcNow,
                CreatedBy = Program.CurrentUser,
                LastUpdate = DateTime.UtcNow,
                LastUpdateBy = Program.CurrentUser
            });

            if (isSuccess)
            {
                DialogResult result = MessageBox.Show("New customer added successfully.\n" +
                        "Do you want to add another customer?", "Success", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    LoadCountryList();
                    LoadCityList();
                    ClearFormControls();
                }
                else
                {
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
