using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using CliWrap;
using whitelist;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Host.UseConsoleLifetime();


var app = builder.Build();

//app.Lifetime.ApplicationStopped.Register(() =>
//{
//    Cli.Wrap("iptables").WithArguments("-t filter -F WHITELIST").ExecuteAsync();
//    Cli.Wrap("iptables").WithArguments("-D INPUT -j WHITELIST").ExecuteAsync();
//    Cli.Wrap("iptables").WithArguments("-t filter -X WHITELIST").ExecuteAsync();
//});

if (!File.Exists(Directory.GetCurrentDirectory()+"/whitelist-key"))
{
    Console.WriteLine("请创建相应的配置文件，详情请参考本项目github readme文档。");
    System.Environment.Exit(0);
};
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
builder.Configuration.AddIniFile("whitelist-key");
Console.WriteLine(Directory.GetCurrentDirectory());
Regex regExp = new Regex("[ \\[ \\] \\^ \\-_*×――(^)$%~!＠@＃#$…&%￥—+=<>《》!！??？:：•`·、。，；,.;/\'\"{}（）‘’“”-]");
string getkey = builder.Configuration["Key"];
if (regExp.IsMatch(getkey))
{
    Console.WriteLine("key中包含特殊字符，会引起错误，请重新编辑配置文件。");
    System.Environment.Exit(0);
}
if (getkey == null)
{
    Console.WriteLine("key为必须参数，请重新设置。");
    System.Environment.Exit(0);
}
    string port = builder.Configuration["Port"];
app.Logger.LogInformation(getkey);
//string sshport = builder.Configuration.GetConnectionString("SshPort");
string listenport = builder.Configuration["ListenPort"];
string ServicePath = "/etc/systemd/system/whitelist.service";
string ?Pathstring = Environment.ProcessPath;

if (port==null)
{
    Console.WriteLine("请填写配置文件的所有节点。");
    System.Environment.Exit(0);
};
if (Pathstring==null)
{
    Console.WriteLine("获取程序路径错误，请重启程序尝试。");
    System.Environment.Exit(0);
};
if (!File.Exists(ServicePath))
{


    var iptableStarter = Cli.Wrap("systemctl")
    .WithArguments("start iptables")
    .ExecuteAsync();
    //var iptablesRuleFlush = Cli.Wrap("iptables") //Reset iptables rules.
    //    .WithArguments("-F")
    //    .ExecuteAsync();
    //Cli.Wrap("iptables -A INPUT -i lo -j ACCEPT").ExecuteAsync();
    //Cli.Wrap("iptables -A INPUT -m state --state ESTABLISHED,RELATED -j ACCEPT").ExecuteAsync();
    //Cli.Wrap($"iptables -A INPUT -p tcp --dport {sshport} -j ACCEPT").ExecuteAsync();
    //Cli.Wrap("iptables -A INPUT -p tcp --dport 5000 -j ACCEPT").ExecuteAsync();

    var iptablesRuleset = Cli.Wrap("iptables")  //Create iptables custom chain.
        .WithArguments("-t filter -N WHITELIST")
        .ExecuteAsync();
    var iptablesChainbind = Cli.Wrap("iptables") //Bind the whitelist custom chain to the input chain.
        .WithArguments($"-I INPUT -p tcp--dport {port} -j WHITELIST")
        .ExecuteAsync();
    var iptablesChainbind1 = Cli.Wrap("iptables") //Bind the whitelist custom chain to the input chain.
        .WithArguments($"-I INPUT -p udp--dport {port} -j WHITELIST")
        .ExecuteAsync();
    var iptablsDrop = Cli.Wrap("iptables")
        .WithArguments($"-t filter -I WHITELIST -j DROP")
        .ExecuteAsync();


    Console.WriteLine("程序初始化开始。");
    FileStream fs = File.Create(ServicePath);
    fs.Close();
    var sw = new StreamWriter(ServicePath);
    var wd = Environment.CurrentDirectory;
    sw.WriteLine("[Unit]" + "\n" + "Description=whitelist app" + "\n" + "After=network-online.target" + "\n" + "[Service]" + "\n" + "Type=simple" + "\n" + $"WorkingDirectory={wd}"+"\n"+ $"ExecStart={Pathstring} --urls=http://0.0.0.0:{listenport} --ASPNETCORE_ENVIRONMENT=Production" + "\n" + "Restart=always" + "\n" + "RestartSec=5" + "\n" + "StartLimitInterval=0" + "\n" + "[Install]" + "\n" + "WantedBy=multi-user.target");
    sw.Close();
    var reload = Cli.Wrap("systemctl")
    .WithArguments("daemon-reload")
    .ExecuteAsync();
    //var start = Cli.Wrap("systemctl")
    //.WithArguments("start whitelist.service")
    //.ExecuteAsync();
    //var boot = Cli.Wrap("systemctl")
    //.WithArguments("enable whitelist.service")
    //.ExecuteAsync();
    Console.WriteLine("已添加系统服务，程序已自动重启，访问监听端口即可。");
    System.Environment.Exit(0);
};

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



//app.MapGet("/", () => "Hello World!");

//app.MapGet("/getip", (HttpContext context, HttpRequest request, HttpResponse response) =>
//{

//    string ip = Convert.ToString(value: context.Connection.RemoteIpAddress);
//    string rule = $"iptables -A INPUT -s {ip} -j ACCEPT";
//    string notice = "rule update success,";
//    var result = Cli.Wrap("iptables")
//    .WithArguments($"-A INPUT -s {ip} -j ACCEPT")
//    .ExecuteAsync();
//    //var psi = new System.Diagnostics.ProcessStartInfo(rule);
//    //psi.UseShellExecute = false;
//    //psi.FileName = "command";
//    //System.Diagnostics.Process.Start(psi);
//    Console.Write(ip);
//    return ip + notice+ rule;
//});

app.MapGet("/getip", (HttpContext context, HttpRequest request, HttpResponse response) =>
{
    string getkey = builder.Configuration["Key"];
    string port = builder.Configuration["Port"];
    var key=request.Query["key"];
    string ?mode=request.Query["mode"];
    string ?hostname=request.Query["host"];
    string result;    
    if (key == getkey)
    {

        if (mode == "dns")
        {
            IPHostEntry adr = Dns.GetHostEntry(hostname);
            
            result = $"rule update,ip:{adr.AddressList[0].ToString()}";
            iptablestransfer(adr.AddressList[0].ToString(),port);

        }
        else
        {
            if (request.Headers.ContainsKey("CF-Connecting-IP"))
            {
                                
                result = $"rule update,ip:{request.Headers.ContainsKey("CF-Connecting-IP").ToString()}";
                iptablestransfer(request.Headers.ContainsKey("CF-Connecting-IP").ToString(),port);

            }
            else
            {                
                result = $"rule update,ip:{Convert.ToString(context.Connection.RemoteIpAddress)}";
                iptablestransfer(Convert.ToString(context.Connection.RemoteIpAddress),port);
            };
        };
    }
    else
    {
        result = null;
    };




    //Ipadrprocess ipadr = new Ipadrprocess();

    //ipadr.Ipadress(context, request,key,getkey,mode,hostname);
    //if (ipadr.result==null)
    //{
    //    Results.StatusCode(503);
    //}
    //else
    //{
    //    Results.Content($"rule update,{ipadr.ip}");
    //}
    return result==null? Results.StatusCode(444): Results.Content(result);
});

app.MapGet("/randnumber", () =>
{
    var rand = System.Security.Cryptography.RandomNumberGenerator.Create();
    byte[] bytes = new byte[32];
    rand.GetBytes(bytes);
    string number=Convert.ToBase64String(bytes);
    return number;
}
);

void iptablestransfer(string ip,string port)
{
    Cli.Wrap("iptables")
                    .WithArguments($"-t filter -A WHITELIST -s {ip} -j ACCEPT")
                    .ExecuteAsync();
}


app.Run();
