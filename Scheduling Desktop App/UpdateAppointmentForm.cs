using System.Collections.Generic;
using System;
using System.Windows.Forms;
using System.Globalization;
using System.Linq;

namespace Scheduling_Desktop_App
{
    public partial class UpdateAppointmentForm : Form
    {
        Appointment appointment;
        bool formLoaded = false;
        public UpdateAppointmentForm(Appointment appointment)
        {
            InitializeComponent();

            this.appointment = appointment;

            // load Customer comboBox
            comboBoxCustomer.DataSource = DatabaseConnection.Customers;
            comboBoxCustomer.DisplayMember = "CustomerName";
            comboBoxCustomer.ValueMember = "CustomerId";
        }

        private void PopulateControls(Appointment appointment)
        {
            Customer customer = DatabaseConnection.Customers.FirstOrDefault(c => c.CustomerId == appointment.CustomerId);

            textBoxAppointmentId.Text = appointment.AppointmentId.ToString();
            textBoxUserId.Text = appointment.UserId.ToString();
            dateTimePickerDate.Value = appointment.Start;
            textBoxTitle.Text = appointment.Title;
            textBoxDescriptiom.Text = appointment.Description;
            textBoxType.Text = appointment.Type;
            comboBoxCustomer.SelectedIndex = DatabaseConnection.Customers.IndexOf(customer);
            textBoxContact.Text = appointment.Contact;
            textBoxUrl.Text = appointment.Url;
            textBoxLocation.Text = appointment.Location;

            LoadStartTimes();
        }

        private void LoadStartTimes()
        {
            List<DateTime> timeSlots = DatabaseConnection.GetAvailableTimeSlots(dateTimePickerDate.Value, appointment);

            comboBoxStartTime.Items.Clear();
            for (var i = 0; i < timeSlots.Count - 2; i++)
            {
                comboBoxStartTime.Items.Add(timeSlots[i].ToString("hh:mm tt"));
            }

            int startTimeIndex = timeSlots.IndexOf(appointment.Start);
            if (startTimeIndex != -1 && !formLoaded)
            {
                comboBoxStartTime.SelectedIndex = startTimeIndex;
            }
        }

        private void UpdateAppointmentForm_Load(object sender, System.EventArgs e)
        {
            PopulateControls(appointment);
            formLoaded = true;
        }

        private void comboBoxStartTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<DateTime> timeSlots = DatabaseConnection.GetAvailableTimeSlots(dateTimePickerDate.Value, appointment);

            string startTimeString = comboBoxStartTime.SelectedItem.ToString();
            TimeSpan increment = TimeSpan.FromMinutes(15);
            DateTime parsedTime = DateTime.ParseExact(startTimeString, "hh:mm tt", CultureInfo.InvariantCulture);
            DateTime startTime = new DateTime(dateTimePickerDate.Value.Year, dateTimePickerDate.Value.Month,
                dateTimePickerDate.Value.Day, parsedTime.Hour, parsedTime.Minute, parsedTime.Second);

            comboBoxEndTime.Items.Clear();
            try
            {
                DateTime time = startTime + increment;
                for (var i = timeSlots.IndexOf(startTime) + 1; i < timeSlots.Count - 1; i++)
                {
                    comboBoxEndTime.Items.Add(time.ToString("hh:mm tt"));
                    if (time == appointment.End) comboBoxEndTime.SelectedIndex = i - timeSlots.IndexOf(startTime) - 1;
                    time += increment;
                    if (time - timeSlots[i] != increment && timeSlots.Count - i != 1) break;
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show("This time will create an overlap. Please select a different time.\n" + ex.Message, "Out Of Range", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dateTimePickerDate_ValueChanged(object sender, EventArgs e)
        {
            // Get selected date
            DateTime selectedDate = dateTimePickerDate.Value;

            // Check if the selected date is a Saturday or Sunday
            if (selectedDate.DayOfWeek == DayOfWeek.Saturday || selectedDate.DayOfWeek == DayOfWeek.Sunday)
            {
                MessageBox.Show("Appointments can only be scheduled from Monday to Friday.\n" +
                    "Please pick a different date.", "Business Hours", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                LoadStartTimes();
                comboBoxEndTime.Items.Clear();
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Get selected date
            DateTime selectedDate = dateTimePickerDate.Value;

            // Check if the selected date is a Saturday or Sunday
            if (selectedDate.DayOfWeek == DayOfWeek.Saturday || selectedDate.DayOfWeek == DayOfWeek.Sunday)
            {
                MessageBox.Show("Appointments can only be scheduled from Monday to Friday.\n" +
                    "Please pick a different date.", "Business Hours", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validation 1: Title required
            if (string.IsNullOrEmpty(textBoxTitle.Text))
            {
                MessageBox.Show("Title is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validation 2: Type required
            if (string.IsNullOrEmpty(textBoxType.Text))
            {
                MessageBox.Show("Type is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validation 3: Customer selection required
            if (comboBoxCustomer.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validation 4: Start and end time selection required
            if (comboBoxStartTime.SelectedIndex == -1 || comboBoxEndTime.SelectedIndex == -1)
            {
                MessageBox.Show("Start and end time are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Get values from form controls
                string title = textBoxTitle.Text;
                string description = textBoxDescriptiom.Text;
                string type = textBoxType.Text;
                int customerId = (int)comboBoxCustomer.SelectedValue;
                string contact = textBoxContact.Text;
                string url = textBoxUrl.Text;
                string location = textBoxLocation.Text;

                string startTimeString = comboBoxStartTime.SelectedItem.ToString();
                DateTime parsedStartTime = DateTime.ParseExact(startTimeString, "hh:mm tt", CultureInfo.InvariantCulture);
                DateTime startTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day,
                    parsedStartTime.Hour, parsedStartTime.Minute, parsedStartTime.Second);

                string endTimeString = comboBoxEndTime.SelectedItem.ToString();
                DateTime parsedEndTime = DateTime.ParseExact(endTimeString, "hh:mm tt", CultureInfo.InvariantCulture);
                DateTime endTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day,
                    parsedEndTime.Hour, parsedEndTime.Minute, parsedEndTime.Second);

                bool isSuccess = DatabaseConnection.UpdateAppointment(new Appointment
                {
                    AppointmentId = appointment.AppointmentId,
                    Title = title,
                    Description = description,
                    Type = type,
                    CustomerId = customerId,
                    UserId = Program.CurrentUserId,
                    Contact = contact,
                    Url = url,
                    Location = location,
                    Start = startTime.ToUniversalTime(),
                    End = endTime.ToUniversalTime(),
                    LastUpdate = DateTime.UtcNow,
                    LastUpdateBy = Program.CurrentUser,
                });

                if (isSuccess)
                {
                    MessageBox.Show("Appointment updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Close the form
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, such as database errors
                MessageBox.Show($"Error saving appointment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            // Ask for confirmation before deleting
            DialogResult result = MessageBox.Show("Are you sure you want to delete this appointment?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bool isSuccess = DatabaseConnection.DeleteAppointment(appointment.AppointmentId);
                if (isSuccess)
                {
                    MessageBox.Show("Appointment deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
