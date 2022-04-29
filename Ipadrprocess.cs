using CliWrap;
using System.Net;
using System;

namespace whitelist
{
    public class Ipadrprocess
    {
        public string? result { get; set; }
        public string? ip { get; set; }

        public void Ipadress(HttpContext context, HttpRequest request, string key, string getkey, string mode,string hostname)
        {

            if (key == getkey)
            {

                if (mode == "dns")
                {
                    IPHostEntry adr=Dns.GetHostEntry(hostname);
                    ip=adr.AddressList[0].ToString();
                    result = $"rule update,ip:{ip}";

                }
                else
                {
                    if (request.Headers.ContainsKey("CF-Connecting-IP"))
                    {
                        string ip = request.Headers["CF-Connecting-IP"].ToString();

                        Cli.Wrap("iptables")
                            .WithArguments($"-t filter -A WHITELIST -dport {ip} -j ACCEPT")
                            .ExecuteAsync();
                        result = $"rule update,ip:{ip}";


                    }
                    else
                    {
                        string? ip = Convert.ToString(context.Connection.RemoteIpAddress);

                        Cli.Wrap("iptables")
                            .WithArguments($"-A WHITELIST -dport {ip} -j ACCEPT")
                            .ExecuteAsync();
                        result = $"rule update,ip:{ip}";
                    };
                };
            }
            else
            {
                result = null;
            };


            }
        }
}
