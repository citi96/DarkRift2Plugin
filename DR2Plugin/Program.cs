using System;
using Domain.Mappers;
using DR2Plugin.Handlers;
using DR2Plugin.Implementations.Services.Authorization;

namespace DataEntities {
    class Program {
        static void Main(string[] args) {
            new UserSaltedPassAuthorizationService().CreateAccount("test", "test");
        }
    }
}
