using Newtonsoft.Json;
using System.Collections.Generic;

namespace cyberblast.Kendo.Angular.Grid.WebApi.Model {
    public class KendoServiceResult<TModel>
        where TModel : class, new()
    {
        /**
         * The data that will be rendered by the Grid as an array.
         */
        [JsonProperty("data")]
        public List<TModel> Data { get; set; }
        /**
         * The total number of records that are available.
         */
        [JsonProperty("total")]
        public int Total { get; set; }
    }
}
