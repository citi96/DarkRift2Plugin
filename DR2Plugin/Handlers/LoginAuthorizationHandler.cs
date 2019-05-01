using Domain.Domain;
using DR2Plugin.Data.Client;
using DR2Plugin.Implementations.Messaging;
using DR2Plugin.Interfaces.Client;
using DR2Plugin.Interfaces.Messaging;
using DR2Plugin.Interfaces.Services;
using GameCommon;
using MGF.Photon.Implementation.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using DR2Plugin.Interfaces.Server;

namespace DR2Plugin.Handlers {
    public class LoginAuthorizationHandler : AbstractSubServerHandler {
        private readonly IAuthorizationService authService;

        public override MessageType Type => MessageType.Request;
        public override byte Code => (byte) MessageOperationCode.Login;
        public override int? SubCode => (int?) MessageSubCode.LoginUserPass;

        public LoginAuthorizationHandler(IAuthorizationService authService) {
            this.authService = authService;
        }

        public override bool HandleMessage(IMessage message, ISubServer subServer) {
            var connectionCollection = subServer.ConnectionCollection;
            Response response;

            try {
                if (connectionCollection.GetPeers<IClientPeer>().FirstOrDefault(p => p == peer) == default) {
                    response = HandleUserAlreadyLoggedIn(message);
                } else {
                    if (!message.Parameters.ContainsKey((byte) MessageParameterCode.LoginName) ||
                        !message.Parameters.ContainsKey((byte) MessageParameterCode.Password)) {
                        response = HandleNotEnoughArguments(message);
                    } else {
                        var returnCode = authService.IsAuthorized(out var user,
                            (string) message.Parameters[(byte) MessageParameterCode.LoginName],
                            (string) message.Parameters[(byte) MessageParameterCode.Password]);

                        response = returnCode == ReturnCode.Ok
                            ? HandleCorrectRequest(message, peer, user, returnCode)
                            : HandleInvalidUsernamePassword(returnCode);
                    }
                }
            }
            catch (KeyNotFoundException e) {
                Console.WriteLine($"Catch exception {e.Message}.");
                Console.WriteLine($"For keys: 2, 3.");
                Console.WriteLine($"Catch exception {e.StackTrace}");
                return false;
            }

            peer.SendMessage(response);
            return true;
        }


        private Response HandleInvalidUsernamePassword(ReturnCode returnCode) {
            var response = new Response(Code, new Dictionary<byte, object>(), SubCode,
                "Invalid username or password.",
                (short) returnCode);
            return response;
        }

        private Response HandleCorrectRequest(IMessage message, IClientPeer peer, User user, ReturnCode returnCode) {
            peer.Connect();
            // Add our user id to the client peer.
            peer.ClientData<UserData>().Id = user.Id;
            peer.ClientData<UserData>().Heads = user.Souls;
            peer.ClientData<UserData>().Skulls = user.Skulls;

            var response = new Response(Code,
                new Dictionary<byte, object> {
                    {(byte) MessageParameterCode.UserId, user.Id},
                    {(byte) MessageParameterCode.Skulls, user.Souls},
                    {(byte) MessageParameterCode.Souls, user.Skulls},
                    {(byte) MessageParameterCode.SubCodeParameterCode, SubCode}
                }, SubCode, "", (short) returnCode);
            return response;
        }

        private Response HandleNotEnoughArguments(IMessage message) {
            return new Response(Code, new Dictionary<byte, object> {
                {(byte) MessageParameterCode.SubCodeParameterCode, SubCode}
            }, SubCode, "Not enough arguments.", (short) ReturnCode.OperationInvalid);
        }

        private Response HandleUserAlreadyLoggedIn(IMessage message) {
            return new Response(Code, new Dictionary<byte, object> {
                    {(byte) MessageParameterCode.SubCodeParameterCode, SubCode}
                }, SubCode, "User already logged in.",
                (short) ReturnCode.OperationInvalid);
        }
    }
}