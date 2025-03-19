using Sedna.Service.Requirements.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Senda.Requirements.Capture.UI.Popups
{
    public class RequirementsDetailVM : INotifyPropertyChanged
    {

        public RequirementsDetailVM()
        {

        }


        public Requirement Requirement
        {
            get
            {
                return _requirement;
            }
            set
            {
                _requirement = value;
                OnPropertyChanged();
            }
        }
        private Requirement _requirement;


        /// <summary>
        /// The list of all the subjects on screen at the moment.  For example, one opportunity and two lines.
        /// This needn't be an ObservableCollection because it won't change.
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
        private ObservableCollection<Subject> _subjects;


        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<VM.TopicsBySubjectType> SubjectTopicDetails
        {
            get
            {
                return _subjectTopicDetails;
            }
            set
            {
                _subjectTopicDetails = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<VM.TopicsBySubjectType> _subjectTopicDetails;

        /// <summary>
        /// The list of all valid topics for the subjects on screen right now.  
        /// This needn't be an ObservableCollection because it won't change.
        /// </summary>
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





        public List<int> SelectedContextIds
        {
            get
            {
                return this.CheckableContexts.Where(c => c.IsChecked).Select(c => c.Context.ContextId).ToList();
            }

            set
            {
                _selectedContextIds = value;
                this.CheckableContexts.ToList().ForEach(c => c.IsChecked = false);
                foreach(int id in _selectedContextIds)
                {
                    this.CheckableContexts.Where(c => _selectedContextIds.Contains(c.Context.ContextId)).ToList().ForEach(c => c.IsChecked = true);
                }
            }
        }
        private List<int> _selectedContextIds = new List<int>();


        /// <summary>
        /// This is the master lists of contexts that a requirement might be relevant in.
        /// </summary>
        /// <param name="contexts"></param>
        public void ContextsList(ObservableCollection<Context> contexts)
        {
            var temp = new ObservableCollection<CheckableContext>();
            foreach(var context in contexts)
            {
                temp.Add(new CheckableContext(context));
            }

            this.CheckableContexts = temp;
        }


        /// <summary>
        /// The master list of all contexts available to choose from.
        /// This needn't be an ObservableCollection because it won't change.
        /// </summary>
        public ObservableCollection<CheckableContext> CheckableContexts
        {
            get
            {
                return _checkableContexts;
            }
            set
            {
                _checkableContexts = value;
                _checkableContexts = new ObservableCollection<CheckableContext>(_checkableContexts.OrderBy(cc => cc.Context.ContextId).ToArray());
                OnPropertyChanged();
            }
        }
        private ObservableCollection<CheckableContext> _checkableContexts;


        public class CheckableContext
        {
            public CheckableContext(Context context)
            {
                this.Context = context;
            }
            
            public Context Context
            {
                get
                {
                    return _context;
                }
                set
                {
                    _context = value;
                }
            }
            private Context _context;


            public bool IsChecked
            {
                get
                {
                    return _isChecked;
                }
                set
                {
                    _isChecked = value;
                }
            }

            private bool _isChecked = false;

        }



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
