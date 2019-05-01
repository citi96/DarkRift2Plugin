namespace GameCommon {
    public enum MessageParameterCode : byte {
        SubCodeParameterCode = 0, // Match the same value in the config file
        PeerID = 1,
        LoginName,
        Password,
        UserId,
        Email,
        Skulls,
        Souls
    }
}