using CloudFlare.BulkDelete;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;

AppConfig? cfg = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText($"{Directory.GetCurrentDirectory()}\\appsettings.json"));
if (cfg is null)
{
    Console.WriteLine("Config couldn't be loaded!");
    return;
}

var api = new CfApi(cfg.AuthEmail, cfg.AuthKey);

Console.WriteLine($"Starting to get zones..");
var allZones = new List<Zone>();
foreach (var domainName in cfg.DomainNames)
{
    Console.WriteLine($"Getting zones for: {domainName}");
    var zonesForDomain = await api.GetZones(domainName);
    allZones.AddRange(zonesForDomain!.Result);
}
Console.WriteLine($"Finished getting zones!");

Console.WriteLine($"Starting to get dns records..");
var allDnsRecords = new List<DnsRecord>();
foreach (var zone in allZones)
{
    Console.WriteLine($"Getting records for zone: {zone.Name}");
    var dnsRecords = await api.GetDnsRecords(zone);
    allDnsRecords.AddRange(dnsRecords!.Result);
}
Console.WriteLine($"Finished getting dns records!");

if (cfg.IpToDelete != "*")
    allDnsRecords = allDnsRecords.Where(x => x.Content == cfg.IpToDelete).ToList();

Console.WriteLine($"Starting to get delete records..");
var deleteTasks = new List<Task>();
foreach (var dnsRecord in allDnsRecords)
{
    deleteTasks.Add(Task.Run(async () =>
    {
        string zoneId = dnsRecord.Zone_Id;
        var localDnsRecord = dnsRecord;
        Console.WriteLine($"Deleting records for zone {localDnsRecord.Zone_Name}: {localDnsRecord.Name}");
        await api.DeleteDnsRecord(zoneId, localDnsRecord);
        Console.WriteLine("Deleted.");
}));
}

await Task.WhenAll(deleteTasks);
Console.WriteLine($"Finished, all done :)");

public class CfApi
{
    private readonly HttpClient http;

    public CfApi(string authEmail, string authKey)
    {
        http = new HttpClient()
        {
            BaseAddress = new Uri("https://api.cloudflare.com/client/v4/")
        };

        http.DefaultRequestHeaders.Add("X-Auth-Email", authEmail);
        http.DefaultRequestHeaders.Add("X-Auth-Key", authKey);

    }

    public async Task<ZonesResponse?> GetZones(string domainName)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["name"] = domainName;
        return await http.GetFromJsonAsync<ZonesResponse>($"zones?{query}");
    }

    public async Task<DnsRecordResponse?> GetDnsRecords(Zone zone)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["per_page"] = "5000";
        return await http.GetFromJsonAsync<DnsRecordResponse>($"zones/{zone.Id}/dns_records?{query}");
    }

    public async Task DeleteDnsRecord(string zoneId, DnsRecord dnsRecord)
    {
        var resp = await http.DeleteAsync($"zones/{zoneId}/dns_records/{dnsRecord.Id}");
        resp.EnsureSuccessStatusCode();
    }

}