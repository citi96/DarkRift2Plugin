using Domain.Domain;
using DR2Plugin.Interfaces.Services;
using MGF.Photon.Implementation.Codes;
using System;
using System.Threading.Tasks;
using Domain.Mappers;

namespace DR2Plugin.Implementations.Services.Authorization {
    public class AsyncAuthorizationService : IAuthorizationService {
        public ReturnCode IsAuthorized(out User user, params string[] authorizationParameters) {
            var returnCode = ReturnCode.OperationInvalid;

            if(authorizationParameters.Length != 2) {
                throw new ArgumentOutOfRangeException();
            }

            user = UserMapper.LoadByUsername(authorizationParameters[0]);
            if(user == null) {
                return ReturnCode.InvalidUserPass;
            }

            bool isAuthorized = AsyncIsAuthorized(authorizationParameters[0], authorizationParameters[1]).Result;
            returnCode = isAuthorized ? ReturnCode.Ok : ReturnCode.InvalidUserPass;

            return returnCode;
        }

        private static async Task<bool> AsyncIsAuthorized(string username, string password) {
            // Create a task to actually run in a separate
            bool isAuthorized = await Task.Run(() => OAuthIsAuthorized(username, password));
            return isAuthorized;

        }

        private static bool OAuthIsAuthorized(string username, string password) {
            // Run OAuth specific code
            // Run in a separate thread in the thread pool
            return false;
        }

        public ReturnCode CreateAccount(params string[] authorizationParameters) {
            return ReturnCode.InternalServerError;
        }
    }
}
