using System;
using System.Windows.Forms;

namespace Scheduling_Desktop_App
{
    public partial class OverviewForm : Form
    {
        public OverviewForm()
        {
            InitializeComponent();

            dataGridViewUserSchedule.DataSource = DatabaseConnection.MonthAppointments;
            dataGridViewUserSchedule.ReadOnly = true;
            dataGridViewTypesReport.DataSource = DatabaseConnection.AppointmentTypesCounts;
            dataGridViewTypesReport.ReadOnly = true;
            dataGridViewCustomerReport.DataSource = DatabaseConnection.AppointmentCustomerCounts;
            dataGridViewCustomerReport.ReadOnly = true;
        }

        private void LoadMonthComboBox()
        {
            for (int i = 1; i <= 12; i++)
            {
                comboBoxMonth.Items.Add(new DateTime(2024, i, 1).ToString("MMMM"));
            }
        }

        private void LoadYearComboBox()
        {
            comboBoxYear.Items.Add(2023);
            comboBoxYear.Items.Add(2024);
            comboBoxYear.Items.Add(2025);
        }

        private void Overview_Load(object sender, EventArgs e)
        {
            LoadMonthComboBox();
            LoadYearComboBox();
            comboBoxMonth.SelectedItem = DateTime.Now.ToString("MMMM");
            comboBoxYear.SelectedItem = DateTime.Now.Year;
            DatabaseConnection.GetMonthAppointments(DateTime.Now.Year, DateTime.Now.ToString("MMMM"));
            DatabaseConnection.GetAppointmentTypes(DateTime.Now.Year, DateTime.Now.ToString("MMMM"));
            DatabaseConnection.GetAppointmentCustomers(DateTime.Now.Year, DateTime.Now.ToString("MMMM"));
        }

        private void comboBoxMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            int year = comboBoxYear.SelectedItem == null ? DateTime.Now.Year : (int)comboBoxYear.SelectedItem;
            DatabaseConnection.GetMonthAppointments(year, (string)comboBoxMonth.SelectedItem);
            DatabaseConnection.GetAppointmentTypes(year, (string)comboBoxMonth.SelectedItem);
            DatabaseConnection.GetAppointmentCustomers(year, (string)comboBoxMonth.SelectedItem);
        }
    }
}
