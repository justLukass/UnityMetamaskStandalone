const ws = new WebSocket("ws://localhost/Connection"); // The "/Connection" service name here has to match with the websocket's server
var  web3;

ws.onopen = async () => // Fired once connection with local Unity websocket server is established
{
  if (window.ethereum) // Check if metamask is installed on user's browser
  {
    web3 = new Web3(window.ethereum);
    ws.send("mmOk");
  }
  else 
  {
    ws.send("mmErr"); // Metamask is not present
    alert("Metamask has not been detected. This tab will now be automatically closed.");
    close();
  }
};

ws.onmessage = (msg) => // Fired when data is received from local Unity websocket server
{
  const args = msg.data.split("|");
  
  switch (args[0]) // Definition of custom arguments for any metamask API function call(s)
  {
    case "requestAccounts":
      web3.eth.requestAccounts().then((data) => ws.send(data[0])); // Returns the first active account address
      break;
      
    case "personalSign":
      web3.eth.personal.sign(args[1], args[2], "sample375").then((data) => ws.send(data));
      break;
  }
};