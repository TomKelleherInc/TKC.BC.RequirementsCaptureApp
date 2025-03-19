using Sedna.API.Core.Model;
using Sedna.Service.Requirements.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Senda.Requirements.Capture.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string currentExternalIdentifier = string.Empty;
        RequirementCaptureVM viewModel = null;
        DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(ComboBox.ItemsSourceProperty, typeof(ComboBox));

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(string SubjectExternalIdentifier)
        {
            InitializeComponent();

            LoadSubjectDataAndWindow(SubjectExternalIdentifier, null);
        }


        /// <summary>
        /// Loads the ViewModel (which probably only has the boring reference data) and 
        /// the external identifier (e.g., the string "OPP00123456") which then loads the 
        /// specifics of the opportunity.
        /// </summary>
        /// <param name="SubjectExternalIdentifier"></param>
        /// <param name="vm"></param>
        public void LoadSubjectDataAndWindow(string SubjectExternalIdentifier, RequirementCaptureVM vm)
        {
            currentExternalIdentifier = SubjectExternalIdentifier;

            if (vm != null)
            {
                viewModel = vm;
                this.DataContext = viewModel;
            }
        }

        public MainWindow(string SubjectExternalIdentifier, RequirementCaptureVM vm)
        {
            InitializeComponent();
            LoadSubjectDataAndWindow(SubjectExternalIdentifier, vm);
        }

        private void ViewModel_SelectedSourceChanged(object sender, EventArgs e)
        {
            viewModel.SourceText = pdfDisplay.GetDocumentText();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.NeedSourceText += ViewModel_NeedSourceText;

            if (Instance.SednaApiToken != string.Empty && currentExternalIdentifier != string.Empty)
            {
                //confirm the token is still good
                if(Instance.TestSednaApiToken(Instance.SednaApiToken) == false ) { return; }

                var query = new Sedna.API.Core.Model.Solicitation.Query() { Filter = currentExternalIdentifier.Trim() };
                if(Helpers.ObjectIdHelper.IsValid(currentExternalIdentifier))
                {
                    int oppId = Helpers.ObjectIdHelper.GetId(currentExternalIdentifier);
                    List<int> oppIds = new List<int>() { oppId };

                    var opportunitySummaries = Sedna.API.Client.Lib.Dashboard.GetOpportunitySummariesByIds(Instance.SednaApiToken, oppIds);

                    // because we're loading by "OPP00123456" there will always be a max of one.
                    viewModel.LoadOpportunityAndAttachments(opportunitySummaries.First());
                }
            }

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvRequirements.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Subject.Description");
            view.GroupDescriptions.Add(groupDescription);

            if(string.IsNullOrWhiteSpace(Instance.SlackChannelName) == false)
                Common.Slack.Send(Instance.SlackChannelName, "Requirements Capture Tool Login", $"{Environment.UserName} just logged in", Common.Slack.SlackEmoji.okay);

            //For testing exception handling
            //throw new DivideByZeroException("Well, that blew up pretty good");



            descriptor.AddValueChanged(cmbAttachments, (sender, e) =>
            {
                if (cmbAttachments.Items.Count > 0)
                { 
                    cmbAttachments.IsDropDownOpen = true; 
                }
                else
                {
                    MessageBox.Show("No attachments were found for this opportunity.", "New Opportunity Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            });
        }

        private void ViewModel_NeedSourceText(object sender, EventArgs e)
        {
            viewModel.SourceText = pdfDisplay.GetDocumentText();
        }

        private void btnSelectOpportunity_Click(object sender, RoutedEventArgs e)
        {
            OpenOpportunity();

        }

        private void LoadSourceFileToViewer(Attachment attachment)
        {
            if(attachment == null) { return;  }

            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                byte[] pdfBytes = Sedna.API.Client.Lib.Attachments.DownloadById(Instance.SednaApiToken, attachment.AttachmentId);
                pdfDisplay.LoadPdf(pdfBytes);
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

        private void cmbAttachments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbAttachments.SelectedIndex >= 0)
            {
                Attachment attachment = cmbAttachments.SelectedItem as Attachment;
                LoadSourceFileToViewer(attachment);
                viewModel.SetSelectedSource(attachment);
            }
        }

         

        private void tbTopicSearch_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void accordionSubjects_SelectedItemChanged(object sender, DevExpress.Xpf.Accordion.AccordionSelectedItemChangedEventArgs e)
        {
            var newItem = e.NewItem;

            if (e.NewItem is TopicSearch)
            {
                TopicSearch topicSearch = e.NewItem as TopicSearch;

                var topics = viewModel.SubjectTypeTopics.SelectMany(std => std.Topics).ToList();
                Topic topic = topics.Where(t => t.TopicId == topicSearch.TopicId).FirstOrDefault();
                viewModel.LatestTopic = topic;

                var subjectType = viewModel.SubjectTypeTopics.Where(std => std.Topics.Contains(topic)).First().SubjectType;
                viewModel.LatestSubjectType = subjectType;

                pdfDisplay.pdfViewer.ContinueSearchFrom = DevExpress.Xpf.PdfViewer.PdfContinueSearchFrom.LastSearchResult; 
                pdfDisplay.Find(topicSearch.SearchString,
                    topicSearch.IsWholeWord,
                    DevExpress.Xpf.DocumentViewer.TextSearchDirection.Forward);

                txtSearch.Text = topicSearch.SearchString;
            }
        }

        private void btnSearchPrevious_Click(object sender, RoutedEventArgs e)
        {
            pdfDisplay.pdfViewer.ContinueSearchFrom = DevExpress.Xpf.PdfViewer.PdfContinueSearchFrom.LastSearchResult;
            pdfDisplay.Find(txtSearch.Text, false, DevExpress.Xpf.DocumentViewer.TextSearchDirection.Backward);
        }

        private void btnSearchNext_Click(object sender, RoutedEventArgs e)
        {
            pdfDisplay.pdfViewer.ContinueSearchFrom = DevExpress.Xpf.PdfViewer.PdfContinueSearchFrom.LastSearchResult;
            pdfDisplay.Find(txtSearch.Text, false, DevExpress.Xpf.DocumentViewer.TextSearchDirection.Forward);

        }


        private void btnCaptureRequirement_Click(object sender, RoutedEventArgs e)
        {
            // Prepare a new requirement
            Requirement newRequirement = new Requirement();
            newRequirement.RequirementId = 0;  // must be zero, so that posting it auto-generates a new ID
            newRequirement.PreferredPhrasing = pdfDisplay.CurrentlySelectedPhrase;
            newRequirement.SourceTextLocation = "page:" + pdfDisplay.CurrentPage;
            newRequirement.ReviewDt = null;
            newRequirement.SourceText = pdfDisplay.CurrentlySelectedPhrase;
            newRequirement.Source = viewModel.SelectedSource;
            newRequirement.SourceId = viewModel.SelectedSource.SourceId;
            newRequirement.IsActive = true;
            newRequirement.CreatedTs = DateTime.Now;
            newRequirement.UpdatedTs = DateTime.Now;
            newRequirement.TopicId = viewModel.LatestTopic?.TopicId;  //null propogation, baby!  :-)

            var vm = new Popups.RequirementsDetailVM();
            vm.ContextsList(viewModel.Contexts);
            vm.Subjects = viewModel.Subjects;
            vm.SubjectTopicDetails = viewModel.SubjectTypeTopics;
            vm.Requirement = newRequirement;             

            Popups.RequirementsDetails reqDetails = new Popups.RequirementsDetails(vm);
            reqDetails.Owner = this;

            if (reqDetails.ShowDialog() == true)
            {
                try
                {
                    Requirement req = vm.Requirement;

                    if (req.Source.SourceId == 0)
                    {
                        /* Issue: We are sometimes assembling a Requirement from a Source that doesn't exist
                         * in the database yet.  So we need to add it to the database and get a non-zero ID
                         * for it, and also update all the internal references here to it.  Those include the 
                         *  - viewModel.SelectedSource
                         *  - vm.Requirement.Source
                         *  - vm.Requirement.SourceId (doesn't automatically change!)
                         * 
                         * 
                         */
                        //req.Source.SourceType = null;
                        Source newSource = Sedna.Service.Requirements.API.Client.Sources.Post(req.Source);
                        req.Source = newSource;
                        req.SourceId = newSource.SourceId;
                        viewModel.SelectedSource = newSource;
                        viewModel.Sources.First(s => s.ExternalIdentifier == newSource.ExternalIdentifier).SourceId = newSource.SourceId;
                    }

                    if (req.Subject.SubjectId == 0)
                    {
                        //does the req.SourceID change, too?
                        //req.Subject.SubjectType = null;
                        Subject newSubject = Sedna.Service.Requirements.API.Client.Subjects.Post(req.Subject);
                        req.Subject = newSubject;
                        viewModel.Subjects.First(s => s.ExternalIdentifier == newSubject.ExternalIdentifier).SubjectId = newSubject.SubjectId;
                        req.SubjectId = newSubject.SubjectId;
                        //viewModel.Subjects
                    }

                    if (req.RequirementId == 0)
                    {
                        //req.SourceId = req.Source.SourceId;
                        //req.Source = null;
                        //req.SubjectId = req.Subject.SubjectId;
                        //req.Subject = null;
                        req.TopicId = req.Topic.TopicId;
                        //req.Topic = null;
                        //req.RequirementContext = null;
                        req = Sedna.Service.Requirements.API.Client.Requirements.Post(req);
                        viewModel.Requirements.Add(req);

                        //Annotates the PDF and leaves a message 
                        pdfDisplay.AnnotateCurrentlySelectedText($"Requirement captured by {Environment.UserName}\n\nTopic: {req.Topic.Name}\n\nRequirement ID: {req.RequirementId}\n\nCaptured on {DateTime.Now.ToShortDateString()}\n\nPhrasing captured: {req.PreferredPhrasing}", Colors.PaleGreen);
                    }
                    else
                    {
                        req.SubjectId = req.Subject.SubjectId;
                        req.SourceId = req.Source.SourceId;
                        req.TopicId = req.Topic.TopicId;
                        Sedna.Service.Requirements.API.Client.Requirements.Put(req);
                    }

                    // We add the Requirement Contexts to the database by sending in just the IDs, and 
                    // the ID of the parent Requirement.  The API does a "reset" by assuming this incoming list
                    // is the full set.  It adds/deletes as needed to match the database to this set of IDs.
                    var preferredContextIds = vm.SelectedContextIds;

                    //Capture the new contexts and attach them to the requirement
                    req.RequirementContexts = Sedna.Service.Requirements.API.Client.Requirements.ResetContexts(req.RequirementId, preferredContextIds);

                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                    throw;
                }                
            }
        }


        public void RequirementSummary_EditLink_Clicked(object sender, Requirement requirement)
        {
            EditRequirement(requirement);
        }

        private void RequirementSummary_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Requirement requirement = (e.Source as UI.Components.Requirements.RequirementSummary).DataContext  as Requirement;
            EditRequirement(requirement);
        }



        /// <summary>
        /// To edit an existing requirement.
        /// </summary>
        /// <param name="requirement"></param>
        public void EditRequirement(Requirement requirement)
        {
            if(requirement == null) { return; }

            var vm = new Popups.RequirementsDetailVM();
            vm.ContextsList(viewModel.Contexts);
            vm.SelectedContextIds = requirement.RequirementContexts.Select(c => c.ContextId).ToList();
            vm.Subjects = viewModel.Subjects;
            vm.SubjectTopicDetails = viewModel.SubjectTypeTopics;
            vm.Topics = viewModel.Topics;
            vm.Requirement = requirement;

            Popups.RequirementsDetails reqDetails = new Popups.RequirementsDetails(vm);
            reqDetails.Owner = this;

            if (reqDetails.ShowDialog() == true)
            {
                try
                {
                    Requirement req = vm.Requirement;

                    if (reqDetails.DeleteThisRequirement)
                    {
                        viewModel.Requirements.Remove(req);
                        List<int> noContextIds = new List<int>();
                        Sedna.Service.Requirements.API.Client.Requirements.ResetContexts(req.RequirementId, noContextIds);
                        Sedna.Service.Requirements.API.Client.Requirements.Delete(req.RequirementId);
                        return;
                    }



                    req.SubjectId = req.Subject.SubjectId;
                    //req.SourceId = req.Source.SourceId;
                    req.TopicId = req.Topic.TopicId;
                    req.RequirementContexts.Clear();
                    Sedna.Service.Requirements.API.Client.Requirements.Put(req);

                    var preferredContextIds = vm.SelectedContextIds;                    
                    req.RequirementContexts = Sedna.Service.Requirements.API.Client.Requirements.ResetContexts(req.RequirementId, preferredContextIds);

                }
                catch (Exception ex)
                {
                    var msg = ex.Message; 
                    throw;
                }
            }
        }

        private void btnSaveRequirementsToClipboard_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder tb = new StringBuilder();

            var reqOpportunity = viewModel.Requirements
                .Where(r => r.Subject.SubjectType.ReferenceKey == Common.Strings.SubjectTypes.ReferenceKeys.Opportunity)
                .OrderBy(r => r.Topic.Name)
                .ToList();

            var reqLines = viewModel.Requirements
                .Where(r => r.Subject.SubjectType.ReferenceKey == Common.Strings.SubjectTypes.ReferenceKeys.OpportunityLine)
                .OrderBy(r => r.Topic.Name).ThenBy(r => r.Subject.Description)
                .ToList();

            var tab1 = new String('\t', 1);
            var tab2 = new String('\t', 2);
            var tab3 = new String('\t', 3);
            var tab4 = new String('\t', 4);
            var tab5 = new String('\t', 5);

            tb.AppendLine("OPPORTUNITY");
            if (reqOpportunity.Count > 0)
            {
                foreach (var r in reqOpportunity)
                {
                    tb.AppendLine(tab1 + "Topic: " + r.Topic.Name);
                    tb.AppendLine(tab1 + "Detail: " + r.PreferredPhrasing);
                    tb.AppendLine();
                }
            }
            else
            {
                // no Opp requirements in list
                tb.AppendLine(tab1 + "(no requirements captured)");
                tb.AppendLine();
            }

            tb.AppendLine("OPPORTUNITY LINES");
            if (reqLines.Count > 0)
            {
                string lineDescription = reqLines.First().Subject.Description;

                foreach (var r in reqLines)
                {
                    if (lineDescription != r.Subject.Description)
                    {
                        lineDescription = r.Subject.Description;
                        tb.AppendLine();
                        tb.AppendLine(tab1 + "Name: " + lineDescription);
                    }
                    tb.AppendLine(tab2 + "Topic: " + r.Topic.Name);
                    tb.AppendLine(tab2 + "Detail: " + r.PreferredPhrasing);
                    tb.AppendLine();
                }
            }
            else
            {
                // no line reqs in list
                tb.AppendLine(tab1 + "(no requirements captured)");
                tb.AppendLine();
            }


            Clipboard.SetText(tb.ToString());

            System.Media.SystemSounds.Beep.Play();
        }

        private void tbTopicTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.RightButton == MouseButtonState.Pressed)
            {
                ContextMenu cm = this.FindResource("tbTopicTitle") as ContextMenu;
                cm.PlacementTarget = sender as Button;
                cm.IsOpen = true;
            }
        }

        private void menuNewSearchTerm_Click(object sender, RoutedEventArgs e)
        {
            Topic clickedTopic = (sender as Control).DataContext as Topic;
            if (clickedTopic == null) return;

            //Create new TopicSearch as the DataContext for the popup
            TopicSearch newTopicSearch = new TopicSearch()
            {
                IsWholeWord = false,
                SearchString = string.Empty,
                TopicId = clickedTopic.TopicId,
                TopicSearchId = 0,
                Topic = clickedTopic
            };
             

            Popups.EditSearchTerm formSearchTerm = new Popups.EditSearchTerm(newTopicSearch);

            var point = Mouse.GetPosition(Application.Current.MainWindow);
            formSearchTerm.Left = point.X - 50;
            formSearchTerm.Top = point.Y - 20;
            
            formSearchTerm.Owner = this; 

            if (formSearchTerm.ShowDialog() == true)
            {
                newTopicSearch.Topic = null;  // simplifies data updates
                newTopicSearch = Sedna.Service.Requirements.API.Client.TopicSearches.Post(newTopicSearch);
                clickedTopic.TopicSearches.Add(newTopicSearch);
                newTopicSearch.Topic = clickedTopic;
                viewModel.ShowOnlyFoundTerms = viewModel.ShowOnlyFoundTerms; // cheesy way to make the list refresh
            }

        }

        private void menuNewTopic_Click(object sender, RoutedEventArgs e)
        {
            VM.TopicsBySubjectType clickedSubjectTypeTopics = (sender as Control).DataContext as VM.TopicsBySubjectType;
            if (clickedSubjectTypeTopics == null) return;


            //Create new Topic as the DataContext for the popup
            Topic newTopic = new Topic()
            {
                Name = string.Empty,
                TopicId = 0,
                Description = string.Empty,
                PreferredPhrasing = string.Empty,
                ReferenceKey = string.Empty
            };


            Popups.EditTopic formTopic = new Popups.EditTopic(newTopic);

            var point = Mouse.GetPosition(Application.Current.MainWindow);
            formTopic.Left = point.X - 50;
            formTopic.Top = point.Y - 20;

            formTopic.Owner = this;

            if (formTopic.ShowDialog() == true)
            {
                newTopic.ReferenceKey = MakeReferenceKey(newTopic.Name);
                newTopic = Sedna.Service.Requirements.API.Client.Topics.Post(newTopic);
                SubjectTypeTopic stt = new SubjectTypeTopic() 
                { 
                    TopicId = newTopic.TopicId, 
                    SubjectTypeId = clickedSubjectTypeTopics.SubjectType.SubjectTypeId, 
                    SubjectTypeTopicId = 0                    
                };

                var sttNew = Sedna.Service.Requirements.API.Client.SubjectTypeTopics.Post(stt);

                clickedSubjectTypeTopics.Topics.Add(newTopic);
                viewModel.ShowOnlyFoundTerms = viewModel.ShowOnlyFoundTerms; // cheesy way to make the list refresh
            }
        }

        private string MakeReferenceKey(string name)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;  // neat trick!
            name = textInfo.ToTitleCase(name.ToLower()); // only works on lowercase text, though.

            string result = string.Empty;
            string[] terms = name.Split(' ');

            foreach (string term in terms)
            {
                if (term.Length >= 4)
                { result += term.Substring(0, 4); }
                else
                { result += term; }
            }
            return result;
        }
         

        private void OpenOpportunity()
        {
            try
            {
                var oppSearchWindow = new Popups.SearchForOpportunity();
                oppSearchWindow.Owner = this;

                if (oppSearchWindow.ShowDialog() == true)
                {
                    pdfDisplay.ClosePdf();
                    cmbAttachments.SelectedIndex = -1;
                    viewModel.LoadOpportunityAndAttachments(oppSearchWindow.Selected);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

            }
        }

        private void hypLinkToServiceDesk_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = e.Uri.ToString(),
                UseShellExecute = true
            };

            System.Diagnostics.Process.Start(psi);
        }


        /// <summary>
        /// Used to ensure that the combobox doesn't present a MessageBox of "No attachments found" when it hasn't been data-bound at least once.
        /// </summary>
        private bool attachments_displayed_before = false;

    }
}
