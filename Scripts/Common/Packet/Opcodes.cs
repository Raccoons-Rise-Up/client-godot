namespace Common.Networking.Packet
{
    // Received from Game Client
    public enum ClientPacketOpcode
    {
        Disconnect,
        PurchaseItem,
        CreateAccount,
        Login,
        ChatMessage
    }

    // Sent to Game Client
    public enum ServerPacketOpcode
    {
        ClientDisconnected,
        PurchasedItem,
        CreatedAccount,
        LoginResponse,
        PlayerData,
        ChatMessage,
        PlayerJoinLeave,
        PlayerList
    }

    public enum PurchaseItemResponseOpcode
    {
        Purchased,
        NotEnoughResources
    }

    public enum JoinLeaveOpcode 
    {
        Join,
        Leave
    }

    public enum LoginResponseOpcode
    {
        LoginSuccessReturningPlayer,
        LoginSuccessNewPlayer,
        VersionMismatch,
        InvalidToken
    }

    public enum DisconnectOpcode
    {
        Disconnected,
        Maintenance,
        Restarting,
        Kicked,
        Banned,
        PlayerWithUsernameExistsOnServerAlready
    }
}
