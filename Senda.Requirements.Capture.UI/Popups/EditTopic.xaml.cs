using Sedna.Service.Requirements.DTO;
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

namespace Senda.Requirements.Capture.UI.Popups
{
    /// <summary>
    /// Interaction logic for EditTopic.xaml
    /// </summary>
    public partial class EditTopic : Window
    {
        public EditTopic()
        {
            InitializeComponent();
        }

        private Topic topic;
        public EditTopic(Topic topic)
        {
            InitializeComponent();
            this.topic = topic;


            this.DataContext = topic;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtTopicName.Focus();
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Only update the underlying data explicitly now...
            foreach (var be in BindingOperations.GetSourceUpdatingBindings(this))
            {
                be.UpdateSource();
            }

            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;

        }

        private void btnCancel_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnSave_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
