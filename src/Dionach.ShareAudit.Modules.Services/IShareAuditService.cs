using Dionach.ShareAudit.Model;
using System;

namespace Dionach.ShareAudit.Modules.Services
{
    public interface IShareAuditService
    {
        event EventHandler Started;

        event EventHandler Stopped;

        void StartAudit(Project project);

        void StopAudit();
    }
}
