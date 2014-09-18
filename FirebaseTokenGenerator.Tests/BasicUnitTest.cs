using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Firebase;

namespace Firebase.Tests
{
    [TestClass]
    public class BasicUnitTest
    {
        private string FIREBASE_SUPER_SECRET_KEY = "moozooherpderp";

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CheckIfBasicLength()
        {
            var payload = new Dictionary<string, object>();

            var tokenGenerator = new TokenGenerator("x");
            var token = tokenGenerator.CreateToken(payload);
        }

        [TestMethod]
        public void CheckBasicStructureHasCorrectNumberOfFragments()
        {
            var payload = new Dictionary<string, object>
            {
                { "uid", "1" },
                { "abc", "0123456789~!@#$%^&*()_+-=abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,./;'[]\\<>?\"{}|" }
            };

            var tokenGenerator = new TokenGenerator(FIREBASE_SUPER_SECRET_KEY);
            var token = tokenGenerator.CreateToken(payload);

            String[] tokenFragments = token.Split('.');

            Assert.IsTrue(tokenFragments.Length == 3, "Token has the proper number of fragments: jwt metadata, payload, and signature");
        }

        [TestMethod]
        public void CheckResultProperlyDoesNotHavePadding()
        {
            var payload = new Dictionary<string, object>
            {
                { "uid", "1" },
                { "abc", "0123456789~!@#$%^&*()_+-=abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,./;'[]\\<>?\"{}|" }
            };

            var tokenGenerator = new TokenGenerator(FIREBASE_SUPER_SECRET_KEY);
            var token = tokenGenerator.CreateToken(payload);

            Assert.IsTrue(token.IndexOf('=') < 0);
        }

        [TestMethod]
        public void CheckIfResultIsUrlSafePlusSign()
        {
            var payload = new Dictionary<string, object>
            {
                { "uid", "1" },
                { "abc", "0123456789~!@#$%^&*()_+-=abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,./;'[]\\<>?\"{}|" }
            };

            var tokenGenerator = new TokenGenerator(FIREBASE_SUPER_SECRET_KEY);
            var token = tokenGenerator.CreateToken(payload);

            Assert.IsTrue(token.IndexOf('+') < 0);
        }

        [TestMethod]
        public void CheckIfResultIsUrlSafePlusSlash()
        {
            var payload = new Dictionary<string, object>
            {
                { "uid", "1" },
                { "abc", "0123456789~!@#$%^&*()_+-=abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,./;'[]\\<>?\"{}|" }
            };

            var tokenGenerator = new TokenGenerator(FIREBASE_SUPER_SECRET_KEY);
            var token = tokenGenerator.CreateToken(payload);

            Assert.IsTrue(token.IndexOf('/') < 0);
        }

        [TestMethod]
        public void CheckIfResultHasWhiteSpace()
        {
            var payload = new Dictionary<string, object>
            {
                { "uid", "1" },
                { "a", "apple" },
                { "b", "banana" },
                { "c", "carrot" },
                { "number", Double.MaxValue },
                { "abc", "0123456789~!@#$%^&*()_+-=abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,./;'[]\\<>?\"{}|" },
                { "herp1", "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.?" }
            };

            var tokenGenerator = new TokenGenerator(FIREBASE_SUPER_SECRET_KEY);
            var token = tokenGenerator.CreateToken(payload);

            var pattern = new Regex(@"\s");
            var hasWhiteSpace = pattern.IsMatch(token);

            Assert.IsFalse(hasWhiteSpace, "Token has white space");
        }

        [TestMethod]
        public void BasicInspectTest()
        {
            var customData = "0123456789~!@#$%^&*()_+-=abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,./;'[]\\<>?\"{}|";
            var payload = new Dictionary<string, object>
            {
                { "uid", "1" },
                { "abc", customData }
            };

            var tokenGenerator = new TokenGenerator(FIREBASE_SUPER_SECRET_KEY);
            var tokenOptions = new TokenOptions(DateTime.Now, DateTime.Now, true, true);

            var token = tokenGenerator.CreateToken(payload, tokenOptions);
            var decoded = JWT.JsonWebToken.DecodeToObject(token, FIREBASE_SUPER_SECRET_KEY) as Dictionary<string, object>;
            Assert.IsTrue(decoded.ContainsKey("v") && (decoded["v"] is int) && (int.Parse(decoded["v"].ToString()) == 0));
            Assert.IsTrue(decoded.ContainsKey("d") && (decoded["d"] as Dictionary<string, object>).ContainsKey("abc"));
            Assert.IsTrue(decoded.ContainsKey("exp") && (decoded["exp"] is int));
            Assert.IsTrue(decoded.ContainsKey("iat") && (decoded["iat"] is int));
            Assert.IsTrue(decoded.ContainsKey("nbf") && (decoded["nbf"] is int));
            Assert.IsTrue(decoded.ContainsKey("admin") && (decoded["admin"] is bool));
            Assert.IsTrue(decoded.ContainsKey("debug") && (decoded["debug"] is bool));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void RequireUidInPayload()
        {
            var payload = new Dictionary<string, object>
            {
                { "abc", "0123456789~!@#$%^&*()_+-=abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,./;'[]\\<>?\"{}|" }
            };

            var tokenGenerator = new TokenGenerator(FIREBASE_SUPER_SECRET_KEY);
            var token = tokenGenerator.CreateToken(payload);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void RequireUidStringInPayload()
        {
            var payload = new Dictionary<string, object>
            {
                { "uid", 1 }
            };

            var tokenGenerator = new TokenGenerator(FIREBASE_SUPER_SECRET_KEY);
            var token = tokenGenerator.CreateToken(payload);
        }

        [TestMethod]
        public void AllowMaxLengthUid()
        {
            var payload = new Dictionary<string, object>
            {
                //                10        20        30        40        50        60        70        80        90       100       110       120       130       140       150       160       170       180       190       200       210       220       230       240       250   256
                { "uid", "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456" }
            };

            var tokenGenerator = new TokenGenerator(FIREBASE_SUPER_SECRET_KEY);
            var token = tokenGenerator.CreateToken(payload);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DisallowUidTooLong()
        {
            var payload = new Dictionary<string, object>
            {
                //                10        20        30        40        50        60        70        80        90       100       110       120       130       140       150       160       170       180       190       200       210       220       230       240       250    257
                { "uid", "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567" }
            };

            var tokenGenerator = new TokenGenerator(FIREBASE_SUPER_SECRET_KEY);
            var token = tokenGenerator.CreateToken(payload);
        }

        [TestMethod]
        public void AllowEmptyStringUid()
        {
            var payload = new Dictionary<string, object>
            {
                { "uid", "" }
            };

            var tokenGenerator = new TokenGenerator(FIREBASE_SUPER_SECRET_KEY);
            var token = tokenGenerator.CreateToken(payload);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DisallowTokensTooLong()
        {
            var payload = new Dictionary<string, object>
            {
                { "uid", "blah" },
                { "longVar", "123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345612345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234561234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456" }
            };

            var tokenGenerator = new TokenGenerator(FIREBASE_SUPER_SECRET_KEY);
            var token = tokenGenerator.CreateToken(payload);
        }

        [TestMethod]
        public void AllowNoUidWithAdmin()
        {
            var tokenOptions = new TokenOptions(null, null, true, false);

            var tokenGenerator = new TokenGenerator(FIREBASE_SUPER_SECRET_KEY);
            var token = tokenGenerator.CreateToken(null, tokenOptions);
            var payload1 = new Dictionary<string, object>();
            var token1 = tokenGenerator.CreateToken(payload1, tokenOptions);
            var payload2 = new Dictionary<string, object>
            {
                { "foo", "bar" }
            };
            var token2 = tokenGenerator.CreateToken(payload2, tokenOptions);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DisallowInvalidUidWithAdmin1()
        {
            var payload = new Dictionary<string, object>
            {
                { "uid", 1 }
            };

            var tokenOptions = new TokenOptions(null, null, true, false);

            var tokenGenerator = new TokenGenerator(FIREBASE_SUPER_SECRET_KEY);
            var token = tokenGenerator.CreateToken(payload, tokenOptions);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DisallowInvalidUidWithAdmin2()
        {
            var payload = new Dictionary<string, object>
            {
                { "uid", null }
            };

            var tokenOptions = new TokenOptions(null, null, true, false);

            var tokenGenerator = new TokenGenerator(FIREBASE_SUPER_SECRET_KEY);
            var token = tokenGenerator.CreateToken(payload, tokenOptions);
        }
    }
}
