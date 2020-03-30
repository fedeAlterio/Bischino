using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Bischino.Settings;

namespace Bischino.Base.Security
{
    public class Jwt
    {
        private static string Issuer { get; set; }
        private static string Audience { get; set; }
        public static SymmetricSecurityKey SymmetricSecurityKey { get; private set; }
        public static SigningCredentials SigningCredentials { get; private set; }

        public JwtSecurityToken Token { get; }

        public string TokenString { get;}

        public static void Initialize(JwtSettings jwtSettings)
        {
            Issuer = jwtSettings.Issuer;
            Audience = jwtSettings.Issuer;
            SymmetricSecurityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings.SecurityKey));
            SigningCredentials = new SigningCredentials(SymmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
        }

        public Jwt(DateTime expires, string role = null, IList<Claim> optionalClaims = null, string issuer = "issuer", string audience = "audience")
        {
            List<Claim> claims = new List<Claim>();
            if (optionalClaims != null)
                claims.AddRange(optionalClaims);
            if (role != null)
                claims.Add(new Claim(ClaimTypes.Role, role));
            Token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: expires,
                signingCredentials: SigningCredentials,
                claims: claims);
            TokenString = new JwtSecurityTokenHandler().WriteToken(Token);
        }

        public static string Hash(string str)
        {
            byte[] salt = new byte[128 / 8];
            var ret = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: str,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return ret;
        }
    }
}
