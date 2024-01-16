using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Scheduling_Desktop_App
{
    public partial class AppointmentControl : UserControl
    {
        private string _title;
        private string _type;
        private string _time;

        [Category("Custom Props")]
        public string Title 
        { 
            get { return _title; } 
            set { _title = value; labelTitle.Text = value; } 
        }

        [Category("Custom Props")]
        public string Type
        {
            get { return _type; }
            set { _type = value; labelType.Text = value; }
        }

        [Category("Custom Props")]
        public string Time
        {
            get { return _time; }
            set { _time = value; labelTime.Text = value; }
        }

        // Define an event for the view button click
        public event EventHandler ViewButtonClick;

        public AppointmentControl()
        {
            InitializeComponent();
        }

        // Event handler for the view button click
        private void buttonView_Click(object sender, System.EventArgs e)
        {
            ViewButtonClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
