﻿using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Objects;

public sealed class DestinationNode
{
    public Directions Direction { get; init; }
    public Location? Location { get; init; }
    public bool IsHidden { get; set; } = false;
    public bool ShowInDescription { get; set; } = true;
    /// <summary>
    /// This description is displayed when the location is listed in a way description.
    /// If the value is not set, a standardized message is shown.
    /// </summary>
    public string DestinationDescription { get; set; } = string.Empty;

    public override string ToString()
    {
        if (ShowInDescription)
        {
            if (string.IsNullOrEmpty(this.DestinationDescription))
            {
                return
                    $"{Location?.Name} {BaseDescriptions.WAY} {this.GetDirectionDescription()}. {this.GetLockDescription()}";
            }

            return $"{this.DestinationDescription} {this.GetLockDescription()}";
        }
        
        return string.Empty;
    }

    private string GetDirectionDescription()
    {
        var result = Direction switch
        {
            Directions.UP => Verbs.UP.Split('|')[0],
            Directions.DOWN => Verbs.DOWN.Split('|')[0],
            Directions.N => Verbs.N.Split('|')[0],
            Directions.S => Verbs.S.Split('|')[0],
            Directions.E => Verbs.E.Split('|')[0],
            Directions.W => Verbs.W.Split('|')[0],
            Directions.NE => Verbs.NE.Split('|')[0],
            Directions.NW => Verbs.NW.Split('|')[0],
            Directions.SE => Verbs.SE.Split('|')[0],
            Directions.SW => Verbs.SW.Split('|')[0],
            _ => throw new ArgumentOutOfRangeException()
        };

        return result;
    }

    private string GetLockDescription()
    {
        return Location is { IsLocked: true } ? Location.LockDescription : string.Empty;
    }
}
