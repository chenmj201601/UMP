using System;
using System.Net;

namespace VoiceCyber.WebSockets
{
    internal sealed class HttpListenerPrefix
    {
        #region Private Fields

        private IPAddress[] _addresses;
        private string _host;
        private HttpListener _listener;
        private string _original;
        private string _path;
        private ushort _port;
        private bool _secure;

        #endregion

        #region Internal Constructors

        // Must be called after calling the CheckPrefix method.
        internal HttpListenerPrefix(string uriPrefix)
        {
            _original = uriPrefix;
            parse(uriPrefix);
        }

        #endregion

        #region Public Properties

        public IPAddress[] Addresses
        {
            get
            {
                return _addresses;
            }

            set
            {
                _addresses = value;
            }
        }

        public string Host
        {
            get
            {
                return _host;
            }
        }

        public bool IsSecure
        {
            get
            {
                return _secure;
            }
        }

        public HttpListener Listener
        {
            get
            {
                return _listener;
            }

            set
            {
                _listener = value;
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
        }

        public int Port
        {
            get
            {
                return _port;
            }
        }

        #endregion

        #region Private Methods

        private void parse(string uriPrefix)
        {
            var defaultPort = uriPrefix.StartsWith("https://") ? 443 : 80;
            if (defaultPort == 443)
                _secure = true;

            var len = uriPrefix.Length;
            var startHost = uriPrefix.IndexOf(':') + 3;
            var colon = uriPrefix.IndexOf(':', startHost, len - startHost);
            int root;
            if (colon > 0)
            {
                root = uriPrefix.IndexOf('/', colon, len - colon);
                _host = uriPrefix.Substring(startHost, colon - startHost);
                _port = (ushort)Int32.Parse(uriPrefix.Substring(colon + 1, root - colon - 1));
            }
            else
            {
                root = uriPrefix.IndexOf('/', startHost, len - startHost);
                _host = uriPrefix.Substring(startHost, root - startHost);
                _port = (ushort)defaultPort;
            }

            _path = uriPrefix.Substring(root);

            var pathLen = _path.Length;
            if (pathLen > 1)
                _path = _path.Substring(0, pathLen - 1);
        }

        #endregion

        #region Public Methods

        public static void CheckPrefix(string uriPrefix)
        {
            if (uriPrefix == null)
                throw new ArgumentNullException("uriPrefix");

            var len = uriPrefix.Length;
            if (len == 0)
                throw new ArgumentException("An empty string.");

            if (!(uriPrefix.StartsWith("http://") || uriPrefix.StartsWith("https://")))
                throw new ArgumentException("The scheme isn't 'http' or 'https'.");

            var startHost = uriPrefix.IndexOf(':') + 3;
            if (startHost >= len)
                throw new ArgumentException("No host is specified.");

            var colon = uriPrefix.IndexOf(':', startHost, len - startHost);
            if (startHost == colon)
                throw new ArgumentException("No host is specified.");

            if (colon > 0)
            {
                var root = uriPrefix.IndexOf('/', colon, len - colon);
                if (root == -1)
                    throw new ArgumentException("No path is specified.");

                int port;
                if (!Int32.TryParse(uriPrefix.Substring(colon + 1, root - colon - 1), out port) ||
                    !port.IsPortNumber())
                    throw new ArgumentException("An invalid port is specified.");
            }
            else
            {
                var root = uriPrefix.IndexOf('/', startHost, len - startHost);
                if (root == -1)
                    throw new ArgumentException("No path is specified.");
            }

            if (uriPrefix[len - 1] != '/')
                throw new ArgumentException("Ends without '/'.");
        }

        // The Equals and GetHashCode methods are required to detect duplicates in any collection.
        public override bool Equals(Object obj)
        {
            var pref = obj as HttpListenerPrefix;
            return pref != null && pref._original == _original;
        }

        public override int GetHashCode()
        {
            return _original.GetHashCode();
        }

        public override string ToString()
        {
            return _original;
        }

        #endregion
    }
}
