using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using VideoLibrary;

string token = "5939275366:AAGC1hS4X6FePojIaPHZ3qKfLMDnEDpNd2Q";

//var botClient = new TelegramBotClient(token);


var botClient = new TelegramBotClient(token);

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    // Echo received message text
    //Message sentMessage = await botClient.SendTextMessageAsync(
    //    chatId: chatId,
    //    text: "You said:\n" + messageText,
    //    cancellationToken: cancellationToken);





    if (messageText.Contains("https://") && (messageText.Contains("youtube.com")
        || messageText.Contains("youtu.be.com")))
    {
        YouTube youTube = new YouTube();
        YouTubeVideo youTubeVideo = youTube.GetVideo(messageText);

        var name = youTubeVideo.FullName;

        await botClient.SendVideoAsync(
                chatId: chatId,
                video: youTubeVideo.Stream(),
                caption: messageText,
                cancellationToken: cancellationToken
        );
    }

    else if (messageText.Contains("instagram") && messageText.Contains(".com/reel"))
    {
        //VIDEO
        string replecedMessage = messageText.Replace("www.", "dd");
        Message messageVideo = await botClient.SendVideoAsync(
            chatId: chatId,
            video: $"{replecedMessage}",
            supportsStreaming: true,
            cancellationToken: cancellationToken
        );
    }

    else if (messageText.Contains("instagram") && messageText.Contains(".com/p"))
    {

        //PHOTO
        string replecedMessage = messageText.Replace("www.", "dd");
        Message message1 = await botClient.SendPhotoAsync(
        chatId: chatId,
        photo: $"{replecedMessage}",
        caption: "<b>Ara bird</b>. <i>Source</i>: <a href=\"https://t.me/reddmi_bot\">DownloaderBot</a>",
        parseMode: ParseMode.Html,
        cancellationToken: cancellationToken);
    }

    //else if(messageText.Contains(""))
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}