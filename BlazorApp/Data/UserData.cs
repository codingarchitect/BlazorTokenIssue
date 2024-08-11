using System.Security.Claims;

namespace BlazorApp.Data
{
    public class UserData
    {
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }

        public List<Claim> UserClaims = new();

        public List<UserClaim> MyClaims
        {
            get
            {
                try
                {
                    return UserClaims
                              .Select(c => new UserClaim { Type = c.Type, Value = c.Value })
                              .ToList();
                }
                catch (Exception)
                {
                    return new List<UserClaim> { new UserClaim { Type = "Error", Value = "Error" } };
                }
            }
        }
    }

    public class UserClaim
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Type}: {Value}";
        }
    }
}
