using System;
using System.Security.Principal;

namespace VoiceCyber.WebSockets
{
    /// <summary>
    /// Provides the access to the HTTP request and response information
    /// used by the <see cref="HttpListener"/>.
    /// </summary>
    /// <remarks>
    /// The HttpListenerContext class cannot be inherited.
    /// </remarks>
    public sealed class HttpListenerContext
    {
        #region Private Fields

        private HttpConnection _connection;
        private string _error;
        private int _errorStatus;
        private HttpListener _listener;
        private HttpListenerRequest _request;
        private HttpListenerResponse _response;
        private IPrincipal _user;

        #endregion

        #region Internal Constructors

        internal HttpListenerContext(HttpConnection connection)
        {
            _connection = connection;
            _errorStatus = 400;
            _request = new HttpListenerRequest(this);
            _response = new HttpListenerResponse(this);
        }

        #endregion

        #region Internal Properties

        internal HttpConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        internal string ErrorMessage
        {
            get
            {
                return _error;
            }

            set
            {
                _error = value;
            }
        }

        internal int ErrorStatus
        {
            get
            {
                return _errorStatus;
            }

            set
            {
                _errorStatus = value;
            }
        }

        internal bool HasError
        {
            get
            {
                return _error != null;
            }
        }

        internal HttpListener Listener
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

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the HTTP request information from a client.
        /// </summary>
        /// <value>
        /// A <see cref="HttpListenerRequest"/> that represents the HTTP request.
        /// </value>
        public HttpListenerRequest Request
        {
            get
            {
                return _request;
            }
        }

        /// <summary>
        /// Gets the HTTP response information used to send to the client.
        /// </summary>
        /// <value>
        /// A <see cref="HttpListenerResponse"/> that represents the HTTP response to send.
        /// </value>
        public HttpListenerResponse Response
        {
            get
            {
                return _response;
            }
        }

        /// <summary>
        /// Gets the client information (identity, authentication, and security roles).
        /// </summary>
        /// <value>
        /// A <see cref="IPrincipal"/> instance that represents the client information.
        /// </value>
        public IPrincipal User
        {
            get
            {
                return _user;
            }

            internal set
            {
                _user = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Accepts a WebSocket connection request.
        /// </summary>
        /// <returns>
        /// A <see cref="HttpListenerWebSocketContext"/> that represents the WebSocket connection
        /// request.
        /// </returns>
        /// <param name="protocol">
        /// A <see cref="string"/> that represents the subprotocol used in the WebSocket connection.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   <para>
        ///   <paramref name="protocol"/> is empty.
        ///   </para>
        ///   <para>
        ///   -or-
        ///   </para>
        ///   <para>
        ///   <paramref name="protocol"/> contains an invalid character.
        ///   </para>
        /// </exception>
        public HttpListenerWebSocketContext AcceptWebSocket(string protocol)
        {
            if (protocol != null)
            {
                if (protocol.Length == 0)
                    throw new ArgumentException("An empty string.", "protocol");

                if (!protocol.IsToken())
                    throw new ArgumentException("Contains an invalid character.", "protocol");
            }

            return new HttpListenerWebSocketContext(this, protocol);
        }

        #endregion
    }
}
