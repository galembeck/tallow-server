using Domain.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Public.Configuration;

public static class JwtInitializer
{
    public static void ConfigureJwt(this IServiceCollection services)
    {
        byte[] key = Encoding.ASCII.GetBytes(Constant.Settings.JwtSettings.SecretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                // ValidIssuer = Constant.Settings.JwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = Constant.Settings.JwtSettings.Audience
            };
        });



        //services.AddAuthentication(options =>
        //{
        //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //})
        //.AddJwtBearer(options =>
        //{
        //    options.TokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuer = true,
        //        ValidateAudience = true,
        //        ValidateLifetime = true,
        //        ValidateIssuerSigningKey = true,
        //        ValidIssuer = Constant.Settings.JwtSettings.Issuer,
        //        ValidAudience = Constant.Settings.JwtSettings.Audience,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constant.Settings.JwtSettings.SecretKey)),
        //        ClockSkew = TimeSpan.Zero
        //    };
        //});
    }
}