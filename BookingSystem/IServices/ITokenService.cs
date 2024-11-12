namespace BookingSystem.IServices
{
    public interface ITokenService
    {
        public string CreateToken(string username,out DateTime expireDate);
        public string RefreshToken(string username,string deviceId, out DateTime expireDate);
    }
}
