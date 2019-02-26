namespace Dionach.ShareAudit.Model
{
    public enum HostState
    {
        New,
        LookingUpPtr,
        CheckingPorts,
        EnumeratingShares,
        AuditingShares,
        NestedAuditingSuspended,
        Complete
    }
}
