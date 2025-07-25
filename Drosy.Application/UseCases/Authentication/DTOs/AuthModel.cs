﻿using System.Text.Json.Serialization;

namespace Drosy.Application.UsesCases.Authentication.DTOs
{
    public class AuthModel
    {
        public string UserName { get; set; }
        public int UserId { get; set; }
        public string AccessToken { get; set; }
        public List<string> Roles { get; set; } = new();
        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
