using Domain.Domain;
using DR2Plugin.Data.Client;
using DR2Plugin.Implementations.Messaging;
using DR2Plugin.Implementations.Server.SubServers;
using DR2Plugin.Interfaces.Client;
using DR2Plugin.Interfaces.Messaging;
using DR2Plugin.Interfaces.Server;
using DR2Plugin.Interfaces.Services;
using GameCommon;
using MGF.Photon.Implementation.Codes;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    response = HandleUserAlreadyLoggedIn();
                } else {
                    if (!message.Parameters.ContainsKey((byte) MessageParameterCode.LoginName) ||
                        !message.Parameters.ContainsKey((byte) MessageParameterCode.Password)) {
                        response = HandleNotEnoughArguments();
                    } else {
                        var returnCode = authService.IsAuthorized(out var user,
                            (string) message.Parameters[(byte) MessageParameterCode.LoginName],
                            (string) message.Parameters[(byte) MessageParameterCode.Password]);

                        response = returnCode == ReturnCode.Ok
                            ? HandleCorrectRequest(message, subServer, user, returnCode)
                            : HandleInvalidUsernamePassword(returnCode);
                    }
                }
            }
            catch (KeyNotFoundException e) {
                Console.WriteLine($"Caught exception {e.Message}.");
                Console.WriteLine("For keys: 2, 3.");
                Console.WriteLine($"Caught exception {e.StackTrace}");
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

        private Response HandleCorrectRequest(IMessage message, ISubServer subServer, User user, ReturnCode returnCode) {
            // Add our user id to the client peer.
            peer.ClientData<UserData>().Id = user.Id;
            peer.ClientData<UserData>().Heads = user.Souls;
            peer.ClientData<UserData>().Skulls = user.Skulls;
            peer.Connect();

            var response = new Response(Code,
                new Dictionary<byte, object> {
                    {(byte) MessageParameterCode.UserId, user.Id},
                    {(byte) MessageParameterCode.Skulls, user.Souls},
                    {(byte) MessageParameterCode.Souls, user.Skulls},
                    {(byte) MessageParameterCode.SubCodeParameterCode, SubCode}
                }, SubCode, "", (short) returnCode);

            if(subServer.Plugin is ServerPlugin serverPlugin) {
                subServer.DisconnectPeer(peer);
                serverPlugin.GetSubServerOfType<MenuSubServer>().First().ConnectPeer(peer);
            }

            return response;
        }

        private Response HandleNotEnoughArguments() {
            return new Response(Code, new Dictionary<byte, object> {
                {(byte) MessageParameterCode.SubCodeParameterCode, SubCode}
            }, SubCode, "Not enough arguments.", (short) ReturnCode.OperationInvalid);
        }

        private Response HandleUserAlreadyLoggedIn() {
            return new Response(Code, new Dictionary<byte, object> {
                    {(byte) MessageParameterCode.SubCodeParameterCode, SubCode}
                }, SubCode, "User already logged in.",
                (short) ReturnCode.OperationInvalid);
        }
    }
}