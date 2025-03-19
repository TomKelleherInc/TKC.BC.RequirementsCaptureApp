using Sedna.API.Core.Model;
using Sedna.Service.Requirements.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using static Sedna.API.Client.BaseClient;

namespace Senda.Requirements.Capture.UI
{
    public class RequirementCaptureVM : INotifyPropertyChanged
    {

        public RequirementCaptureVM()
        {

        }



        #region Requirements API Data


        #region Reference Data

        /// <summary>
        /// Loads the general reference data for this type of external identifier.
        /// It doesn't load the specifics of this opportunity, it notes that it's *an* opportunity
        /// (not a line or NSN or Vendor), and loads up the contexts, topics, etc. appropriate for
        /// an opportunity.
        /// </summary>
        /// <param name="externalIdentifier"></param>
        /// <returns></returns>
        public void LoadReferenceData(string externalIdentifier)
        {
            this.Contexts = new ObservableCollection<Context>(Sedna.Service.Requirements.API.Client.Contexts.GetAll());
            this.SubjectTypes = new ObservableCollection<SubjectType>(Sedna.Service.Requirements.API.Client.SubjectTypes.GetAll());
            this.SourceTypes = new ObservableCollection<SourceType>(Sedna.Service.Requirements.API.Client.SourceTypes.GetAll());


            // For the near term, this tool is only going to look at Opportunities and their Lines.  Later, probably Awards and Lines.
            var subjectTypeTopics = new List<VM.TopicsBySubjectType>();

            if (string.IsNullOrWhiteSpace(externalIdentifier) || externalIdentifier.ToUpper().StartsWith("OPP"))
            {
                List<Topic> topics = new List<Topic>();
                var subjectType = this.SubjectTypes.Where(st => st.ReferenceKey == Common.Strings.SubjectTypes.ReferenceKeys.Opportunity).FirstOrDefault();
                int oppSubTypeId = subjectType.SubjectTypeId;
                topics = Sedna.Service.Requirements.API.Client.SubjectTypes.GetTopicDetails(oppSubTypeId);
                subjectTypeTopics.Add(new VM.TopicsBySubjectType(subjectType, topics));

                topics.Clear();
                var oppLineSubjectType = this.SubjectTypes.Where(st => st.ReferenceKey == Common.Strings.SubjectTypes.ReferenceKeys.OpportunityLine).FirstOrDefault();
                int oppLineSubTypeId = oppLineSubjectType.SubjectTypeId;
                topics = Sedna.Service.Requirements.API.Client.SubjectTypes.GetTopicDetails(oppLineSubTypeId);

                subjectTypeTopics.Add(new VM.TopicsBySubjectType(oppLineSubjectType, topics));
            }

            this.SetSubjectTopicDetails(subjectTypeTopics);
        }


        public ObservableCollection<SourceType> SourceTypes
        {
            get
            {
                return _sourceTypes;
            }
            set
            {
                _sourceTypes = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SourceType> _sourceTypes = new ObservableCollection<SourceType>();

        public ObservableCollection<Topic> Topics
        {
            get
            {
                return _topics;
            }
            set
            {
                _topics = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Topic> _topics = new ObservableCollection<Topic>();



        /// <summary>
        /// All the possible Brighton Cromwell workflow contexts in which a requirement might 
        /// be relevant or important.
        /// <para>
        /// This collection will include the TopicContexts that relate to it.  These are used to
        /// prompt the user with the "typical" contexts in which that a particular requirement might 
        /// be relevant--though the user can pick others.
        /// </para>
        /// </summary>
        public ObservableCollection<Context> Contexts
        {
            get
            {
                return _contexts;
            }
            set
            {
                _contexts = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Context> _contexts;

        /// <summary>
        /// The master list of subject-types that can have requirements associated with them, such
        /// as "Opportunity," "OpportunityLine," "Award," "Part, "Vendor" and so on.
        /// </summary>
        public ObservableCollection<SubjectType> SubjectTypes
        {
            get
            {
                return _subjectTypes;
            }
            set
            {
                _subjectTypes = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SubjectType> _subjectTypes;

        #endregion


        //========================================================
        #region Usage-Specific Data

        /// <summary>
        /// Loads the previously-collected Subjects, Sources, Requirements and RequirementContexts for
        /// this particular opportunity and it's lines.  The Sources come from the attachments. If 
        /// some of these already exist on the database, we take those; if not we make new ones with
        /// an ID of zero.  Same for the Subjects and Requirements.
        /// </summary>
        /// <param name="opportunitySummary"></param>
        public void LoadOpportunityAndAttachments(OpportunitySummary opportunitySummary)
        {
            this.CurrentOpportunity = null;
            this.Subjects = new ObservableCollection<Subject>();
            this.Sources = new ObservableCollection<Source>();
            this.Requirements = new ObservableCollection<Requirement>();

            if (opportunitySummary == null) { return; }

            try
            {
                this.CurrentOpportunity = opportunitySummary;

                var oppSubjType = this.SubjectTypes.Where(st => st.ReferenceKey == Common.Strings.SubjectTypes.ReferenceKeys.Opportunity).First();
                var lineSubjType = this.SubjectTypes.Where(st => st.ReferenceKey == Common.Strings.SubjectTypes.ReferenceKeys.OpportunityLine).First();

                List<string> opportunityObjectIds = new List<string>();

                string oppObjectID = Sedna.API.Core.Helper.ObjectIdHelper.Format("OPP" + opportunitySummary.OpportunityId);

                //------------------------------
                // Load the Opportunity Subject (check if they exist already)
                var oppSubject = Sedna.Service.Requirements.API.Client.Subjects.GetByExternalIdentifierWithRequirements(oppObjectID);

                if (oppSubject != null)
                {
                    this.Subjects.Add(oppSubject);
                }
                else
                {
                    this.Subjects.Add(new Subject()
                    {
                        Description = opportunitySummary.CustomerReferenceNumber,
                        ExternalIdentifier = oppObjectID,
                        SubjectId = 0,
                        SubjectType = oppSubjType,
                        SubjectTypeId = oppSubjType.SubjectTypeId,
                        Tag = null
                    });
                }

                opportunityObjectIds.Add(oppObjectID);


                //---------------------
                // Load the Opp-Line Subjects (check if they exist already)
                foreach (var line in opportunitySummary.Lines)  // just need to do this once, if at all
                {
                    string lineObjectId = Sedna.API.Core.Helper.ObjectIdHelper.Format("OPL" + line.OpportunityLineId);

                    var lineSubject = Sedna.Service.Requirements.API.Client.Subjects.GetByExternalIdentifierWithRequirements(lineObjectId);
                    if (lineSubject != null)
                    {
                        this.Subjects.Add(lineSubject);
                    }
                    else
                    {

                        this.Subjects.Add(new Subject()
                        {
                            Description = line.Name,
                            ExternalIdentifier = lineObjectId,
                            SubjectId = 0,
                            SubjectType = lineSubjType,
                            SubjectTypeId = lineSubjType.SubjectTypeId,
                            Tag = null
                        });
                    }
                    opportunityObjectIds.Add(lineObjectId);
                }


                //------------------------------
                // Get any requirements we already know about for these objectIds.
                var requirements = Sedna.Service.Requirements.API.Client.Requirements.GetBySubjectExternalIdentifiers(opportunityObjectIds);
                this.Requirements = new ObservableCollection<Requirement>(requirements);


                var attachments = Sedna.API.Client.Lib.Attachments.GetByRelatedObjectIds(Instance.SednaApiToken, opportunityObjectIds);

                // whittle the list down to just the file types we can deal with
                attachments = attachments.Where(a => a.Filename.ToLower().EndsWith(".pdf")
                    || a.Filename.ToLower().EndsWith(".xls")
                    || a.Filename.ToLower().EndsWith(".xlsx")
                    || a.Filename.ToLower().EndsWith(".doc")
                    || a.Filename.ToLower().EndsWith(".docx"))
                    .ToList();

                this.SetAttachments(attachments);

                //------------------------------
                // Load the Sources (check if they exist already)
                foreach (var att in attachments)
                {
                    string attObjectID = Sedna.API.Core.Helper.ObjectIdHelper.Format("ATT" + att.AttachmentId);
                    Source objSource = Sedna.Service.Requirements.API.Client.Sources.GetByExternalIdentifier(attObjectID);

                    if (objSource != null)
                    {
                        this.Sources.Add(objSource);
                    }
                    else
                    {
                        SourceType srcType = GetSourceType(att.Filename);

                        objSource = new Source()
                        {
                            Name = att.Filename,
                            SourceId = 0,
                            SourceType = srcType,
                            SourceTypeId = srcType.SourceTypeId,
                            ExternalIdentifier = att.ObjectId
                        };

                        this.Sources.Add(objSource);
                    }
                    dctAttachmentSources.Add(att, objSource);
                }

            }
            catch (ServerApiException ex)
            {
                MessageBox.Show("Error occured gathering Sedna data.", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occured preparing the main screen.", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                //throw;
            }
            finally
            {
                 
            }
        }

        private Dictionary<Attachment, Source> dctAttachmentSources = new Dictionary<Attachment, Source>();


        private SourceType GetSourceType(string filename)
        {
            string fileExtension = filename.Trim().Split('.').Last().ToLower();

            switch(fileExtension)
            {
                case "xls":
                case "xlsx":
                    return this.SourceTypes.First(s => s.ReferenceKey == Common.Strings.SourceTypes.ReferenceKeys.ExcelFile);
                    break;

                case "doc":
                case "docx":
                    return this.SourceTypes.First(s => s.ReferenceKey == Common.Strings.SourceTypes.ReferenceKeys.WordFile);
                    break;


                case "pdf":
                    return this.SourceTypes.First(s => s.ReferenceKey == Common.Strings.SourceTypes.ReferenceKeys.PdfFile);
                    break;

                default:
                    return this.SourceTypes.First(s => s.ReferenceKey == Common.Strings.SourceTypes.ReferenceKeys.Person);
                    break;
            }
        }


        public OpportunitySummary CurrentOpportunity
        {
            get
            {
                return _currentOpportunity;
            }
            set
            {
                _currentOpportunity = value;
                OnPropertyChanged();
            }
        }
        private OpportunitySummary _currentOpportunity;


        public void SetAttachments(List<Attachment> attachments)
        {
            this.Attachments = new ObservableCollection<Attachment>(attachments);
        }

        public ObservableCollection<Attachment> Attachments
        {
            get
            {
                return _attachments;
            }
            set
            {
                _attachments = value;
                OnPropertyChanged();
            }
        }
        private static ObservableCollection<Attachment> _attachments = new ObservableCollection<Attachment>();


        public SubjectType LatestSubjectType { get; set; }
        public Topic LatestTopic { get; set; }




        /// <summary>
        /// Needed so that the View Model can tell the UI that it needs to get the text from
        /// the viewer.  It then pulls that, and sends that data in via the SourceText property.
        /// Dumb, but if we try to do this all from the UI when the source-selection occurs, 
        /// on the Combo_SelectionChanged event, the PDF control issues some error that seems
        /// to indicate it's not ready.
        /// </summary>
        public event EventHandler NeedSourceText;

        /// <summary>
        /// Used to tell the UI to send in the source text through the .SourceText property.
        /// </summary>
        private void OnNeedSourceText()
        {
            NeedSourceText?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// This is called by the UI after we issue the "OnNeedSourceText()" event.
        /// Yes, this is a bit loopy to follow.
        /// </summary>
        public string SourceText
        {
            get
            {
                return _sourceText;
            }
            set
            {
                _sourceText = value + string.Empty;  // appending the string is a sneaky way of turning nulls to empty strings
                OnPropertyChanged();
            }
        }
        private string _sourceText = string.Empty;


        /// <summary>
        /// Takes the current SourceText and compares it to the contents of the _masterSubjectdTypeTopics,
        /// to create the _filteredSubjectTypeTopics object -- which is the same, but whittled down to only
        /// contain the Topics and TopicSearches that it finds in this document's text.
        /// </summary>
        private void CreateFilteredSubjectTypeTopicsForSource()
        {
            //need to make _filteredSubjectTypeTopics a wholly distinct collection, with no references connecting them.  Yay, Json!
            var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(_masterSubjectTypeTopics, jsonSettings);

            _filteredSubjectTypeTopics = Newtonsoft.Json.JsonConvert.DeserializeObject<List<VM.TopicsBySubjectType>>(json);

            // Remove the stuff tha's not in SourceText
            foreach (var subj in _filteredSubjectTypeTopics)
            {
                foreach (var topic in subj.Topics)
                {
                    var ids = topic.TopicSearches.Select(ts => ts.TopicSearchId).ToList();

                    foreach (int id in ids)
                    {
                        var topicSearch = topic.TopicSearches.FirstOrDefault(ts => ts.TopicSearchId == id);
                        string searchString = topicSearch.SearchString.ToLower();

                        // strip out the ones we don't find, being careful to
                        // do the whole-word searches with RegEx.

                        if (topicSearch.IsWholeWord)  // use RegEx for these
                        {
                            if (Regex.Match(this.SourceText, $@"\b{searchString}\b").Success == false)
                            {
                                topic.TopicSearches.Remove(topicSearch);
                            }
                        }
                        else  // do normal "contains()" checking for these
                        {
                            if (this.SourceText.Contains(searchString) == false)
                            {
                                topic.TopicSearches.Remove(topicSearch);
                            }
                        }
                    }
                }


                // Remove any "empty" Topics.

                // Have to iterate by IDs, because you can't do .Remove() 
                // on an Enumerable while looping thru its objects.
                var topicIds = subj.Topics.Select(t => t.TopicId).ToList();

                foreach (int topicId in topicIds)
                {
                    var topic = subj.Topics.First(t => t.TopicId == topicId);
                    if (topic.TopicSearches.Count == 0)
                    {
                        subj.Topics.Remove(topic);
                    }
                }

            }
        }


        public bool ShowOnlyFoundTerms
        {
            get
            {
                return _showOnlyFoundTerms;
            }

            set
            {
                try
                {
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                    _showOnlyFoundTerms = value;

                    // If changing now to show only the found terms, need to get source text
                    if (_showOnlyFoundTerms && string.IsNullOrWhiteSpace(SourceText))
                    {
                        // Get the latest document text so we can compare
                        OnNeedSourceText();
                        CreateFilteredSubjectTypeTopicsForSource();
                    }

                    // Change the view now
                    if (_showOnlyFoundTerms)
                    {
                        SubjectTypeTopics = new ObservableCollection<VM.TopicsBySubjectType>(_filteredSubjectTypeTopics);
                    }
                    else
                    {
                        SubjectTypeTopics = new ObservableCollection<VM.TopicsBySubjectType>(_masterSubjectTypeTopics);
                    }

                }
                catch (Exception ex)
                {
                    throw;
                }
                finally 
                {
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showOnlyFoundTerms = false;

        /// <summary>
        /// The unfiltered original list of SubjectTypeTopics drawn from the database.  This is assigned
        /// to .SubjectTypeTopics when the user wants to see all the Topics available.
        /// </summary>
        private List<VM.TopicsBySubjectType> _masterSubjectTypeTopics = new List<VM.TopicsBySubjectType>();

        /// <summary>
        /// The filtered list of SubjectTypeTopics, based on which ones were found in the text of
        /// the active source/attachment.  This hides from the user the ones they needn't worry about
        /// because we've already determined those TopicSearch phrases don't occur in it.  This is assigned
        /// to .SubjectTypeTopics when the user wants to see the filtered list.
        /// </summary>
        private List<VM.TopicsBySubjectType> _filteredSubjectTypeTopics = new List<VM.TopicsBySubjectType>();

        public void SetSubjectTopicDetails(List<VM.TopicsBySubjectType> subjectTypeTopics)
        {
            _masterSubjectTypeTopics = subjectTypeTopics;
            this.SubjectTypeTopics = new ObservableCollection<VM.TopicsBySubjectType>(subjectTypeTopics);

        }


        /// <summary>
        /// A special collection of the Topics (and child TopicSearch objects) for a given
        /// SubjectType.  So for an OpportunityLine, this list of topics might include HazMat
        /// and ShelfLife; for a PartEntity or OpportunityLine it might include Special Packaging 
        /// Instructions.  This special collection omits the database's "SubjectType_Topics"
        /// cross-reference table.
        /// </summary>
        public ObservableCollection<VM.TopicsBySubjectType> SubjectTypeTopics
        {
            get
            {
                return _subjectTopicDetails;
            }
            private set
            {
                _subjectTopicDetails = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// The "active" set of SubjectTypeTopics that are being used in the 
        /// UI at the moment.  Compare to the "_filteredSubjectTypeTopics" and "_masterSubjectTypeTopics."
        /// </summary>
        private static ObservableCollection<VM.TopicsBySubjectType> _subjectTopicDetails = new ObservableCollection<VM.TopicsBySubjectType>();


        /// <summary>
        /// When a new document is loaded this is emptied, and then
        /// new items are added as their search terms are found in the 
        /// document.  This is then used to highlight/hide the UI elements
        /// corresponding to these terms.
        /// </summary>
        public ObservableCollection<TopicSearch> FoundTopicSearches
        {
            get
            {
                return _foundTopicSearches;
            }

            set
            {
                _foundTopicSearches = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<TopicSearch> _foundTopicSearches = new ObservableCollection<TopicSearch>();



        /// <summary>
        /// The particular documents/files/etc. that we're pulling requirements from.
        /// </summary>
        public ObservableCollection<Source> Sources
        {
            get
            {
                return _sources;
            }

            set
            {
                _sources = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Source> _sources = new ObservableCollection<Source>();


        /// <summary>
        /// The specific things we are gathering requirements about, such as the 
        /// Opportunity, Opportunity Line, Award, Award Line, etc.  When capturing requirements for
        /// an Opportunity, the Opportunity Lines are also loaded for req-capture, so this is a collection
        /// rather than a single object.  
        /// <para>
        /// Each subject will have these children/grandchildren: SubjectType, SubjectTypeTopics, and the TopicSearch terms for those.
        /// </para>
        /// </summary>
        public ObservableCollection<Subject> Subjects
        {
            get
            {
                return _subjects;
            }

            set
            {
                _subjects = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Subject> _subjects = new ObservableCollection<Subject>();


        /// <summary>
        /// The requirements gathered for this collection of Subjects.  These could be captured 
        /// during this session, or reloaded from the database if a user is revisiting them.
        /// </summary>
        public ObservableCollection<Requirement> Requirements  
                {
            get
            {
                return _requirements;
            }

            set
            {
                _requirements = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Requirement> _requirements = new ObservableCollection<Requirement>();


        public CollectionView RequirementsSorted
        {
            get
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Requirements);
                view.SortDescriptions.Add(new SortDescription("Topic.Name", ListSortDirection.Ascending));
                return view;
            }
        }






        /// <summary>
        /// This is called when user selects a source document from the top combobox of PDFs.
        /// 
        /// </summary>
        /// <param name="attachment"></param>
        public void SetSelectedSource(Attachment attachment)
        {
            if (dctAttachmentSources[attachment] != null) { SelectedSource = dctAttachmentSources[attachment]; }

            // We need to ditch the previous doc's text, as a flag that we need to get the new doc's text.
            // Also, because we don't know what the 
            this.SourceText = string.Empty;
            this.ShowOnlyFoundTerms = false;

            this.SubjectTypeTopics = new ObservableCollection<VM.TopicsBySubjectType>(_masterSubjectTypeTopics);
        }

        public Source SelectedSource
        {
            get
            {
                return _selectedSource;
            }
            set
            {
                _selectedSource = value;
                OnPropertyChanged();
            }
        }
        private Source _selectedSource;


        #endregion

        #endregion

        #region Property Change Code

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}




namespace Senda.Requirements.Capture.UI.VM
{
    /// <summary>
    /// A speciality object that lets us group the Topic-details objects under the SubjectTypes they pertain to.
    /// </summary>
    public class TopicsBySubjectType
    {

        public TopicsBySubjectType(SubjectType SubjectType, List<Topic> Topics)
        {
            this.SubjectType = SubjectType;
            this.Topics = new ObservableCollection<Topic>(Topics);
        }

        public SubjectType SubjectType
        {
            get
            {
                return _subjectType;
            }
            set
            {
                _subjectType = value;
                OnPropertyChanged();
            }
        }
        private SubjectType _subjectType;


        public ObservableCollection<Topic> Topics
        {
            get
            {
                return _topics;
            }

            set
            {
                _topics = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Topic> _topics;

        #region Property Change Code

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}


