# visionsample-msteams-csharp

C# version of the [Node.js sample](https://github.com/aosolis/visionsample-msteams-node) for working with files in bots.

For more details, check out the full README in the other project!

## Getting started
Follow the instructions in the [Microsoft Teams Sample (C#)](https://github.com/OfficeDev/microsoft-teams-sample-complete-csharp), under [Steps to see the full app in Microsoft Teams](https://github.com/OfficeDev/microsoft-teams-sample-complete-csharp#steps-to-see-the-full-app-in-microsoft-teams), to:
1. Set up a tunneling service such as [ngrok](https://ngrok.com/)
2. Register a bot in [Microsoft Bot Framework](https://dev.botframework.com/).

This project contains 2 bots. Depending on which one you want to run, set the corresponding messaging endpoint in the Bot Framework portal, and the right variables in `web.config`. (You can also register two bots and run both simultaneously.)

| Property | Caption Bot | OCR Bot |
|---|---|---|
| Messaging endpoint | `https://xxxx.ngrok.io/caption/messages` | `https://xxxx.ngrok.io/ocr/messages` |
| App ID | Set `CaptionMicrosoftAppId` | Set `OcrMicrosoftAppId` |
| App password | Set `CaptionMicrosoftAppPassword` | Set `OcrMicrosoftAppPassword` |
| Bot ID (`28:<appid>`) | Set `CaptionBotId` | Set `OcrBotId` |

The bot ID is `28:` + the app ID. For example, if your app ID is `11257ab0-5965-4ece-a5c7-fa155fdf1c02`, then the bot ID is `28:11257ab0-5965-4ece-a5c7-fa155fdf1c02`.

[Get an API key](https://azure.microsoft.com/en-us/try/cognitive-services/?api=computer-vision) for the Azure Computer Vision API, and set the following values in `web.config`.
* `VisonEndpoint` = hostname of the Vision API endpoint, e.g., `westus.api.cognitive.microsoft.com`. Note that this depends on where you created your API key.
* `VisionAccessKey` = access key for the Vision API
