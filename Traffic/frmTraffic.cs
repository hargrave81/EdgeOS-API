using EdgeOS.API.Core;
using EdgeOS.API.Core.Types;
using EdgeOS.API.Core.Types.SubscriptionRequests;
using EdgeOS.API.Core.Types.SubscriptionResponses;
using System.Configuration;
using System.Linq;

namespace Traffic
{
    public partial class frmTraffic : Form
    {
        private StatsConnection statsConnection = null;

        public frmTraffic()
        {
            InitializeComponent();
            this.propTraffic.SelectedObject = new Dictionary<string, Traffic>();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (ConfigurationManager.AppSettings["username"] == null || ConfigurationManager.AppSettings["password"] == null || ConfigurationManager.AppSettings["host"] == null)
            {
                MessageBox.Show("Program cannot start, some credentials were missing in the program's configuration file.", "Missing Credentials", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            // The WebClient allows us to get a valid SessionID to then use with the StatsConnection.
            using (WebClient webClient = new WebClient(ConfigurationManager.AppSettings["username"], ConfigurationManager.AppSettings["password"], "https://" + ConfigurationManager.AppSettings["host"] + "/"))
            {
                // Login to the router.
                webClient.Login();


                statsConnection = new StatsConnection();
                // Share a valid SessionID with the StatsConnection object.
                statsConnection.SessionID = webClient.SessionID;

                // Ignore TLS certificate errors if there is a ".crt" file present that matches this host.
                statsConnection.AllowLocalCertificates();

                // Connect to the router.
                statsConnection.ConnectAsync(new Uri("wss://" + ConfigurationManager.AppSettings["Host"] + "/ws/stats"));

                // Setup an event handler for when data is received.
                statsConnection.DataReceived += Connection_DataReceived;

                // Setup an event handler for when the connection state changes.
                statsConnection.ConnectionStatusChanged += Connection_ConnectionStatusChanged;
            }
        }

        /// <summary>Method which when a StatsConnection is established requests Interface statistics.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="connectionStatus">The <see cref="StatsConnection"/>'s new <see cref="StatsConnection.ConnectionStatus"/>.</param>
        private void Connection_ConnectionStatusChanged(object sender, StatsConnection.ConnectionStatus connectionStatus)
        {
            // The sender should be a StatsConnection so that we can interact with that StatsConnection instance.
            StatsConnection statsConnection = sender as StatsConnection;
            if (statsConnection == null) { return; }

            // Specifically what did the ConnectionStatus change to?
            switch (connectionStatus)
            {
                // It was previously not connected and now it is.
                case StatsConnection.ConnectionStatus.Connected:

                    // Compose a subscription request message.
                    SubscriptionRequest subscriptionRequest = new SubscriptionRequest
                    {
                        Subscribe = new Subscription[] { new Subscription() { name = SubscriptionMessageType.TrafficAnalysis } },
                        SessionID = statsConnection.SessionID
                    };

                    // Ask for events to be delivered.
                    statsConnection.SubscribeForEvents(subscriptionRequest);
                    break;
            }
        }

        /// <summary>Method which is invoked when new <see cref="SubscriptionDataEvent"/> arrives.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SubscriptionDataEvent"/> instance containing the event data.</param>
        private void Connection_DataReceived(object sender, SubscriptionDataEvent e)
        {
            // Ignore any data that isn't an Interfaces response message.
            if (e.rootObject.GetType() != typeof(TrafficAnalysisResponse)) { return; }

            TrafficAnalysisResponse analysisRoot = (TrafficAnalysisResponse)e.rootObject;

            Dictionary<string,Traffic> result = (Dictionary<string, Traffic>)this.propTraffic.SelectedObject;
            var removals = result.Keys.Where(t => !analysisRoot.TrafficAnalysis.Keys.Contains(t));
            foreach (var key in removals)
            {
                result.Remove(key);
            }
            float rxRate = 0;
            float txRate = 0;
            long txBytes = 0;
            long rxBytes = 0;
            foreach(string host in analysisRoot.TrafficAnalysis.Keys)
            {
                var cRec = analysisRoot.TrafficAnalysis[host];
                string hostName = host;
                Traffic record = null;
                if (result.ContainsKey(host))
                {
                    record = result[host];
                }
                else
                {
                    record = new Traffic() { Host = hostName };
                    result.Add(hostName,record);
                }
                record.SetApps(cRec);
                rxRate += record.RxRate;
                txRate += record.TxRate;
                txBytes += record.TxBytes;
                rxBytes += record.RxBytes;
            }            
            // get totals to calculate percentages
            foreach(var entry in result)
            {
                // calc percentages
                entry.Value.CalcPercent(rxRate, txRate, rxBytes, txBytes);
            }
            this.propTraffic.SelectedObject = result;
            this.propTraffic.Invalidate();
        }
    }
}