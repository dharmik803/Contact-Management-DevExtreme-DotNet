namespace Contact_management_devextreme.Models
{
    public interface IRefreshTokenGenerator
    {
        string GenerateToken(string username);
    }
}
