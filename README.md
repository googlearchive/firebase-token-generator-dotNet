# Status: Archived
This repository has been archived and is no longer maintained.

![status: inactive](https://img.shields.io/badge/status-inactive-red.svg)

# Firebase Token Generator - .NET

**WARNING: This token generator is compatible with versions 1.x.x and 2.x.x of the Firebase SDK. If you are using the 3.x.x SDK, please refer to the documentation [here](https://firebase.google.com/docs/auth/server#use_a_jwt_library).**

[Firebase Custom Login](https://www.firebase.com/docs/web/guide/simple-login/custom.html)
gives you complete control over user authentication by allowing you to authenticate users
with secure JSON Web Tokens (JWTs). The auth payload stored in those tokens is available
for use in your Firebase [security rules](https://www.firebase.com/docs/security/api/rule/).
This is a token generator library for .Net which allows you to easily create those JWTs.


## Installation

The easiest way to install The Firebase .Net token generator is via NuGet. Just search for
"FirebaseTokenGenerator" in NuGet or install it via the Package Manager Console:

```
PM> Install-Package FirebaseTokenGenerator
```


## A Note About Security

**IMPORTANT:** Because token generation requires your Firebase Secret, you should only generate
tokens on *trusted servers*. Never embed your Firebase Secret directly into your application and
never share your Firebase Secret with a connected client.


## Generating Tokens

To generate tokens, you'll need your Firebase Secret which you can find by entering your Firebase
URL into a browser and clicking the "Secrets" tab on the left-hand navigation menu.

Once you've downloaded the library and grabbed your Firebase Secret, you can generate a token with
this snippet of .Net code:

```
var tokenGenerator = new Firebase.TokenGenerator("<YOUR_FIREBASE_SECRET>");
var authPayload = new Dictionary<string, object>()
{
  { "uid", "1" },
  { "some", "arbitrary" },
  { "data", "here" }
};
string token = tokenGenerator.CreateToken(authPayload);
```

The payload object passed into `CreateToken()` is then available for use within your
security rules via the [`auth` variable](https://www.firebase.com/docs/security/api/rule/auth.html).
This is how you pass trusted authentication details (e.g. the client's user ID) to your
Firebase rules. The payload can contain any data of your choosing, however it
must contain a "uid" key, which must be a string of less than 256 characters. The
generated token must be less than 1024 characters in total.


## Token Options

A second `options` argument can be passed to `CreateToken()` to modify how Firebase treats the
token. Available options are:

* **expires** (DateTime) - A timestamp denoting the time after which this token should no longer
be valid.

* **notBefore** (DateTime) - A timestamp denoting the time before which this token should be
rejected by the server.

* **admin** (bool) - Set to `true` if you want to disable all security rules for this client. This
will provide the client with read and write access to your entire Firebase.

* **debug** (bool) - Set to `true` to enable debug output from your security rules. You should
generally *not* leave this set to `true` in production (as it slows down the rules implementation
and gives your users visibility into your rules), but it can be helpful for debugging.

Here is an example of how to use the second `options` argument:

```
var tokenGenerator = new Firebase.TokenGenerator("<YOUR_FIREBASE_SECRET>");
var authPayload = new Dictionary<string, object>()
{
  { "uid", "1" },
  { "some", "arbitrary" },
  { "data", "here" }
};
string token = tokenGenerator.CreateToken(authPayload, new Firebase.TokenOptions(admin: true));
```
