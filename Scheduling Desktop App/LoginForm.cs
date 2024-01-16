using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace Scheduling_Desktop_App
{
    public partial class LoginForm : Form
    {
        private Dictionary<string, Dictionary<string, string>> localizedStrings;
        string userLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        public LoginForm()
        {
            InitializeComponent();
            InitializeLocalizedStrings();

            // Set the text of the controls based on the user's language
            buttonLogin.Text = GetLocalizedString("ButtonLoginText", userLanguage);
            labelUsername.Text = GetLocalizedString("LabelUsernameText", userLanguage);
            labelPassword.Text = GetLocalizedString("LabelPasswordText", userLanguage);
        }

        private void InitializeLocalizedStrings()
        {
            localizedStrings = new Dictionary<string, Dictionary<string, string>>();

            // English strings
            var englishStrings = new Dictionary<string, string>
            {
                { "ValidationErrorMessage", "Validation Error" },
                { "ValidationUsernameRequired", "Username is required." },
                { "ValidationPasswordRequired", "Password is required." },
                { "LoginFailedTitle", "Login Failed" },
                { "LoginFailedMessage", "The username and password do not match." },
                { "ButtonLoginText", "Login" },
                { "LabelUsernameText", "Username" },
                { "LabelPasswordText", "Password" }
            };
            localizedStrings.Add("en", englishStrings);

            // French strings
            var frenchStrings = new Dictionary<string, string>
            {
                { "ValidationErrorMessage", "Erreur de validation" },
                { "ValidationUsernameRequired", "Nom d'utilisateur requis." },
                { "ValidationPasswordRequired", "Mot de passe requis." },
                { "LoginFailedTitle", "Échec de la connexion" },
                { "LoginFailedMessage", "Le nom d'utilisateur et le mot de passe ne correspondent pas." },
                { "ButtonLoginText", "Connexion" },
                { "LabelUsernameText", "Nom d'utilisateur" },
                { "LabelPasswordText", "Mot de passe" }
            };
            localizedStrings.Add("fr", frenchStrings);
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Get the user's country or region name 
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            string location = currentCulture.DisplayName;

            // Get the user's timezone
            TimeZoneInfo userTimeZone = TimeZoneInfo.Local;
            string timeZone = userTimeZone.DisplayName;

            // Display the location in the UI
            labelLocation.Text = location + " — " + timeZone;
        }

        private string GetLocalizedString(string key, string language)
        {
            if (!localizedStrings.ContainsKey(language))
            {
                language = "en";
            }

            if (localizedStrings.TryGetValue(language, out var languageStrings) &&
                languageStrings.TryGetValue(key, out var localizedString))
            {
                return localizedString;
            }

            //return $"[Key '{key}' not found for language '{language}']";
            return "";
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string username = textBoxUsername.Text;
            string password = textBoxPassword.Text;
            int userId = DatabaseConnection.AuthenticateUser(username, password);

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show(GetLocalizedString("ValidationUsernameRequired", userLanguage),
                                GetLocalizedString("ValidationErrorMessage", userLanguage), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(GetLocalizedString("ValidationPasswordRequired", userLanguage),
                                GetLocalizedString("ValidationErrorMessage", userLanguage), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (userId != -1)
            {
                Program.CurrentUser = username;
                Program.CurrentUserId = userId;

                string logFilePath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "Login_History.txt");
                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {username} logged in");
                }

                MainForm mainForm = new MainForm();
                mainForm.Show();
                Hide();
            }
            else
            {
                MessageBox.Show(GetLocalizedString("LoginFailedMessage", userLanguage),
                                GetLocalizedString("LoginFailedTitle", userLanguage), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
