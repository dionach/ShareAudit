namespace Dionach.ShareAudit.Model
{
    public enum FolderEntryState
    {
        New,
        EnumeratingAcls,
        GettingEffectiveAccess,
        EnumerationSuspended,
        EnumeratingFilesystemEntries,
        AuditingFileSystemEntries,
        NestedAuditingSuspended,
        Complete
    }
}
