using System.Diagnostics;
using CliWrap;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");

app.MapGet("/getip", (HttpContext context, HttpRequest request, HttpResponse response) =>
{

    string ip = Convert.ToString(value: context.Connection.RemoteIpAddress);
    string rule = $"iptables -A INPUT -s {ip} -j ACCEPT";
    string notice = "rule update success,";
    var result = Cli.Wrap("iptables")
    .WithArguments($"-A INPUT -s {ip} -j ACCEPT")
    .ExecuteAsync();
    //var psi = new System.Diagnostics.ProcessStartInfo(rule);
    //psi.UseShellExecute = false;
    //psi.FileName = "command";
    //System.Diagnostics.Process.Start(psi);
    Console.Write(ip);
    return ip + notice+ rule;
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






app.Run();

