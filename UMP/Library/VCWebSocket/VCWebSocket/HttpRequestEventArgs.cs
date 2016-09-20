using System;

namespace VoiceCyber.WebSockets
{
    /// <summary>
    /// Contains the event data associated with an HTTP request event that
    /// the <see cref="HttpServer"/> emits.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   An HTTP request event occurs when the <see cref="HttpServer"/> receives an HTTP request.
    ///   </para>
    ///   <para>
    ///   If you would like to get the request data, you should access
    ///   the <see cref="HttpRequestEventArgs.Request"/> property.
    ///   </para>
    ///   <para>
    ///   And if you would like to get the data used to return a response, you should access
    ///   the <see cref="HttpRequestEventArgs.Response"/> property.
    ///   </para>
    /// </remarks>
    public class HttpRequestEventArgs : EventArgs
    {
        #region Private Fields

        private HttpListenerRequest _request;
        private HttpListenerResponse _response;

        #endregion

        #region Internal Constructors

        internal HttpRequestEventArgs(HttpListenerContext context)
        {
            _request = context.Request;
            _response = context.Response;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the <see cref="HttpListenerRequest"/> that represents the HTTP request sent from
        /// a client.
        /// </summary>
        /// <value>
        /// A <see cref="HttpListenerRequest"/> that represents the request.
        /// </value>
        public HttpListenerRequest Request
        {
            get
            {
                return _request;
            }
        }

        /// <summary>
        /// Gets the <see cref="HttpListenerResponse"/> used to return an HTTP response to the client.
        /// </summary>
        /// <value>
        /// A <see cref="HttpListenerResponse"/> used to return a response.
        /// </value>
        public HttpListenerResponse Response
        {
            get
            {
                return _response;
            }
        }

        #endregion
    }
}
