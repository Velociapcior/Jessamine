using System.Threading.Tasks;
using Jessamine.Client.State.Conversation.Actions;
using Microsoft.AspNetCore.Components;

namespace Jessamine.Client.Pages
{
  public partial class Conversations
  {
    [Parameter]
    public long ConversationId { get; set; }

    protected override Task OnInitializedAsync()
    {
      _dispatcher.Dispatch(new FetchConversations());

      return base.OnInitializedAsync();
    }
  }
}
