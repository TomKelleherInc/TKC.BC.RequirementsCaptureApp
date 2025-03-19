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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Senda.Requirements.Capture.UI.Components.Requirements
{
    /// <summary>
    /// Interaction logic for RequirementSummary.xaml
    /// </summary>
    public partial class RequirementSummary : UserControl
    {
        public RequirementSummary()
        {
            InitializeComponent();
        }



        public event EditLink_ClickedEventHandler EditLink_Clicked;
        public delegate void EditLink_ClickedEventHandler(object sender, Requirement requirement);

        private void lblEdit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var req = this.DataContext as Requirement;
            EditLink_Clicked(this, req);


        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
