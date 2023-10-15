# Randobot

## Description

A relatively barebones Twitch Bot featuring a random string generator that pulls from local lists and plugs the value into a customizable string template. The bot features an extremely simple local api to easily handle token setup and rotation without too much involvement from the user outside of a few preliminary configuration steps.

This was commissioned mainly for the random string generation, but I wanted to make something open source and relatively generic that I could share / pull from in the future.

## Getting Started

### Creating an application for your bot

First thing you'll need to do is go to the [Twitch Developer Console](https://dev.twitch.tv/console) and create a new Application for your bot. If you've not used this before, it will likely ask for permissions to connect your Twitch account to it.

Register a new Application, decide on a name and then mark down the `ClientId` and `ClientSecret`. Then decide on an `OAuth Redirect URL`, appended with `/token`. This is the endpoint that twitch will try to send a token to when you setup your bot. If you're hosting this locally (which you probably are) something like `http://localhost:3000/token` is completely fine. If you're hosting this on a website somewhere, you'll likely have to make some code changes to get things working right so have at it but don't you _dare_ contact me if you break it afterwards. 

Also mark down your `Redirect URL`.

### Configuration through config.json

A simple [example.config.json](./example.config.json) has been created that has all the configuration items you need to get started. You will create your own `config.json` file with your actual values in it, don't attempt to use the example config. 

// TODO get environment shit working

// TODO fix redirect uri + port config settings

- `ClientId`
    - the `ClientId` of the bot you created while registering an application. It is used to identify the application (your bot) requesting access to the account you wish to run the bot as.
- `ClientSecret`
    - the `ClientSecret` from the previous step. This is used to securely authenticate that the Client making requests as the bot is running is indeed who they say they are. Like the name, it is to be kept secret. If it's exposed, immediately make a new one and replace it.
- `BotUsername`
    - this should match the name of the account that the bot is running under. It has to be passed in, so it's here, but to be honest it doesn't error out or cause issues if it's wrong. In the end the generated token dictates who the bot runs as, so this probably just affects logging.
- `BaseUri`
    - This should be the `Redirect URL` you marked down previously _minus_ the `/token` portion. So if your `Redirect URL` was `http://localhost:3000/token` in the dev console, this should be `http://localhost:3000`. This defines what port the bot is listening to for token creation and how it constructs the Redirect URI to match where twitch expects to send your new token to.
- `TwitchChannels`
    - this is an array of Twitch Channels that you want your bot to join. Keep in mind that rate limits for sending messages are not only individual to each channel, but there is also a (larger) global rate limit to be aware of that is spread across all connected channels. You probably won't hit it, though.
- `CommandCharacter`
    - a single character that defines which symbol (or letter or number) should be considered the `Bot Command Identifier`. It's fine to leave it as `!`.
- `BotCommandMapping`
    - this is an array of mappings that will allow you to customize commands to be used for certain internal commands. As it stands there's only one, but it's a simple system that also allows a Many -> One relationship if you want certain commands to have multiple options. Each object should have an `Input` (what the user types in) and an `Output` (the actual command the input maps to)
- `Template`
    - this is a string that defines how the gacha command will replace randomized strings
    - template substitutions are marked by braces, with the filename to retrieve a replacement from within the braces, like so: `"{filenamehere} and {anotherfilenamehere}"`
    - the system will attempt to find those lists in the `ListDirectory`, generating a random entry and replacing it accordingly for each item in the template
- `ListDirectory`
    - this is the directory that the gacha command should check for files to populate the `Template`
    - directory should be either absolute or relative to the project directory
    - files should be simple text files, with one entry per line. Examples have been provided in the [Data](./Data/) directory of this repository.

### Logging configuration

Logging is configured through `NLog`, via the [NLog.config](./NLog.config) file. This topic is simply too broad to explain here, so look it up if you wanna mess with it. Right now it writes logs to a file in the `/logs` folder, and archives a log every day to `/logs/archives`, or if the logfile gets too large. It's also set to only keep a maximum of 30 archived logs.

### Starting the bot

// TODO ports?

Once you have all your configuration bits set, simply run the bot. It should create a console window (unless you ran it from a console). In order to kickstart the bot's connection to twitch, you'll need to visit `WhateverYourBaseUriIsHere/authorize` in your browser (e.g., `http://localhost:3000/authorize` if your `BaseUri` is `http://localhost:3000`). This will take you to twitch where it will ask you if you would like to authorize the Application you made in the first step to operate as your account with the permissions specified on the page. 

VERY IMPORTANTLY, MAKE SURE THAT YOU ARE LOGGING IN AS THE PROFILE YOU WANT TO RUN THE BOT AS. This WILL dictate what it runs under. If it helps, you can make an Incognito Window and log in there first. Now would be the time to decide on whether to make a separate account for the bot or just to use your own account. Keep in mind that rate limits are _slightly_ more forgiving for mods/broadcasters of a channel versus regular users, so it's a good idea to have the bot be one of those.

Once you authorize the application, if you've set up your `OAuth Redirect URL` and `BaseUri` properly, the bot will pick up the generated token, write the refresh token to a local file cache, and proceed with connecting according to the settings you specified. 

And there you go, the bot should be running and the application should be able to handle refreshing tokens as needed on its own without having to go through the `/authorize` step again. Rarely, your refresh token may expire (if you change your password or disconnect the account from your twitch application), in which case you can just run through the `/authorize` step again.

## Contributing

If there's something wrong with the code here, feel free to make an issue. If you make changes and it something goes wrong, keep it to yourself; I get enough of that sort of thing elsewhere. As it stands, I don't care too much about security as this was intended to be used locally. If you care a lot about it, go ahead and fork it and make a PR and I'll review it.

### VS Code

#### Extensions

Just what I use, do whatever you want obviously but while this is in active development I'm going to be enforcing my personal formatting rules, these should help somewhat with that

- [C#](vscode:extension/ms-dotnettools.csharp)
- [C# Dev Kit](vscode:extension/ms-dotnettools.csdevkit)
  - These two extensions kind of suck BUT they offer in-editor linting options which I desperately need as someone who is new to C#
  - This is going to install their [Intellicode for C# Dev Kit](vscode:extension/ms-dotnettools.vscodeintellicode-csharp) extension as well, which offers up AI code suggestions for tab completion. I find it incredibly annoying and it tends to get in the way of my normal workflow, but if you like it obviously feel free; just wanted to note it here so you know where to disable it if you wish to
- [EditorConfig for VS Code](vscode:extension/EditorConfig.EditorConfig)
  - Paired up with the previous two extensions to set some formatting options
  - I've simply grabbed the MS template from [here](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/code-style-rule-options#example-editorconfig-file) and made some additions/modifications that are personal preferences
  - Probably gonna change as I move through things and learn more, but it's a bit daunting as it stands

## License

Basic MIT License see [LICENSE](./LICENSE) for more info