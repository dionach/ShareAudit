namespace Dionach.ShareAudit.Model
{
    public enum FileEntryState
    {
        New,
        EnumeratingAcls,
        GettingEffectiveAccess,
        ReadingHead,
        Complete
    }
}
