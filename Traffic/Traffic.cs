using EdgeOS.API.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traffic
{
    public class Traffic
    {

        public Traffic()
        {
            Applications = new Dictionary<string, TrafficDetail>();
        }

        public float Percent { get; set; }
        public float TxRate => Applications.Sum(t => t.Value.TxRate);
        public float RxRate => Applications.Sum(t => t.Value.RxRate);
        public int TxBytes => Applications.Sum(t => t.Value.TxBytes);
        public int RxBytes => Applications.Sum(t => t.Value.RxBytes);
        public string Host { get; set; }

        public Dictionary<string, TrafficDetail> Applications { get; set; }

        public void CalcPercent(float rxRate, float txRate, long rxBytes, long txBytes)
        {
            // percent is based on total bytes going through
            Percent = (float)(this.TxBytes + this.RxBytes) / (float)(rxBytes + txBytes) * 100f;
        }

        public void SetApps(Dictionary<string, ApplicationStats>? input)
        {
            if(input == null || input.Count == 0)
            {
                Applications.Clear();
                return;
            }
            //remove records
            var removal  = Applications.Keys.Where(t => !input.Keys.Contains(t));
            foreach(var key in removal)
            {
                Applications.Remove(key);
            }
            foreach(var key in input.Keys)
            {
                TrafficDetail entry;
                if(Applications.ContainsKey(key))
                {
                    entry = Applications[key];
                }
                else
                {
                    entry = new TrafficDetail() { AppName = key };
                    Applications.Add(key, entry);
                }
                entry.RxBytes = (int)input[key].rx_bytes;
                entry.TxBytes = (int)input[key].tx_bytes;
                entry.RxRate = (float)input[key].rx_rate;
                entry.TxRate = (float)input[key].tx_rate;
            }            
        }

        public override string ToString()
        {
            return $"{Host}  {Percent.ToString("##0.0")}%";
        }
    }

    public class TrafficDetail
    {
        public float TxRate { get; set; }
        public float RxRate { get; set; }
        public int TxBytes { get; set; }
        public int RxBytes { get; set; }
        public string AppName { get; set; }
    }
}
