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
