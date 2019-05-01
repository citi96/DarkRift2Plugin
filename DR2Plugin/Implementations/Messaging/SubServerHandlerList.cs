using System;
using System.Collections.Generic;
using System.Linq;
using DR2Plugin.Handlers;
using DR2Plugin.Interfaces.Client;
using DR2Plugin.Interfaces.Messaging;
using DR2Plugin.Interfaces.Server;

namespace DR2Plugin.Implementations.Messaging {
    public class SubServerHandlerList : IHandlerList<ISubServer> {
        private readonly List<AbstractSubServerHandler> requestSubCodeHandlerList;
        private readonly List<AbstractSubServerHandler> requestCodeHandlersList;
        public IClientPeer Peer { private get; set; }

        public SubServerHandlerList(IEnumerable<AbstractSubServerHandler> handlers) {
            requestCodeHandlersList = new List<AbstractSubServerHandler>();
            requestSubCodeHandlerList = new List<AbstractSubServerHandler>();

            foreach (var handler in handlers) {
                RegisterHandler(handler);
            }
        }

        public bool RegisterHandler(IHandler<ISubServer> handler) {
            bool registered = false;
            var subServerHandler = (AbstractSubServerHandler) handler;

            if ((handler.Type & MessageType.Request) == MessageType.Request) {
                if (handler.SubCode.HasValue) {
                    requestSubCodeHandlerList.Add(subServerHandler);
                    registered = true;
                } else {
                    requestCodeHandlersList.Add(subServerHandler);
                }
            }

            return registered;
        }

        public bool HandleMessage(IMessage message, ISubServer serverPeer) {
            bool handled = false;

            switch (message.Type) {
                case MessageType.Request:
                    // Get all matching code and subCode - Normal message handling
                    var handlers = requestSubCodeHandlerList.Where(h =>
                        h.Code == message.Code && h.SubCode == message.SubCode);
                    var handlersList = handlers.ToList();

                    // If no normal message handling occurs - check if there is one that handles only by code- normal Forward handling
                    if (!handlersList.Any()) {
                        handlersList = requestSubCodeHandlerList.Where(h => h.Code == message.Code).ToList();
                    }

                    // If still no message handling occurs - Default handler
                    if (!handlersList.Any()) {
                        // No default handler for incoming client request. Usually output error message on server.
                        //defaultRequestHandler.handleMessage(message, clientPeer);
                        Console.WriteLine($"Failed to handle message {message.Code} - {message.SubCode}");
                    }

                    // If the default handler was called, its because the handler list was empty. otherwise we call all matching handlers
                    foreach (var handler in handlersList) {
                        handler.HandleMessage(message, serverPeer, Peer);
                        handled = true;
                    }

                    break;
                case MessageType.Response:
                    break;
                case MessageType.Async:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return handled;
        }
    }
}