using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SampleApp.Models
{
    public class TrackSearchModel: IValidatableObject
    {
        private String _TrackName = "";
        private String _ArtistName = "";

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

        public int NumberOfResults { get; set; }
        public int NumberOfResultsPerPage { get; set; }
        public int NumberOfPages
        {
            get
            {
                // TODO: Write tests for this
                if (NumberOfResults == 0) { return 0; }
                if (NumberOfResults < NumberOfResultsPerPage) { return 1; }
                if (NumberOfResults % NumberOfResultsPerPage == 0) { return NumberOfResults / NumberOfResultsPerPage; }
                return (NumberOfResults / NumberOfResultsPerPage) + 1;
            }
        }
        public int Page { get; set; }
        public int ResultEnumerationStart
        {
            get
            {
                // TODO: Write tests for this
                return (this.NumberOfResultsPerPage * (this.Page - 1)) + 1;
            }
        }

        public List<TrackSearchResultModel>? Results { get; set; }

        // TODO: Write a test for this
        public TrackSearchModel()
        {
            this.NumberOfResultsPerPage = 5;
        }


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
