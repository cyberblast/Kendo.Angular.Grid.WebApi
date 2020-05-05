using cyberblast.Kendo.Angular.Grid.WebApi.Model;

namespace cyberblast.Kendo.Angular.Grid.WebApi {
    public interface IKendoServiceController<TModel>
        where TModel : class, new()
    {
        KendoServiceResult<TModel> List(GridState state);
    }
}
