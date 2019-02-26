namespace Dionach.ShareAudit.Modules.Services
{
    public interface ISidUtilitiesService
    {
        string SidStringToAccountName(string host, string sid);
    }
}
