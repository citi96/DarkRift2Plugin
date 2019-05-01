using Domain.Domain;
using MGF.Photon.Implementation.Codes;

namespace DR2Plugin.Interfaces.Services {
    public interface IAuthorizationService {
        ReturnCode IsAuthorized(out User user, params string[] authorizationParameters);
        ReturnCode CreateAccount(params string[] authorizationParameters);
    }
}