using Westwind.Data.EfCore;
using Westwind.Webstore.Business.Entities;

namespace Westwind.Webstore.Business;

/// <summary>
///  Base business object for all store related business objects
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class WebStoreBusinessObject<TEntity> : EntityFrameworkBusinessObject<WebStoreContext, TEntity>
    where TEntity : class, new()
{
    public WebStoreBusinessObject(WebStoreContext context) : base(context)
    {
            
    }
}



