using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Jessamine.Client.Pages
{
  public partial class Conversations
  {
    [Parameter]
    public long ConversationId { get; set; }

    protected override Task OnInitializedAsync()
    {
      return base.OnInitializedAsync();
    }
  }
}
