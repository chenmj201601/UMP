
namespace VoiceCyber.WebSockets
{
    internal enum InputChunkState
    {
        None,
        Data,
        DataEnded,
        Trailer,
        End
    }
}
