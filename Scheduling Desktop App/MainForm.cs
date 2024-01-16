using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scheduling_Desktop_App
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            dataGridViewCustomers.DataSource = DatabaseConnection.Customers;
            dataGridViewCustomers.ReadOnly = true;
            dataGridViewCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewCustomers.MultiSelect = false;
        }

        private void LoadComboBoxViews()
        {
            comboBoxView.Items.Add("Daily View");
            comboBoxView.Items.Add("Weekly View");
            comboBoxView.Items.Add("Monthly view");
            comboBoxView.SelectedIndex = 0;
        }

        private void DisplayAppointments(DateTime selectedDate)
        {
            int viewIndex = comboBoxView.SelectedIndex == -1 ? 0 : comboBoxView.SelectedIndex;
            // Fetch appointments from the database
            DatabaseConnection.FetchAppointments(selectedDate, viewIndex);

            // Clear existing controls in flowLayoutPanelApts
            flowLayoutPanelApts.Controls.Clear();

            if (DatabaseConnection.Appointments.Count == 0)
            {
                // If there are no appointments, add a label indicating so with font size 10
                var noAppointmentsLabel = new Label
                {
                    Text = "No appointments for this date",
                    Font = new Font("Microsoft Sans Serif", 10),
                    AutoSize = true,
                };

                flowLayoutPanelApts.Controls.Add(noAppointmentsLabel);
            }
            else
            {
                foreach (var appointment in DatabaseConnection.Appointments)
                {
                    // Create a new instance of each AppointmentControl
                    var control = new AppointmentControl
                    {
                        Title = appointment.Title,
                        Type = appointment.Type,
                        Time = appointment.Start.ToShortTimeString() + " - " + appointment.End.ToShortTimeString()
                    };

                    // Attach an event handler for the View button click
                    control.ViewButtonClick += (sender, e) => OpenUpdateAppointmentForm(appointment);

                    flowLayoutPanelApts.Controls.Add(control);
                }
            }
        }

        private void OpenUpdateAppointmentForm(Appointment appointment)
        {
            // Open the UpdateAppointmentForm and pass the appointment data
            using (UpdateAppointmentForm updateAppointmentForm = new UpdateAppointmentForm(appointment))
            {
                if (updateAppointmentForm.ShowDialog() == DialogResult.OK)
                {
                    // Refresh display after update
                    DisplayAppointments(monthCalendar.SelectionStart);
                }
            }
        }

        private void DisplayCustomers()
        {
            DatabaseConnection.FetchCustomers();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadComboBoxViews();
            DisplayAppointments(DateTime.Today);
            DisplayCustomers();
        }
        
        private async void MainForm_Shown(object sender, EventArgs e)
        {
            await Task.Delay(3000);
            DatabaseConnection.AlertIfUpcomingAppointment(DateTime.Today, DateTime.Now + TimeSpan.FromMinutes(15));
        }
        
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void monthCalendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            DateTime selectedDate = monthCalendar.SelectionStart;
            DisplayAppointments(selectedDate);
        }

        private void comboBoxView_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = monthCalendar.SelectionStart;
            DisplayAppointments(selectedDate);
        }

        private void buttonAddNewCustomer_Click(object sender, EventArgs e)
        {
            using (NewCustomerForm newCustomerForm = new NewCustomerForm())
            {
                newCustomerForm.ShowDialog();
            }
        }

        private void buttonDeleteCustomer_Click(object sender, EventArgs e)
        {
            // Check if a row is selected in the dataGridViewCustomers
            if (dataGridViewCustomers.SelectedRows.Count > 0)
            {
                int customerId = Convert.ToInt32(dataGridViewCustomers.SelectedRows[0].Cells["CustomerId"].Value);

                // Ask for confirmation before deleting
                DialogResult result = MessageBox.Show("Are you sure you want to delete this customer?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    DatabaseConnection.DeleteCustomer(customerId);
                }
            }
            else
            {
                // Alert the user if no row is selected
                MessageBox.Show("Please select a customer to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void buttonUpdateCustomer_Click(object sender, EventArgs e)
        {
            // Check if a row is selected in the dataGridViewCustomers
            if (dataGridViewCustomers.SelectedRows.Count > 0)
            {
                int customerId = Convert.ToInt32(dataGridViewCustomers.SelectedRows[0].Cells["CustomerId"].Value);
                Customer customer = DatabaseConnection.Customers.FirstOrDefault(c => c.CustomerId == customerId);

                // open update customer form
                using (UpdateCustomerForm updateCustomerForm = new UpdateCustomerForm(customer))
                {
                    if (updateCustomerForm.ShowDialog() == DialogResult.OK)
                    {
                        DisplayCustomers();
                    }
                }
            }
            else
            {
                // Alert the user if no row is selected
                MessageBox.Show("Please select a customer to update.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void buttonNewAppointment_Click(object sender, EventArgs e)
        {
            using (NewAppointmentForm newAppointmentForm = new NewAppointmentForm(monthCalendar.SelectionStart))
            {
                if (newAppointmentForm.ShowDialog() == DialogResult.OK)
                {
                    DisplayAppointments(monthCalendar.SelectionStart);
                }
            }
        }

        private void buttonToggle_Click(object sender, EventArgs e)
        {
            using (OverviewForm  overviewForm = new OverviewForm())
            {
                overviewForm.ShowDialog();
            }
        }
    }
}
