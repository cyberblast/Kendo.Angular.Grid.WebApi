using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace cyberblast.Kendo.Angular.Grid.Model
{
    public class CompositeFilterDescriptor
    {
        /**
         * The logical operation to use when the `filter.filters` option is set.
         *
         * The supported values are:
         * * `"and"`
         * * `"or"`
         */
        [JsonProperty("logic")]
        public string Logic { get; set; }
        /**
         * The nested filter expressions&mdash;either [`FilterDescriptor`]({% slug api_kendo-data-query_filterdescriptor %}), or [`CompositeFilterDescriptor`]({% slug api_kendo-data-query_compositefilterdescriptor %}). Supports the same options as `filter`. You can nest filters indefinitely.
         */
        [JsonProperty("filters")]
        public List<JObject> Filters { get; set; } // Array<FilterDescriptor | CompositeFilterDescriptor>;
    }
}
