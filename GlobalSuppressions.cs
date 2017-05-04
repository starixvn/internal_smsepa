// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Sonar Code Smell",
    "S107:Methods should not have too many parameters",
    Justification = "<Pending>")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Sonar Code Smell",
    "S100:Methods and properties should be named in camel case",
    Justification = "<Pending>")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Sonar Code Smell",
    "S1541:Methods and properties should not be too complex",
    Justification = "<Pending>")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Sonar Code Smell",
    "S2342:Enumeration types should comply with a naming convention",
    Justification = "<Pending>",
    Scope = "type",
    Target = "~T:SMS.EPA.Core.Enumerations.EPAStatus")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Sonar Code Smell",
    "S2325:Methods and properties that don't access instance data should be static",
    Justification = "Required for app to work",
    Scope = "member",
    Target = "~M:SMS.EPA.Site.WebApiApplication.Application_Start")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Sonar Code Smell",
    "S2325:Methods and properties that don't access instance data should be static",
    Justification = "Required for app to work",
    Scope = "member",
    Target = "~M:SMS.EPA.Site.WebApiApplication.Application_PostAuthorizeRequest")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Sonar Code Smell",
    "S104:Files should not have too many lines of code",
    Justification = "Reviewed",
    Scope = "namespace",
    Target = "~N:SMS.EPA.Site.Controllers")]