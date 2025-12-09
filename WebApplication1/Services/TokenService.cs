using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;



namespace WebApplication1.Services
{
    
    public class TokenService
    {

        private readonly IConfiguration _configuration;


        public TokenService(IConfiguration configuration)
        {

            _configuration = configuration;

        }


        public string 
            GenerateToken(
                string username, 
                string role )

        {

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {

                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)

            };


            var token = new JwtSecurityToken(

                _configuration["Jwt:Issuer"],

                _configuration["Jwt:Audience"],

                claims,

                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),

                signingCredentials: credentials

            );


            return new JwtSecurityTokenHandler().WriteToken(token);

        }


    }



}
