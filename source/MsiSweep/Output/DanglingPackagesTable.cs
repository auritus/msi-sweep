// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using MsiSweep.Extensions;
using MsiSweep.Inventory;
using MsiSweep.Model;

namespace MsiSweep.Output;

internal static class DanglingPackagesTable
{
    public static void Print(InstallerInventory inventory)
    {
        var packages = inventory.Packages
            .Where(package => package.LocalPackage.Value.Length > 0 && !package.LocalPackage.Exists)
            .ToArray();

        Console.WriteLine();
        Console.WriteHeader("Dangling Packages Summary");

        if (packages.Length == 0)
        {
            Console.WriteLine();
            Console.WriteLine("No dangling packages detected.");
            Console.WriteLine();
            return;
        }

        var table = new VerticalTable<Package>([
            Column.Create("", (Package p) => p.Name),
            Column.Create("Kind", (Package p) => p.Kind),
            Column.Create("Code", (Package p) => p.Code.ToApiString()),
            Column.Create("User", (Package p) => p.User),
            Column.Create("File", (Package p) => p.LocalPackage),
            Column.Create("Registry", (Package p) => $"HKLM\\{p.RegistryKey}"),
        ]);

        table.Print(packages, new Margin(2));
    }
}
