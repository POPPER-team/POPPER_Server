namespace POPPER_Server.Dtos;

public class TokensDto
{
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
        
        public override bool Equals(object obj)
        {
                if (obj == null || GetType() != obj.GetType())
                {
                        return false;
                }

                var other = (TokensDto)obj;
                return JwtToken == other.JwtToken && RefreshToken == other.RefreshToken;
        }

        public override int GetHashCode()
        {
                return HashCode.Combine(JwtToken, RefreshToken);
        }
}