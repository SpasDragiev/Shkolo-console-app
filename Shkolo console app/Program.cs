using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RazorConsole.Core;
using Shkolo_console_app.Data;
using Shkolo_console_app.Models;
using Shkolo_console_app.UI;

// Load the saved school data once; the Razor components get it through dependency injection.
var storage = new SchoolStorage(new JsonRepository());
var school = storage.Load();

// Make sure the default subjects exist (only missing ones are added, so nothing is duplicated).
string[] defaultSubjects =
{
    "Mathematics", "Bulgarian", "English", "Biology", "Physics", "Science",
    "Geography", "History", "IT", "Object-oriented programming (C#)",
    "Software technologies (JavaScript)"
};
bool added = false;
foreach (string subjectName in defaultSubjects)
{
    if (school.Subjects.Any(s => s.Name == subjectName)) continue;
    int nextId = school.Subjects.Count == 0 ? 1 : school.Subjects.Max(s => s.Id) + 1;
    school.AddSubject(new Subject(nextId, subjectName));
    added = true;
}
if (added) storage.Save(school);

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging => logging.ClearProviders()) // log lines would mess up the screen
    .ConfigureServices(services =>
    {
        services.AddSingleton(storage);
        services.AddSingleton(school);
    })
    .UseRazorConsole<App>()
    .Build();

await host.RunAsync();
