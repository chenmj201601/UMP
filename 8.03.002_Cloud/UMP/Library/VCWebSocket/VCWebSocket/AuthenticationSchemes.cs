using System;

namespace VoiceCyber.WebSockets
{
  /// <summary>
  /// Contains the values of the schemes for authentication.
  /// </summary>
  [Flags]
  public enum AuthenticationSchemes
  {
    /// <summary>
    /// Indicates that no authentication is allowed.
    /// </summary>
    None,
    /// <summary>
    /// Indicates digest authentication.
    /// </summary>
    Digest = 1,
    /// <summary>
    /// Indicates basic authentication.
    /// </summary>
    Basic = 8,
    /// <summary>
    /// Indicates anonymous authentication.
    /// </summary>
    Anonymous = 0x8000
  }
}
