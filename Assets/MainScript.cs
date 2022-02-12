using UnityEngine;
using UnityEngine.UI;

using System.Threading.Tasks;

namespace GameEngine
{
    public class MainScript : MonoBehaviour
    {
        [SerializeField] private Button reqButton, signButton;

        //private async void Start() // Automatic Metamask connection & request first wallet address & signature
        //{
        //    while (!MetamaskManager.instance.isConnected) // Wait until connection with Metamask is established
        //        await Task.Yield();

        //    string nonce = "375250"; // Arbitrary message to sign by metamask
        //    string mainAddr = await MetamaskManager.instance.SendRequest(MetamaskManager.RequestType.RequestAccounts);
        //    string signature = await MetamaskManager.instance.SendRequest(MetamaskManager.RequestType.PersonalSign, nonce, mainAddr);

        //    Debug.Log(mainAddr + " | " + signature);
        //}

        private void Start() // Manual Metamask API invocation
        {
            reqButton.onClick.AddListener(async () => await MetamaskManager.instance.SendRequest(MetamaskManager.RequestType.RequestAccounts));
            signButton.onClick.AddListener(async () => await MetamaskManager.instance.SendRequest(MetamaskManager.RequestType.PersonalSign, "375250", "0x0bb152347F982E63e9DEB73Bc0699C0890622E13"));
        }
    }
}