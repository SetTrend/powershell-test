# Multi-Threading Issue in Active Directory PowerShell module
The [Microsoft Active Directory module for Windows PowerShell](https://docs.microsoft.com/en-us/powershell/module/addsadministration/?view=win10-ps)
reveals a multi-threading issue.

This sample repository depicts that multi-threading issue.

When a number of PowerShell runspaces are created in several threads of a
.NET Framework application, and if those PowerShell runspaces simultaneously run Cmdlets from the
**Microsoft Active Directory module for Windows PowerShell**, then
[Active Directory Web Services (ADWS)](https://blogs.msdn.microsoft.com/adpowershell/2009/04/06/active-directory-web-services-overview/)
fails by throwing an `invalid enumration context` exception of type `System.ServiceModel.FaultException`.

So, enumeration contexts don't seem to be stored thread-safe by **Microsoft
Active Directory module for Windows PowerShell**.

<br/>

## Steps to Reproduce

### Prerequisites
- Make sure you have access to a Windows Active Directory domain controller
from your development machine.

- .NET Framework 4.7.2 must be installed on the development machine for the project
  to compile.

- To create an executable file from the code in this repository, Microsoft Visual Studio
  2017/2019 should be installed on the development machine.

<br/>

1. Download this repository to your local drive on your development machine.
2. Open the `PowerShell Test.sln` file using Visual Studio 2017/2019.
3. Make sure to download the referenced [System.Management.Automation.dll](https://www.nuget.org/packages/System.Management.Automation.dll/)
   NuGet package using Visual Studio 2017/2019 Package Manager.
4. Compile and run the console application.

<br/>

## What Does This Sample Project Do?

The code in this repository's project creates a number of threads that run in parallel.

Within each of these threads, a new PowerShell runspace is created and within each PowerShell
runspace a PowerShell script is invoked. Basically, all threads simply invoke the same
PowerShell script.

What the PowerShell script does is simply accessing **Active Directory Web Services (ADWS)**
by querying LDAP a number of times, running the `Get-ADUser` Cmdlet from the **Microsoft
Active Directory module for Windows PowerShell**.

Each of the LDAP queries establishes its own enumeration context when connecting to
**Active Directory Web Services**. Yet, when iterating the result returned by **Active
Directory Web Services**, wrong enumeration context information is sent to **ADWS** which
results in a `System.ServiceModel.FaultException` to be thrown with the exception message
`invalid enumeration context`.

<br/>

## Why Does This Repository Exist

There are many programmers out there [facing the `invalid enumeration context` issue](https://www.google.com/search?client=firefox-b-d&q=activedirectory+invalid+enumeration+context).

With this sample project the failure can easily be reproduced. Currently, there is no
workaround for this issue.

I believe this to be a bug in **Microsoft Active Directory module for Windows PowerShell**.
It is a breaking issue and I hope Microsoft will be able to fix it using the test code from this repository.

<br/>

## Program Execution Screenshot

![PowerShell Test screenshot](./PowerShell%20Test%20screenshot.png)