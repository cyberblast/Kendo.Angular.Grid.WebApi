using Newtonsoft.Json;

namespace cyberblast.Kendo.Angular.Grid.WebApi.Model 
{
    public class FilterDescriptor
    {
        /**
         * The data item field to which the filter operator is applied.
         */
        [JsonProperty("field")]
        public string Field { get; set; }
        /**
         * The filter operator (comparison).
         *
         * The supported operators are:
         * * `"eq"` (equal to)
         * * `"neq"` (not equal to)
         * * `"isnull"` (is equal to null)
         * * `"isnotnull"` (is not equal to null)
         * * `"lt"` (less than)
         * * `"lte"` (less than or equal to)
         * * `"gt"` (greater than)
         * * `"gte"` (greater than or equal to)
         *
         * The following operators are supported for string fields only:
         * * `"startswith"`
         * * `"endswith"`
         * * `"contains"`
         * * `"doesnotcontain"`
         * * `"isempty"`
         * * `"isnotempty"`
         */
        [JsonProperty("operator")]
        public string Operator { get; set; }
        /**
         * The value to which the field is compared. Has to be of the same type as the field.
         */
        [JsonProperty("value")]
        public object Value { get; set; }
        /**
         * Determines if the string comparison is case-insensitive.
         */
        [JsonProperty("ignoreCase")]
        public bool? IgnoreCase { get; set; }
    }
}
