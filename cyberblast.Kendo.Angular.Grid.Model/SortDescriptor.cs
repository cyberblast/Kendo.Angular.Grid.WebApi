using Newtonsoft.Json;

namespace cyberblast.Kendo.Angular.Grid.WebApi.Model 
{
    public class SortDescriptor
    {
        /**
         * The field that is sorted.
         */
        [JsonProperty("field")]
        public string Field { get; set; }
        /**
         * The sort direction. If no direction is set, the descriptor will be skipped during processing.
         *
         * The available values are:
         * - `asc`
         * - `desc`
         */
        [JsonProperty("dir")]
        public string Dir { get; set; }
    }
}
