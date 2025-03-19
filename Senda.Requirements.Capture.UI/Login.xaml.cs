using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Senda.Requirements.Capture.UI
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {

        public Login()
        {
            InitializeComponent();
            
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtUsername.Focus();
        }


        public string SednaLoginToken { get; set; }


        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            lblFeedback.Text = string.Empty;

            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Password) )
            {
                lblFeedback.Text = "Username and password are required";
                if (string.IsNullOrWhiteSpace(txtPassword.Password)) { txtPassword.Focus(); }
                if (string.IsNullOrWhiteSpace(txtUsername.Text)) { txtUsername.Focus(); }
                return;
            }

            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                SednaLoginToken = Sedna.API.Client.Instance.GetTokenFromPassword(txtUsername.Text, txtPassword.Password);

                this.DialogResult = true;                 
            }
            catch (Exception ex)
            {
                lblFeedback.Text = "Login failed.  Please check your info and try again.";
                txtPassword.Clear();
                txtPassword.Focus();
            }
            finally
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
            }

        }

        private void linkCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
