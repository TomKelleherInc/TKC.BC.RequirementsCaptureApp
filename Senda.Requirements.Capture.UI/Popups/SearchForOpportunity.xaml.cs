using Sedna.API.Core.Model;
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
    /// Interaction logic for ReviewOpportunity.xaml
    /// </summary>
    public partial class SearchForOpportunity : Window
    {

        SearchForOpportunitiesVM vm = null;
        public SearchForOpportunity()
        {
            InitializeComponent();

            vm = new SearchForOpportunitiesVM();

            this.DataContext = vm;
        }

        public OpportunitySummary Selected { get; set; }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            vm.SearchForOpportunity(txtSearch.Text);
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Selected = null;
            this.DialogResult = false;
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if(listOpportunities.SelectedItem == null) { return; }

            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                var oppSummary = listOpportunities.SelectedItem as OpportunitySummary;
                this.Selected = oppSummary;
                this.DialogResult = true;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
            }


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtSearch.Focus();
        }
    }
}
