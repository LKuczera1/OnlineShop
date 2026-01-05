namespace Identity.Dtos
{
    public class RefreshRequestDto
    {
        public string Username { get; set; } = string.Empty;

        //Token is already send in request
        //public string JWTtoken { get; set; } = string.Empty;
    }
}