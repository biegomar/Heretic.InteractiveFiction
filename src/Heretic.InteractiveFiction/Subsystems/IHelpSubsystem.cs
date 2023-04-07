using Heretic.InteractiveFiction.Grammars;

namespace Heretic.InteractiveFiction.Subsystems;

/// <summary>The help subsystem.</summary>
public interface IHelpSubsystem
{
    /// <summary>Get help for a all help categories.</summary>
    bool Help();
    
    /// <summary>Get help for a specific help category.</summary>
    bool Help(HelpCategory helpCategory);
    
    /// <summary>Get help for a specific verb.</summary>
    bool Help(IList<Verb> verbs);
}