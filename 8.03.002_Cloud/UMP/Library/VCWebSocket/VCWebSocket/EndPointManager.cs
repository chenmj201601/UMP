using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace VoiceCyber.WebSockets
{
  internal sealed class EndPointManager
  {
    #region Private Fields

    private static readonly Dictionary<IPAddress, Dictionary<int, EndPointListener>>
      _addressToEndpoints;

    #endregion

    #region Static Constructor

    static EndPointManager ()
    {
      _addressToEndpoints = new Dictionary<IPAddress, Dictionary<int, EndPointListener>> ();
    }

    #endregion

    #region Private Constructors

    private EndPointManager ()
    {
    }

    #endregion

    #region Private Methods

    private static void addPrefix (string uriPrefix, HttpListener listener)
    {
      var pref = new HttpListenerPrefix (uriPrefix);

      var path = pref.Path;
      if (path.IndexOf ('%') != -1)
        throw new HttpListenerException (400, "Invalid path."); // TODO: Code?

      if (path.IndexOf ("//", StringComparison.Ordinal) != -1)
        throw new HttpListenerException (400, "Invalid path."); // TODO: Code?

      // Listens on all the interfaces if host name cannot be parsed by IPAddress.
      getEndPointListener (pref, listener).AddPrefix (pref, listener);
    }

    private static IPAddress convertToIPAddress (string hostname)
    {
      if (hostname == "*" || hostname == "+")
        return IPAddress.Any;

      IPAddress addr;
      if (IPAddress.TryParse (hostname, out addr))
        return addr;

      try {
        var host = Dns.GetHostEntry (hostname);
        return host != null ? host.AddressList[0] : IPAddress.Any;
      }
      catch {
        return IPAddress.Any;
      }
    }

    private static EndPointListener getEndPointListener (
      HttpListenerPrefix prefix, HttpListener listener)
    {
      var addr = convertToIPAddress (prefix.Host);

      Dictionary<int, EndPointListener> eps;
      if (_addressToEndpoints.ContainsKey (addr)) {
        eps = _addressToEndpoints[addr];
      }
      else {
        eps = new Dictionary<int, EndPointListener> ();
        _addressToEndpoints[addr] = eps;
      }

      var port = prefix.Port;

      EndPointListener lsnr;
      if (eps.ContainsKey (port)) {
        lsnr = eps[port];
      }
      else {
        lsnr = new EndPointListener (
          addr,
          port,
          listener.ReuseAddress,
          prefix.IsSecure,
          listener.CertificateFolderPath,
          listener.SslConfiguration);

        eps[port] = lsnr;
      }

      return lsnr;
    }

    private static void removePrefix (string uriPrefix, HttpListener listener)
    {
      var pref = new HttpListenerPrefix (uriPrefix);

      var path = pref.Path;
      if (path.IndexOf ('%') != -1)
        return;

      if (path.IndexOf ("//", StringComparison.Ordinal) != -1)
        return;

      getEndPointListener (pref, listener).RemovePrefix (pref, listener);
    }

    #endregion

    #region Internal Methods

    internal static void RemoveEndPoint (EndPointListener listener)
    {
      lock (((ICollection) _addressToEndpoints).SyncRoot) {
        var addr = listener.Address;
        var eps = _addressToEndpoints[addr];
        eps.Remove (listener.Port);
        if (eps.Count == 0)
          _addressToEndpoints.Remove (addr);

        listener.Close ();
      }
    }

    #endregion

    #region Public Methods

    public static void AddListener (HttpListener listener)
    {
      var added = new List<string> ();
      lock (((ICollection) _addressToEndpoints).SyncRoot) {
        try {
          foreach (var pref in listener.Prefixes) {
            addPrefix (pref, listener);
            added.Add (pref);
          }
        }
        catch {
          foreach (var pref in added)
            removePrefix (pref, listener);

          throw;
        }
      }
    }

    public static void AddPrefix (string uriPrefix, HttpListener listener)
    {
      lock (((ICollection) _addressToEndpoints).SyncRoot)
        addPrefix (uriPrefix, listener);
    }

    public static void RemoveListener (HttpListener listener)
    {
      lock (((ICollection) _addressToEndpoints).SyncRoot)
        foreach (var pref in listener.Prefixes)
          removePrefix (pref, listener);
    }

    public static void RemovePrefix (string uriPrefix, HttpListener listener)
    {
      lock (((ICollection) _addressToEndpoints).SyncRoot)
        removePrefix (uriPrefix, listener);
    }

    #endregion
  }
}
