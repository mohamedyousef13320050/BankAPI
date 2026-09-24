namespace BankSystem.Models
{
    public class JwtSettings
    {
        public string Key { get; set; } = "SuperSecretBankManagementSystemJwtKey2026!#$";
        public string Issuer { get; set; } = "BankSystemServer";
        public string Audience { get; set; } = "BankSystemClients";
        public int DurationInMinutes { get; set; } = 180; // 3 Hours
    }
}
