using Reloaded.Mod.Interfaces;
using SmoothAnalogMovement.Template;
using Reloaded.Memory.Sigscan;
using System.Diagnostics;
using Reloaded.Memory.Sources;
using Reloaded.Memory.Sigscan.Definitions;
using Reloaded.Memory.Sigscan.Definitions.Structs;

namespace SmoothAnalogMovement;

/// <summary>
/// Your mod logic goes here.
/// </summary>
public class Mod : ModBase // <= Do not Remove.
{
    /// <summary>
    /// Provides access to the mod loader API.
    /// </summary>
    private readonly IModLoader _modLoader;

    /// <summary>
    /// Provides access to the Reloaded logger.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Entry point into the mod, instance that created this class.
    /// </summary>
    private readonly IMod _owner;

    /// <summary>
    /// The configuration of the currently executing mod.
    /// </summary>
    private readonly IModConfig _modConfig;

    public Mod(ModContext context)
    {
        _modLoader = context.ModLoader;
        _logger = context.Logger;
        _owner = context.Owner;
        _modConfig = context.ModConfig;


        // For more information about this template, please see
        // https://reloaded-project.github.io/Reloaded-II/ModTemplate/

        // If you want to implement e.g. unload support in your mod,
        // and some other neat features, override the methods in ModBase.

        var mainModule = Process.GetCurrentProcess().MainModule;
        if (mainModule == null)
        {
            throw new Exception("[SmoothAnalogMovement]Main Module is Null!");
        }
        var baseAddress = mainModule.BaseAddress;
        var exeSize = mainModule.ModuleMemorySize;
        unsafe
        {
            using var scanner = new Scanner((byte*)baseAddress, exeSize);
            int instructionToPatchAddressOffset = FindInstructionToPatch(scanner);

            _logger.WriteLine($"[SmoothAnalogMovement] instruction offset {instructionToPatchAddressOffset:X}");

            var ínstructionAddress = baseAddress + instructionToPatchAddressOffset;
            var instructionSize = 8;
            var instructionSourceDataOffset = 4;
            var zeroFloatAddressOffset = 0x14;

            Memory.Instance.SafeWrite(ínstructionAddress + instructionSourceDataOffset, zeroFloatAddressOffset - instructionToPatchAddressOffset - instructionSize);

        }
        _logger.WriteLine($"[SmoothAnalogMovement] Init Ok");
    }

    private int FindInstructionToPatch(Scanner scanner)
    {
        var result = scanner.FindPattern("F3 0F 10 3D ?? ?? ?? ?? 44 0F 29 44 24 20 f2 44 0f 10 05");
        if (!result.Found)
        {
            throw new Exception("[SmoothAnalogMovement]Pattern not found");
        }

        return result.Offset;
    }

    #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Mod() { }
#pragma warning restore CS8618
    #endregion
}