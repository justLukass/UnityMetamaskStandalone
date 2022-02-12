using UnityEngine;

using WebSocketSharp.Server;

using System.Threading.Tasks;

namespace GameEngine
{
    public class MetamaskManager : Singleton<MetamaskManager>
    {
        public enum RequestType : int
        {
            RequestAccounts, PersonalSign
        }

        private const string CONNECTION = "Connection";
        private string[] requests = { "requestAccounts", "personalSign" }; // These request types strings have to match ones in the Javascript code
        private string response;

        [SerializeField] private string webUrl;

        public bool isConnected { get; private set; }

        private WebSocketServer wsServer;
        private WebSocketSessionManager session;

        private void Awake()
        {
            // Create & start local web socket server
            this.wsServer = new WebSocketServer("ws://127.0.0.1");
            this.wsServer.AddWebSocketService("/" + CONNECTION, () => new Connection(this));
            this.wsServer.Start();

            // Create service to which clients can connect to
            this.session = wsServer.WebSocketServices["/" + CONNECTION].Sessions;

            if (!wsServer.IsListening)
                Debug.LogError("Error in MetamaskManager.Awake(): WebSocketServer is not listening.");
            else
                Application.OpenURL(webUrl); // Your website URL here
        }

        private void OnDestroy()
        {
            wsServer.Stop(); // Clean after game ends
        }

        private void OnGUI() // Internal debug output
        {
            GUI.Label(new Rect(10, 10, 1000, 1000), "isConnected: " + isConnected, new GUIStyle() { fontSize = 40, normal = { textColor = Color.white } });
            GUI.Label(new Rect(10, 100, 1000, 1000), "response: " + response, new GUIStyle() { fontSize = 40, normal = { textColor = Color.white } });
        }

        /// <summary>
        /// Use this method to send any request to Metamask
        /// </summary>
        /// <param name="requestType">Type of request you would like to make</param>
        /// <param name="requestArgs">Any additional request arguments, can be left empty</param>
        /// <returns>Response from Metamask</returns>
        public async Task<string> SendRequest(RequestType requestType, params string[] requestArgs)
        {
            response = null;

            if (isConnected)
            {
                string request = requests[(int)requestType].ToString(); // Assign 'RequestType' to a string

                foreach (string ARGS in requestArgs) // Pack all arguments into single string separated by '|' character
                    request += "|" + ARGS;

                session.Broadcast(request);

                while (response == null) // Wait until 'ReceiveResponse(string)' is called
                    await Task.Yield();
            }
            else Debug.LogError("Error in MetamaskManager.SendRequest(RequestType): WebSocket connection isn't established.");

            return response;
        }

        /// <summary>
        /// This method is called whenever there's a response from Metamask so don't call it directly.
        /// </summary>
        public void ReceiveResponse(string response)
        {
            switch (response)
            {
                case "mmOk": // Metamask has been detected & connection is established
                    isConnected = true;
                    break;

                case "mmErr": // Metamask is not istalled on user's browser
                    Debug.LogError("Error in MetamaskManager.ReceiveResponse(string): Metamask is not installed.");
                    break;

                default: // Other responses
                    this.response = response;
                    break;
            }
        }
    }
}