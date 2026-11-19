using Shkolo_console_app.Data;
using Shkolo_console_app.UI;

var storage = new SchoolStorage(new JsonRepository());
var school = storage.Load();

new ConsoleMenu(school, storage).Run();
