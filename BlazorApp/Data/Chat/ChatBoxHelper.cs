namespace BlazorApp.Data.Chat
{
    public static class ChatBoxHelper
    {
        public static void ClearChat(ChatBroadcastModel chatBroadcast, string username)
        {
            if (chatBroadcast?.Messages == null || !chatBroadcast.Messages.Any() || string.IsNullOrEmpty(username))
            {
                return; //Probably waiting for the user to authenticate
            }
            chatBroadcast.Messages = chatBroadcast.Messages
                .Where(x => !x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }
    }
}
