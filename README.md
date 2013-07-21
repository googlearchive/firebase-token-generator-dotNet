# .Net Firebase Token Generator
Library for generating Firebase authentication tokens from .Net.

## Installation
The easiest way to install is via [NuGet](https://nuget.org/packages/FirebaseTokenGenerator).  Just search 
for FirebaseTokenGenerator in NuGet or install it via the Package Manager Console:

    PM> Install-Package FirebaseTokenGenerator

You can also download the source and compile it yourself, of course!

## Usage
To generate a token with an arbitrary auth payload:
    
    var tokenGenerator = new Firebase.TokenGenerator("YOUR_FIREBASE_SECRET_HERE");
    var authPayload = new Dictionary<string, object>()
    {
        { "some", "arbitrary" },
        { "data", "here" }
    };
    string token = tokenGenerator.CreateToken(authPayload);

You can also specify custom options via a second argument to CreateToken.  For example, to create an admin token, you could use:

    var tokenGenerator = new Firebase.TokenGenerator("YOUR_FIREBASE_SECRET_HERE");
    string token = tokenGenerator.CreateToken(null, new Firebase.TokenOptions(admin: true));

See the [Firebase Authentication Docs](https://www.firebase.com/docs/security/authentication.html) for more information about authentication tokens.

License
-------
[MIT](http://firebase.mit-license.org)
