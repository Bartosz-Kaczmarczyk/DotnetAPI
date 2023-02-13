using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Data;

    public class AuthHelper
    {
        private readonly IConfiguration _config;
        private readonly DataContextEF _entity;

        public AuthHelper(IConfiguration config)
        {
            _config = config;
            _entity = new DataContextEF(config);
        }

        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value +
                Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            );
        }

        public string CreateToken(int UsedId)
        {
            Claim[] claims = new Claim[]
            {
                new Claim("userId", UsedId.ToString())
            };

            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(tokenKeyString != null ? tokenKeyString : ""));

            SigningCredentials credentials = new SigningCredentials(tokenKey,
                SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(7)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }

        // public bool SetPassword(UserForLoginDto userForSetPasswrd)
        // {

        //     byte[] passwordSalt = new byte[128 / 8];
        //     using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        //     {
        //         rng.GetNonZeroBytes(passwordSalt);
        //     }

        //     byte[] passwordHash = GetPasswordHash(userForSetPasswrd.Password, passwordSalt);

        //     string sqlAddAuth = @"EXEC TutorialAppSchema.spRegistration_Upsert 
        //                                 @Email = @EmailParam, 
        //                                 @PasswordHash = @PasswordHashParam,
        //                                 @PasswordSalt = @PasswordSaltParam";

        //     DynamicParameters sqlParameters = new DynamicParameters();

        //     sqlParameters.Add("@EmailParam", userForSetPasswrd.Email, DbType.String);
        //     sqlParameters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
        //     sqlParameters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);

        //     // List<SqlParameter> sqlParameters = new List<SqlParameter>();

        //     // SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.VarChar);
        //     // emailParameter.Value = userForSetPasswrd.Email;
        //     // sqlParameters.Add(emailParameter);

        //     // SqlParameter passwordHashParameter = new SqlParameter("@PasswordHashParam", SqlDbType.VarBinary);
        //     // passwordHashParameter.Value = passwordHash;
        //     // sqlParameters.Add(passwordHashParameter);

        //     // SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSaltParam", SqlDbType.VarBinary);
        //     // passwordSaltParameter.Value = passwordSalt;
        //     // sqlParameters.Add(passwordSaltParameter);

        //     return _dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters);

        // }
    }
