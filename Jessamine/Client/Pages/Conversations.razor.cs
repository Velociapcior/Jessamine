using System;
using System.Linq;
using System.Threading.Tasks;
using Jessamine.Client.State.Conversation.Actions;
using Microsoft.AspNetCore.Components;

namespace Jessamine.Client.Pages
{
  public partial class Conversations
  {
    [Parameter]
    public long? ConversationId { get; set; }

    protected override Task OnInitializedAsync()
    {

      return base.OnInitializedAsync();
    }

    protected override void OnAfterRender(bool firstRender)
    {
      if (firstRender)
      {
        _dispatcher.Dispatch(new FetchConversations(ConversationId));
      }
    }
  }
}
