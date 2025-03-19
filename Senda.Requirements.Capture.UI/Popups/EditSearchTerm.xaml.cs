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
    /// Interaction logic for AddSearchTerm.xaml
    /// </summary>
    public partial class EditSearchTerm : Window
    {

         private TopicSearch topicSearch;
        public EditSearchTerm(TopicSearch topicSearch)
        {
            InitializeComponent();
            this.topicSearch = topicSearch;


            this.DataContext = topicSearch;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtTopicSearch.Focus();
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

    }
}
