using Newtonsoft.Json;
using System.Collections.Generic;

namespace cyberblast.Kendo.Angular.Grid.Model {
    public class GridState
    {
        /**
         * The number of records to be skipped by the pager.
         */
        [JsonProperty("skip")]
        public int? Skip { get; set; }
        /**
         * The number of records to take.
         */
        [JsonProperty("take")]
        public int? Take { get; set; }
        /**
         * The descriptors used for sorting.
         */
        [JsonProperty("sort")]
        public List<SortDescriptor> Sort { get; set; }
        /**
         * The descriptors used for filtering.
         */
        [JsonProperty("filter")]
        public CompositeFilterDescriptor Filter { get; set; }
        /*
         * The descriptors used for grouping.
         */
        //group?: Array<GroupDescriptor>;

        //public Expression<Func<TEntity, object>> SortExpression<TEntity>()
        //{
        //    //sortEntry => sortEntry.Id
        //    if (sort != null && sort.Count > 0)
        //    {
        //        var sorting = sort[0];
        //        var item = Expression.Parameter(typeof(TEntity), "item");
        //        var prop = Expression.Property(item, sorting.field);                
        //        return Expression.Lambda<Func<TEntity, object>>(prop, item);
        //    }
        //    return null;
        //}

    }
}
