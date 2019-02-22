using Core.EsahaPlus.Dtos;
using EsahaPlusApi.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.DirectoryServices;
using System.Security.Claims;

namespace EsahaPlusApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppSettings _appSettings;
        private readonly IJsonService _jsonService;

        public AuthService(IOptions<AppSettings> appSettings, IJsonService jsonService)
        {
            _appSettings = appSettings.Value;
            _jsonService = jsonService;
        }

        public UserDto Authenticate(UserDto user)
        {
            bool isAuthenticated = CheckAuthentication(_appSettings.Domain, user.Username, user.Password);
            if (!isAuthenticated)
                return null;

            /// TODO: Get user info from Repository pattern
            var _user = new UserDto() { Id = "00112233", FirsName = "Barış", LastName = "Usanmaz", Username = "busanmaz", Token = "secret token" };
            //var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (_user == null)
                return null;

            _user.Token = _jsonService.GetToken(
                _appSettings.Secret,
                new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, _user.Id),
                    new Claim(ClaimTypes.Name, _user.Username)
                });

            // remove password before returning
            _user.Password = null;

            return _user;
        }

        private bool CheckAuthentication(string domain, string username, string password)
        {
            try
            {
                string domainUsername = $@"{domain}\{username}";
                DirectoryEntry entry = new DirectoryEntry(_appSettings.LdapPath, domainUsername, password);

                //Bind to the native AdsObject to force authentication.
                object obj = entry.NativeObject;

                DirectorySearcher search = new DirectorySearcher(entry);

                search.Filter = "(SAMAccountName=" + username + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();

                if (result == null)
                {
                    return false;
                }

                //Update the new path to the user in the directory.
                string _path = result.Path;
                string _filterAttribute = (string)result.Properties["cn"][0];

                return true;
            }
            catch (Exception ex)
            {
                /// TODO: save exception log
                return false;
            }
        }
    }
}
