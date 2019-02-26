# What is share auditing?

Share auditing is the process of reviewing file shares within an Active Directory environment in order to determine whether any content is inappropriately accessible.

# What does this tool do?

ShareAudit impersonates the provided credentials and attempts to access the given systems. It will then enumerate all shares, including hidden shares, and retrieve the first level contents of the shares. It will display whether each file or folder is readable or writable to the impersonated user, as well as display the NTFS permissions. For files, ShareAudit will also retrieve the first 1024 bytes of the file, to add in determining whether the file has sensitive content.

Once the initial audit of the first level of all shares is complete, additional folders can also be audited using the right click menu. The reason to only audit the first level of each share automatically is that it is common to see very complex directory structures, that can take a very long time to process, and could be quickly identified as non-sensitive by the auditor.

The right click menu on folders and files also has the option to reveal the item in explorer. This will open a new Explorer window that can be used to browse the share as the impersonated user.

# How do I use this tool?

When creating a new project, you will be first asked for the credentials you wish to impersonate. This would typically be a low privileged user, representative of a normal domain user. In larger organisations, you may wish to run ShareAudit several times in order to represent users from different departments.

When entering credentials, you will need to specify the user's domain (which is not necessarily the same as the domain you wish to test), username, and password.

Alternatively, you can select to use the current Windows credentials, if you are already logged in as the target user. This can be more reliable, as it prevents the need to impersonate the target user.

Next, you will need to enter the scope, which is the list of systems you wish to audit. The scope can either be entered manually or imported from Active Directory.

Once the configuration is complete, you will be presented with the audit page, press start to begin the scan. You will see systems appear and disappear as they are audited and found to be accessible or inaccessible. Whilst you can interact with the result whilst the scan is running, it is best to wait for it to complete to ensure you are working with accurate results.

Once complete, you can browse the result tree and select a share, folder, or file to view more information, including the effective permissions, NTFS permissions, and file previews.

You can also right click folders and files to open in Explorer as the impersonated user and to audit additional folders that may be interesting.

Results can also be exported to a CSV file once the audit is complete for processing with other tools.

# What am I looking for in the results?

Whilst the nature of sensitive data will vary from organisation to organisation, common indicators include:

* Unnecessary writable files or folders. Write access should be as restrictive as possible to limit the impact of ransomware attacks.

* Access to administrative shares (ADMIN$, C$, etc), as this means the impersonated user has local admin rights on the system.
