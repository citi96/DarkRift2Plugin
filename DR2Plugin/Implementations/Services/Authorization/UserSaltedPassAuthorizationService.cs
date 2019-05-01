using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Domain.Domain;
using Domain.Mappers;
using DR2Plugin.Interfaces.Services;
using MGF.Photon.Implementation.Codes;

namespace DR2Plugin.Implementations.Services.Authorization {
    public class UserSaltedPassAuthorizationService : IAuthorizationService {
        public ReturnCode IsAuthorized(out User user, params string[] authorizationParameters) {
            if (authorizationParameters.Length != 2 || authorizationParameters.Contains(string.Empty)) {
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
            //Get the salt from the user and add it to the password passed in.
            var hashedPsw = sha512.ComputeHash(Encoding.UTF8.GetBytes(authorizationParameters[1])
                .Concat(Convert.FromBase64String(user.Salt)).ToArray());
            return user.Hash.Equals(Convert.ToBase64String(hashedPsw), StringComparison.OrdinalIgnoreCase)
                ? ReturnCode.Ok
                : ReturnCode.InvalidUserPass;
        }

        public ReturnCode CreateAccount(params string[] authorizationParameters) {
            if (authorizationParameters.Length != 3 || authorizationParameters.Contains(string.Empty)) {
                return ReturnCode.OperationInvalid;
            }

            var userMapper = new UserMapper();
            var user = UserMapper.LoadByUsername(authorizationParameters[0]);
            if (user == null) {
                var sha512 = SHA256.Create();
                var salt = Guid.NewGuid();
                var hashedPsw = sha512.ComputeHash(Encoding.UTF8.GetBytes(authorizationParameters[1])
                    .Concat(salt.ToByteArray()).ToArray());

                user = new User() {
                    Username = authorizationParameters[0],
                    Hash = Convert.ToBase64String(hashedPsw),
                    Salt = Convert.ToBase64String(salt.ToByteArray()),
                    Email = authorizationParameters[2],
                    Skulls = 100
                };
                userMapper.Save(user);
                return ReturnCode.Ok;
            }

            return ReturnCode.InvalidUserPass;
        }
    }
}