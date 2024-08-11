using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using BlazorApp.Data;
using BlazorApp.Data.Chat;

namespace BlazorApp.Components.Shared.ChatHub;

public class ChatHubComponentBase : ComponentBase, IAsyncDisposable
{
    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected IConfiguration Configuration { get; set; }

    [Inject]
    public ChatBroadcastModel ChatBroadcast { get; set; }

    protected List<ChatMessageModel> Messages { get; set; }

    private string PageName { get; set; }

    protected bool DisableChat => Convert.ToBoolean(Configuration["DisableOrderChat"]);

    [CascadingParameter(Name = "UserData")]
    protected UserData UserData { get; set; }

    protected bool IsChatting { get; set; }
    protected bool IsReloadingMessages { get; set; }
    protected bool IsChatError { get; set; }
    protected string ChatErrorMessage { get; set; }

    protected bool IsLoading { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        ChatBroadcast ??= new ChatBroadcastModel { Messages = [] };
        ChatBroadcast.Messages ??= [];
        PageName = new Uri(NavigationManager.Uri).PathAndQuery;

        PopulateChatMessages();

        await Task.Delay(1);//Force a refresh

        await Chat();
        StateHasChanged();
    }

    private void PopulateChatMessages()
    {
        //Clear out all the previous messages for this user
        ChatBoxHelper.ClearChat(ChatBroadcast, UserData.Username);


    }

    protected string _HubUrl;
    protected Microsoft.AspNetCore.SignalR.Client.HubConnection _HubConnection;

    protected bool NoMessages => Messages == null || Messages.Count == 0;

    protected string CallingUsername { get; set; }
    protected string CallingPage { get; set; }

    /// <summary>
    /// All this method does is act like a trigger to refresh the display of the singleton
    /// </summary>
    /// <param name="username"></param>
    /// <param name="pageName"></param>
    private void BroadcastMessage(string username, string pageName)
    {
        if (username != null)
        {
            bool isMine = UserData.Username != null && username.Equals(UserData.Username, StringComparison.OrdinalIgnoreCase);
            //The user that sent this message will have updated the singleton that contains their order message
            if (isMine)
            {
                return;
            }

            CallingUsername = username;
            CallingPage = pageName;
            IsReloadingMessages = true;
            StateHasChanged();

            Messages = ChatBroadcast.Messages;

            IsReloadingMessages = false;
            StateHasChanged();
            CallingUsername = string.Empty;
            CallingPage = string.Empty;
        }
    }

    public async Task Chat()
    {
        // User must be working on an order number
        if (string.IsNullOrWhiteSpace(PageName) || DisableChat)
        {
            return;
        };

        try
        {
            IsChatting = true;
            await Task.Delay(1);

            var baseUrl = NavigationManager.BaseUri;
            _HubUrl = baseUrl.TrimEnd('/') + ChatHubControl.__HubUrl;
            _HubConnection = new HubConnectionBuilder()
                .WithUrl(_HubUrl, options =>
                {
                    options.WebSocketConfiguration = conf =>
                    {
                        conf.RemoteCertificateValidationCallback = (message, cert, chain, errors) => { return true; };
                    };
                    options.HttpMessageHandlerFactory = factory => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                    };
                })
                .Build();

            _HubConnection.On<string, string>("Broadcast", BroadcastMessage);

            await _HubConnection.StartAsync();

            await SendAsync();
        }
        catch (Exception e)
        {
            ChatErrorMessage = $"ERROR: Failed to start chat client: {e.Message}";
            IsChatError = true;
            IsChatting = false;
        }
    }

    private async Task SendAsync()
    {
        if (IsChatting)
        {
            await _HubConnection.SendAsync("Broadcast", UserData.Username, PageName);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_HubConnection is not null)
        {
            await _HubConnection.DisposeAsync();
        }
    }

    protected bool IsLicensingConflict(ChatMessageModel message)
    {
        if (string.IsNullOrEmpty(PageName))
        {
            return false;
        }
        try
        {
            return message.Page.Equals(PageName, StringComparison.CurrentCultureIgnoreCase) &&
                       !message.Username.Equals(UserData.Username, StringComparison.CurrentCultureIgnoreCase);
        }
        catch (Exception)
        {
            return false;
        }
    }

}
