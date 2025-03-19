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
using System.Linq;
using System.Globalization;

namespace Senda.Requirements.Capture.UI.Popups
{
    /// <summary>
    /// Interaction logic for RequirementsDetails.xaml
    /// </summary>
    public partial class RequirementsDetails : Window
    {
        RequirementsDetailVM viewModel;

        public RequirementsDetails(RequirementsDetailVM viewModel)
        {
            InitializeComponent();

            this.viewModel = viewModel;
            this.DataContext = viewModel;

            // Requirement might have gotten Subject and Topic from another source, 
            // and so aren't literally the same object even if identical. This makes it 
            // hard to set the dropdowns correctly. So we sync them up first.
            if (viewModel.Requirement.SubjectId > 0)
            {
                viewModel.Requirement.Subject = viewModel.Subjects.First(s => s.SubjectId == viewModel.Requirement.SubjectId);
                viewModel.Topics = viewModel.SubjectTopicDetails.Where(std => std.SubjectType.SubjectTypeId == viewModel.Requirement.Subject.SubjectTypeId).First().Topics;
                viewModel.Requirement.Topic = viewModel.Topics.First(t => t.TopicId == viewModel.Requirement.TopicId);
            }
            // Now the dropdowns are using the same references.
            cmbSubject.SelectionChanged += CmbSubject_SelectionChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
             
            cmbSubject.SelectedItem = viewModel.Requirement.Subject;

            if(viewModel.Requirement.RequirementId > 0)
            {
                btnDelete.Visibility = Visibility.Visible;
            }
            else
            {
                btnDelete.Visibility = Visibility.Hidden;
            }
        }


        private void CmbSubject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Subject subject = cmbSubject.SelectedItem as Subject;
            viewModel.Topics = viewModel.SubjectTopicDetails.Where(std => std.SubjectType.SubjectTypeId == subject.SubjectType.SubjectTypeId).First().Topics;

        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(viewModel.Requirement.Subject == null)
            {
                cmbSubject.Focus();
                MessageBox.Show("Must select a subject", "Requirement Details Incomplete", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbSubject.IsDropDownOpen = true;
                return;
            }

            if (viewModel.Requirement.Topic == null)
            {
                cmbTopic.Focus();
                MessageBox.Show("Must select a topic", "Requirement Details Incomplete", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbTopic.IsDropDownOpen = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.Requirement.PreferredPhrasing))
            {
                txtPreferredPhrasing.Focus();
                MessageBox.Show("Phrasing cannot be empty", "Requirement Details Incomplete", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (viewModel.CheckableContexts.Any(c => c.IsChecked) == false)
            {
                lvContexts.Focus();
                MessageBox.Show("Must select a one or more contexts", "Requirement Details Incomplete", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Only update the underlying data explicitly now...
            foreach (var be in BindingOperations.GetSourceUpdatingBindings(this))
            {
                be.UpdateSource();
            }

            this.DialogResult = true;
        }

        private void PreferredPhrasingSetCase(object sender, MouseButtonEventArgs e)
        {
            if (sender == lblAllUpperCase)
                txtPreferredPhrasing.Text = txtPreferredPhrasing.Text.ToUpper();

            if(sender == lblAllLowerCase)
                txtPreferredPhrasing.Text = txtPreferredPhrasing.Text.ToLower();

            if (sender == lblAllMixedCase)
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;  // neat trick!
                txtPreferredPhrasing.Text = textInfo.ToTitleCase(txtPreferredPhrasing.Text.ToLower()); // only works on lowercase text, though.

            }

        }


        /// <summary>
        /// A special flag for the calling routine, to let them know the user wants to delete this
        /// requirement. Must only be available if the RequirementId > 0, meaning it's not being created
        /// anew in this session.
        /// </summary>
        public bool DeleteThisRequirement { get; set; } = false;

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Deleting this requirement cannot be undone.  Are you sure you want to proceed?", "Delete Requirement", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.DeleteThisRequirement = true;
                this.DialogResult = true;
            }
        }
    }
}
