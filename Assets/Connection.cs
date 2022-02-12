using WebSocketSharp;
using WebSocketSharp.Server;

namespace GameEngine
{
    public class Connection : WebSocketBehavior
    {
        private MetamaskManager mmManager;

        public Connection(MetamaskManager mmManager)
        {
            this.mmManager = mmManager;
        }

        /// <summary>
        /// Invoked when message is received from browser (Metamask)
        /// </summary>
        protected override void OnMessage(MessageEventArgs e)
        {
            mmManager.ReceiveResponse(e.Data);
        }

        /// <summary>
        /// Invoked when websocket connection closes. This happens when user closes the opened browser tab.
        /// </summary>
        protected override void OnClose(CloseEventArgs e)
        {
            mmManager.ReceiveResponse("onClose");
        }
    }
}