using System;
using System.Collections.Generic;
using System.Text;

namespace Firebase
{
    /// <summary>
    /// Generates firebase auth tokens for your firebase.
    /// </summary>
    public class TokenGenerator
    {
        private static int TOKEN_VERSION = 0;
        private string _firebaseSecret;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="firebaseSecret">The Firebase Secret for your firebase (can be found by entering your Firebase URL into a web browser, and clicking the "Auth" pane).</param>
        public TokenGenerator(string firebaseSecret)
        {
            _firebaseSecret = firebaseSecret;
        }

        /// <summary>
        /// Creates an authentication token containing arbitrary auth data.
        /// </summary>
        /// <param name="data">Arbitrary data that will be passed to the Firebase Rules API, once a client authenticates.  Must be able to be serialized to JSON with <see cref="System.Web.Script.Serialization.JavaScriptSerializer"/>.</param>
        /// <returns>The auth token.</returns>
        public string CreateToken(object data) 
        {
            return CreateToken(data, new TokenOptions());
        }

        /// <summary>
        /// Creates an authentication token containing arbitrary auth data and the specified options.
        /// </summary>
        /// <param name="data">Arbitrary data that will be passed to the Firebase Rules API, once a client authenticates.  Must be able to be serialized to JSON with <see cref="System.Web.Script.Serialization.JavaScriptSerializer"/>.</param>
        /// <param name="options">A set of custom options for the token.</param>
        /// <returns>The auth token.</returns>
        public string CreateToken(object data, TokenOptions options)
        {
            var claims = new Dictionary<string, object>();
            claims["v"] = TOKEN_VERSION;
            claims["iat"] = millisecondsSinceEpoch(DateTime.Now);
            claims["d"] = data;

            // Handle options.
            if (options.expires.HasValue)
                claims["exp"] = millisecondsSinceEpoch(options.expires.Value);
            if (options.notBefore.HasValue)
                claims["nbf"] = millisecondsSinceEpoch(options.notBefore.Value);
            if (options.admin)
                claims["admin"] = true;
            if (options.debug)
                claims["debug"] = true;

            return computeToken(claims);
        }

        private string computeToken(Dictionary<string, object> claims)
        {
            return JWT.JsonWebToken.Encode(claims, this._firebaseSecret, JWT.JwtHashAlgorithm.HS256);
        }

        private static long millisecondsSinceEpoch(DateTime dt)
        {
            TimeSpan t = dt.ToUniversalTime() - new DateTime(1970, 1, 1);
            return (long)t.TotalMilliseconds;
        }
    }
}
