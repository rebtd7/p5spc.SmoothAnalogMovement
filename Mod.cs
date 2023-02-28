using Reloaded.Mod.Interfaces;
using SmoothAnalogMovement.Template;
using System.Diagnostics;
using Reloaded.Memory.Sources;

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
            _logger.WriteLine("[SmoothAnalogMovement] Fail to load. MainModule is null.");
            return;
        }
        var baseAddress = (long)mainModule.BaseAddress;
        var analogDirectionDeadZoneAddress = baseAddress + 0x18F8148;
        var resolutionChangePatchAddress = baseAddress + 0x2978AFB;
        unsafe
        {
            var analogDirectionDeadZoneValue = *(float*)analogDirectionDeadZoneAddress;
            if (MathF.Abs(analogDirectionDeadZoneValue - 0.0625f) > 0.00001)
            {
                _logger.WriteLine($"[SmoothAnalogMovement] Fail to load. analogDirectionDeadZoneValue = [{analogDirectionDeadZoneValue}]");
                return;
            }
        }
        Memory.Instance.SafeWrite(analogDirectionDeadZoneAddress, 0.0f);
        
        // change instruction so that it points to any read only value in RAM with the original value of 0.0625f...
        Memory.Instance.SafeWrite(resolutionChangePatchAddress, 0xFD752A35);
        _logger.WriteLine($"[SmoothAnalogMovement] Init Ok");
    }

    #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Mod() { }
#pragma warning restore CS8618
    #endregion
}