namespace minecraft_launcher_v2.CustomStructs
{
    public enum AuthenticationResult : ushort
    {
        UnknownError,
        Success,
        MethodNotAllowed,
        NotFound,
        InvalidCredentials,
        InvalidCredentialsMigrated,
        InvalidCredentialsTooManyAttempts,
        CredentialsIsNull,
        InvalidToken,
        AccessTokenHasProfile,
        UnsupportedMediaType
    }

}
