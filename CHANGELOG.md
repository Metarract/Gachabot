# v1.0.2
- Added example Data Directory to publish
- Fixed even more issues with redirect, this time with string concatenation

# v1.0.1
- Fixed an issue with redirect, it was not working before and now it should be


# v1.0.0
A little hefty, but she should work! This one is for Windows 64bit specifically

What we got:

- Simple command bot for twitch with a single command to output a randomized string from local lists, constructed via a configurable template string
- Command Mapping where internal commands can have other words mapped to them
- Simple local API to handle initial token creation / authorization
- Refreshes and rotates tokens as needed
- Logging (through NLog). Defaults to logging everything to ./logs/log.txt, archives once per day / @ size, max 30 logs
- Check README for details on setup