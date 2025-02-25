﻿using Core.Entities.Concrete;
using Core.Extensions;
using Core.Utilities.Security.Encryption;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Core.Utilities.Security.JWT
{
	public class JwtHelper : ITokenHelper
	{
		public IConfiguration Configuration { get; set; }
		TokenOptions _tokenOptions;
		DateTime _accessTokenExpiration;
		public JwtHelper(IConfiguration configuration)
		{
			Configuration = configuration;
			_tokenOptions = new TokenOptions 
			{ 
				Audience = "turan@turan.com", 
				Issuer = "turan@turan.com", 
				AccessTokenExpiration = 10,
				SecurityKey = "mysupersecretkeymysupersecretkey" 
			};
		}

		public AccessToken CreateToken(User user, List<OperationClaim> operationClaims)
		{
			_accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);

			var securityKey = SecurityKeyHelper.CreatrSecurityKey(_tokenOptions.SecurityKey);
			var signingCredentials = SigningCredentialsHelper.CreateSignIngCredentials(securityKey);
			var jwt = CreateJwtSecurityToken(_tokenOptions, user, signingCredentials, operationClaims);
			var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
			var token = jwtSecurityTokenHandler.WriteToken(jwt);

			return new AccessToken { Token = token, Expiration = _accessTokenExpiration };
		}

		public JwtSecurityToken CreateJwtSecurityToken(TokenOptions tokenOptions, User user, SigningCredentials signingCredentials, List<OperationClaim> operationClaims)
		{
			var jwt = new JwtSecurityToken
			(
				issuer: tokenOptions.Issuer,
				audience: tokenOptions.Audience,
				expires: _accessTokenExpiration,
				notBefore: DateTime.Now,
				claims: SetClaims(user, operationClaims),
				signingCredentials: signingCredentials
			);
			return jwt;
		}

		public IEnumerable<Claim> SetClaims(User user, List<OperationClaim> operationClaims)
		{
			var claims = new List<Claim>();
			claims.AddEmail(user.Email);
			claims.AddName($"{user.FirstName} {user.LastName}");
			claims.AddNameIdentifier(user.Id.ToString());
			claims.AddRoles(operationClaims.Select(c => c.Name).ToArray());
			return claims;
		}
	}
}
