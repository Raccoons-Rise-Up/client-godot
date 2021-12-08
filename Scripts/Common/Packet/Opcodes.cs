namespace Common.Networking.Packet
{
    // Received from Game Client
    public enum ClientPacketOpcode
    {
        Disconnect,
        PurchaseItem,
        CreateAccount,
        Login
    }

    // Sent to Game Client
    public enum ServerPacketOpcode
    {
        ClientDisconnected,
        PurchasedItem,
        CreatedAccount,
        LoginResponse,
        PlayerData
    }

    public enum PurchaseItemResponseOpcode
    {
        Purchased,
        NotEnoughResources
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
        Banned
    }
}
