using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SampleApp.Models
{
    public class TrackSearchModel: IValidatableObject
    {
        private String _TrackName = "";
        private String _ArtistName = "";
        private int _TotalNumberOfResults, _Page;

        [DisplayName("Track")]
        public String? TrackName 
        { 
            get { return this._TrackName; }
            set
            {
                this._TrackName = (value ?? "").Trim();
            }
        }

        [DisplayName("Artist")]
        public String? ArtistName
        {
            get { return this._ArtistName; }
            set
            {
                this._ArtistName = (value ?? "").Trim();
            }
        }

        public int TotalNumberOfResults 
        { 
            get { return this._TotalNumberOfResults; }
            set { this._TotalNumberOfResults = (value < 0 ? 0 : value); }
        }
        public int NumberOfResultsPerPage { get; set; } = 5;
        public int NumberOfPages
        {
            get
            {
                if (TotalNumberOfResults % NumberOfResultsPerPage == 0) { return TotalNumberOfResults / NumberOfResultsPerPage; }
                return (TotalNumberOfResults / NumberOfResultsPerPage) + 1;
            }
        }
        public int Page
        {
            get 
            {
                return ((this._Page == 0 && this.TotalNumberOfResults > 0) ? 1 : this._Page);
            }
            set { this._Page = value; }
        }
        public int ResultsCounterStart
        {
            get 
            { 
                if (this.Page == 0) return 0;
                return (this.NumberOfResultsPerPage * (this.Page - 1)) + 1; 
            }
        }

        public List<TrackSearchResultModel>? Results { get; set; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (String.IsNullOrEmpty(this.TrackName) && String.IsNullOrEmpty(this.ArtistName))
            { 
                // This error is not tied to a specific field, which means it will only show up in a validation summary on the view.
                // To tie it to a specific view property, specify the second constructor parameter as "new[] { nameof(PropertyName} }"
                yield return new ValidationResult("Please specify one or both of Track and Artist");
            }
        }
    }
}
