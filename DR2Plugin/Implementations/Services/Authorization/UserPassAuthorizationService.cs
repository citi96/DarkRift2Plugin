using System;
using System.Security.Cryptography;
using System.Text;
using Domain.Domain;
using Domain.Mappers;
using DR2Plugin.Interfaces.Services;
using MGF.Photon.Implementation.Codes;

namespace DR2Plugin.Implementations.Services.Authorization {
    public class UserPassAuthorizationService : IAuthorizationService {
        public ReturnCode IsAuthorized(out User user, params string[] authorizationParameters) {
            if (authorizationParameters.Length != 2) {
                user = null;
                return ReturnCode.OperationInvalid;
            }

            user = UserMapper.LoadByUsername(authorizationParameters[0]);
            if (user == null) {
                return ReturnCode.InvalidUserPass;
            }

            // Valid user, check password.
            // Create hash object with SHA512
            var sha512 = SHA256.Create();
            var hashedPsw = sha512.ComputeHash(Encoding.UTF8.GetBytes(authorizationParameters[1]));
            return user.Hash.Equals(Convert.ToBase64String(hashedPsw), StringComparison.OrdinalIgnoreCase)
                ? ReturnCode.Ok
                : ReturnCode.InvalidUserPass;
        }

        public ReturnCode CreateAccount(params string[] authorizationParameters) {
            if (authorizationParameters.Length != 2) {
                return ReturnCode.OperationInvalid;
            }

            var userMapper = new UserMapper();
            var user = UserMapper.LoadByUsername(authorizationParameters[0]);
            if (user == null) {
                var sha512 = SHA256.Create();
                var hashedPsw = sha512.ComputeHash(Encoding.UTF8.GetBytes(authorizationParameters[1]));

                user = new User() {
                    Username = authorizationParameters[0],
                    Hash = Convert.ToBase64String(hashedPsw)
                };
                userMapper.Save(user);
                return ReturnCode.Ok;
            }

            return ReturnCode.InvalidUserPass;
        }
    }
}