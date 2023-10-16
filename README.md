# Randobot

## Description

A relatively barebones Twitch Bot featuring a random string generator that pulls from local lists and plugs the value into a customizable string template. The bot features an extremely simple local api to easily handle token setup and rotation without too much involvement from the user outside of some preliminary configuration steps.

This was commissioned mainly for the random string generation (with the game ToME specifically in mind), but I wanted to make something open source and relatively generic that I could share / pull from in the future. Some features are probably a product of this.

#### `PLEASE NOTE:`

This was developed with local hosting and setup in mind. I have made no attempts to ensure this works in nor is secure in a remote hosted environment, and as such I cannot recommend it. Keep that in mind if you decide to go above and beyond. I won't be putting any details on how to set that up here, you're on your own.

## Getting Started

How to get this bot up and running!

### 1. Creating a new application for your bot

First thing you'll need to do is go to the [Twitch Developer Console](https://dev.twitch.tv/console) and Register a new Application for your bot if you have not already done so. If you've not used this console before, it will likely ask for permissions to connect your Twitch account to it. Verify that I haven't created an elaborate phishing scam and then do so.

Register a new Application and decide on a Name. Your Category should probably be Chat Bot. You'll need to set up an `OAuth Redirect URL` here, make sure it is appended with `/token` for this specific bot to work properly. This is the endpoint that twitch will try to send a token to when you set things up. If you're hosting this locally (which you probably are) something like `http://localhost:3000/token` is completely fine. You can always fix it later, but you'll need to put one here to continue regardless. Mark down your `OAuth Redirect URL` and Create your application. Once you do so, you'll get a `Client ID` and a `Client Secret`. You'll only get access to the `Client Secret` on application creation or when you overwrite it with a new one, so make sure to mark it down now, along with your `Client ID`.

### 2. Configuration through config.json

A simple [example.config.json](./example.config.json) has been created that has all the configuration items you need to get started. Don't use or modify this config, just make a copy and rename it to `config.json`; this is what you'll actually use to configure your application. Elements are in the collapsed section below:

<details>
<summary>Config Elements</summary>

- `ClientConfig`
  - `ClientId`
    - Uniquely identifies your application for the token request
    - Set this to the `Client ID` of the bot you created while registering an application
  - `ClientSecret`
    - Securely authorizes the client to act as your application. Like the name, it is to be kept secret. If it's exposed, immediately make a new one and replace it
    - Set this to the `Client Secret` from registering your application
  - `BotUsername`
    - This should probably match the name of the account that the bot is running under. It has to be passed in, so it's here, but to be honest it doesn't seem to error out or cause issues if it's wrong. In the end the generated token dictates who the bot runs as, so this probably just affects logging
  - `LocalAppPort`
    - The local port that your bot will listen in on
    - For a local setup, it's fine to leave this at something like `3000`, just make sure it matches the number used in your `OAuth Redirect URL` and `RedirectBaseUrl`
  - `RedirectBaseUrl`
    - This defines what site Twitch will try to send your generated token to once you `/authorize` the application (bot) properly
    - This should be the `OAuth Redirect URL` you marked down previously _minus_ the `/token` portion. So if your `OAuth Redirect URL` was `http://localhost:3000/token` in the Twitch Developer Console, this should be `http://localhost:3000`
  - `TwitchChannels`
    - this is an array of Twitch Channels that you want your bot to join. Keep in mind that rate limits for sending messages are not only individual to each channel, but there is also a (larger) global rate limit to be aware of that is spread across all connected channels. You probably won't hit it, though.
    - I'd generally keep this to a single channel, unless you've severely extended this project yourself or you know what you're doing.
- `CommandConfig`
  - `CommandCharacter`
    - a single character that defines which symbol (or letter or number) should be considered the `Bot Command Identifier`. Messages prefixed with that character will be treated as commands and processed appropriately
    - it's fine to leave this as `!` unless you want to be special. I'd strongly recommend against setting it to a normal alphanumeric character
  - `BotCommandMapping`
    - this is an array of mappings that will allow you to customize commands to be used for certain internal commands. Each object should have an `Input` (what the user types in) and an `Output` (the internal command the input maps to). These are case-insensitive.
    - this can be set up to have a `Many -> One` relationship, i.e. where multiple commands map to the same internal command
  - `Template`
    - this is a string that defines how the `rando` command will replace randomized strings
    - template substitutions are marked by braces, with the filename to retrieve a replacement from within the braces, like so: `"{filenamehere} and {anotherfilenamehere}"`
    - the system will attempt to find those lists in the `ListDirectory`, generating a random entry and replacing it accordingly for each item in the template
  - `ListDirectory`
    - this is the directory that the `rando` command should check for files to populate the `Template`
    - directory should be either absolute (like `C:/Folder/Path/Here`) or relative to the project directory (something like `./Data` or `../Data`)
    - files should be simple text files, with one entry per line. Examples have been provided in the [Data](./Data/) directory of this repository

All elements are required. If you do not wish to map any commands, you can simple leave the `BotCommandMapping` section empty, like so:
```json
"BotCommandMapping": [ ]
```

At some point I'll probably make the bot request any missing configurations to streamline setup.

</details>

### Starting the bot

Once you have all your configuration bits set, simply run the bot. It should create a console window (unless you ran it from a console). In order to kickstart the bot's connection to twitch, you'll need to visit `<YourBaseUrlHere>/authorize` in your browser (e.g., `http://localhost:3000/authorize` if your `RedirectBaseUrl` is `http://localhost:3000`). This will take you to twitch where it will ask you if you would like to authorize the Application you made in the first step to operate as your account with the permissions specified on the page. Don't accept just yet because:

_`IMPORTANT!` Make sure you are logging in as the account you want the bot to run as!_

Whatever account you use to authorize your application will dictate what the bot runs as. If it helps, you can make an Incognito/Private Window and log in there first, or use a separate browser. Now would be the time to decide on whether to make a separate account for the bot or just to use your own account. Keep in mind that rate limits are _slightly_ more forgiving for mods/broadcasters of a channel versus regular users, so it's a good idea to have the bot be one of those. With all that in mind, make sure the authorization request looks correct and if it does, accept it.

Once you authorize the application, if you've set up your `OAuth Redirect URL` and `RedirectBaseUrl` properly, the bot will automatically pick up the generated token and proceed with connecting according to the settings you specified. If you've set them up _incorrectly_ you'll likely get a 404 in your browser. Double check your work and try again.

And there you go, the bot should be running and the application should be able to handle refreshing tokens as needed on its own without having to go through the `/authorize` step again. Rarely, your refresh token may expire (if you change your password or disconnect the account from your twitch application), in which case you can just run through the `/authorize` step again.

## Other Configuration bits

### Logging

Logging is configured through `NLog`, via the [NLog.config](./NLog.config) file. This topic is simply too broad to explain here, so look it up if you wanna mess with it. Right now it writes logs to a file in the `/logs` folder, and archives a log every day to `/logs/archives`, or if the logfile gets too large. It's also set to only keep a maximum of 30 archived logs.

## Contributing

If there's something wrong with the code here, feel free to make an issue. If you pull it down, make some changes and break it, keep it to yourself. As it stands, I don't care too much about security as this was intended to be used locally. If you care a lot about it, go ahead and fork it and make a PR and I'll review it.

### Formatting

I'm using an `.editorconfig` for this one. I'm fairly new to C# so this is helping me stay consistent.
  - I've simply grabbed the MS template from [here](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/code-style-rule-options#example-editorconfig-file) and made some additions/modifications that are personal preferences
  - Probably gonna change as I move through things and learn more, but it's a bit daunting as it stands

If anyone has a not-shit way of linting a C# project (either something I can just run externally or something that works with VSCode), please let me know.

## License

Basic MIT License see [LICENSE](./LICENSE) for more info