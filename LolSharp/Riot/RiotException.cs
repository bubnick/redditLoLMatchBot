namespace LolSharp.Riot
{
    public enum RiotException
    {
        Good = 200,
        BadRequest = 400,
        AccessDenied = 401,
        NotFound = 404,
        RateLimitExceeded = 429,
        InternalServerError = 500,
        ServiceUnavailable = 503
    }
}
