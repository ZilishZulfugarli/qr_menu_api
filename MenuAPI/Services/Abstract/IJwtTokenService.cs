namespace MenuAPI.Services.Abstract
{
    public interface IJwtTokenService
    {
        public string GenerateToken(string name, string userName, List<string> roles, string Id);
    }
}
