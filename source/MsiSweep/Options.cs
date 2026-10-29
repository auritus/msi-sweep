// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep;

internal sealed class Options
{
    public string? MoveTargetDir { get; set; }

    public string? RestoreSourceDir { get; set; }

    public bool ListOrphaned { get; set; }

    public bool ListDangling { get; set; }

    public bool ShowHelp { get; set; }

    public bool ShowVersion { get; set; }

    public static Options? FromCommandLine(string[] args, TextWriter? error = null)
    {
        var options = default(Options?);

        for (var index = 0; index < args.Length;)
        {
            switch (args[index])
            {
                case "--list":
                    options ??= new();
                    options.ListOrphaned = true;
                    options.ListDangling = true;
                    break;

                case "--list-orphaned":
                    options ??= new();
                    options.ListOrphaned = true;
                    break;

                case "--list-dangling":
                    options ??= new();
                    options.ListDangling = true;
                    break;

                case "--move-orphaned" when HasParameterValue(args, index):
                    options ??= new();
                    options.MoveTargetDir = args[++index];
                    break;

                case "--restore" when HasParameterValue(args, index):
                    options ??= new();
                    options.RestoreSourceDir = args[++index];
                    break;

                case "--help":
                    options ??= new();
                    options.ShowHelp = true;
                    break;

                case "--version":
                    options ??= new();
                    options.ShowVersion = true;
                    break;

                default:
                    error?.WriteLine($"Unknown option or missing parameter: {args[index]}");
                    return null;
            }

            index++;
        }

        if (options is { MoveTargetDir: not null, RestoreSourceDir: not null })
        {
            error?.WriteLine("--move-orphaned and --restore cannot be combined.");
            return null;
        }

        return options;

        static bool HasParameterValue(string[] args, int index) =>
            index + 1 < args.Length && !args[1 + index].StartsWith("--", StringComparison.OrdinalIgnoreCase);
    }
}
