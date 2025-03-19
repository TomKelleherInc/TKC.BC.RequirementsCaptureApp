using Sedna.Service.Requirements.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Sedna.API.Core.Model;
using System.Collections.ObjectModel;
using System.Linq;

namespace Senda.Requirements.Capture.UI.Popups
{
    public class SearchForOpportunitiesVM : INotifyPropertyChanged
    {

        public void SearchForOpportunity(string searchString)
        {
            /* If it's an integer, it's the opportunity_id.
             * If it's an ObjectId, then grab the opportunity_id
             * If it's neither, presume it's a Customer Reference Number (and "SPE" number)             * 
             */

            int opportunityId = -1;
            List<Solicitation> solicitations = new List<Solicitation>();
            List<int> opportunityIds = new List<int>();

            searchString = searchString.ToLower().Trim();

            if(int.TryParse(searchString, out opportunityId))
            {
                // done, we've got the opportunityId!
                var sol = Sedna.API.Client.Lib.Solicitations.GetById(Instance.SednaApiToken, opportunityId);
                opportunityIds.Add(sol.OpportunityId.Value);
            }
            else if(Helpers.ObjectIdHelper.IsValid(searchString))
            {
                opportunityId = Helpers.ObjectIdHelper.GetId(searchString);
                var sol = Sedna.API.Client.Lib.Solicitations.GetById(Instance.SednaApiToken, opportunityId);
                opportunityIds.Add(sol.OpportunityId.Value);

            }
            else
            {
                searchString = searchString.Replace("-", string.Empty);

                // this could return more than one, so limit to 20 hits
                Sedna.API.Core.Model.Solicitation.Query query = new Solicitation.Query() { Filter = searchString };
                query.Length = 20;  // otherwise it might return a million
                solicitations = Sedna.API.Client.Lib.Solicitations.GetByQuery(Instance.SednaApiToken, query);
                opportunityIds.AddRange(solicitations.Select(s => s.OpportunityId.Value).ToList());

            }


            this.OpportunitySummaries = new ObservableCollection<OpportunitySummary>();

            // convert these into OpportunitySummaries
            var oppSummaries = Sedna.API.Client.Lib.Dashboard.GetOpportunitySummariesByIds(Instance.SednaApiToken, opportunityIds);
            this.OpportunitySummaries = new ObservableCollection<OpportunitySummary> ( oppSummaries );
        }

        public ObservableCollection<OpportunitySummary> OpportunitySummaries
        {
            get
            {
                return _oppSummaries;
            }
            set
            {
                _oppSummaries = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<OpportunitySummary> _oppSummaries = new ObservableCollection<OpportunitySummary>();



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
