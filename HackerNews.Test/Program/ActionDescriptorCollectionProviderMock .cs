using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace HackerNews.Test.Program
{
    /// <summary>
    /// Action descriptor collection property.
    /// </summary>
    public class ActionDescriptorCollectionProviderMock : IActionDescriptorCollectionProvider
    {
        public ActionDescriptorCollection ActionDescriptors { get; } = new ActionDescriptorCollection(new List<ActionDescriptor>(), 0);
    }



}
