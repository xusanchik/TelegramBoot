using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botClient = new TelegramBotClient("6332306275:AAGI4-wfyfvaP-ldH-YWFDzSIbG1KpgZ_Ew");

using CancellationTokenSource cts = new();
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
cts.Cancel();
async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    var Handlar = update.Type switch
    {
        UpdateType.Message => HandlaMessageAsync(botClient, update, cancellationToken),
        UpdateType.EditedMessage => HandlaMessageAsync(botClient, update, cancellationToken),
        _ => HandleUnknowUpdateType(botClient, update, cancellationToken)
    };
    try
    {
        await Handlar;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
}
async Task HandlaMessageAsync(ITelegramBotClient? botClient, Update update, CancellationToken cancellationToken)
{
    var message = update.Message;
    var handlar = message.Type switch
    {
        MessageType.Text => HandlaTextMessageAsync(botClient, update, cancellationToken),
        MessageType.Video => HandleVideoMessageAync(botClient, update, cancellationToken),
        MessageType.Voice => HandleAudioMessageAsync(botClient , update , cancellationToken),
        _ => HandlaUnkowMessageAsync(botClient, update, cancellationToken)
    };
}
async Task HandleAudioMessageAsync(ITelegramBotClient? botClient, Update update, CancellationToken cancellationToken)
{
    await botClient.SendVoiceAsync(
        chatId:update.Id,
        voice: InputFile.FromUri("https://github.com/TelegramBots/book/raw/master/src/docs/audio-guitar.mp3"),
    cancellationToken: cancellationToken);
        
}

async Task HandleVideoMessageAync(ITelegramBotClient? botClient, Update update, CancellationToken cancellationToken)
{
    await botClient.SendPhotoAsync(
        chatId: update.Id,
        caption: "Ishlashi kerek",
        photo:InputFile.FromUri( "https://www.google.com/imgres?imgurl=https%3A%2F%2Fwww.stat.uz%2Fimages%2Fishbilarmonlik.jpg&tbnid=OLzPvpKHzp-qGM&vet=12ahUKEwjHs73m6t-DAxXhOhAIHd0jCvEQMygHegQIARBA..i&imgrefurl=https%3A%2F%2Fwww.stat.uz%2Fen%2Fabout%2Fcentral-office&docid=iyGn_1ISAN1_EM&w=499&h=400&q=samadov%20xusan&ved=2ahUKEwjHs73m6t-DAxXhOhAIHd0jCvEQMygHegQIARBA"),
        cancellationToken:cancellationToken
        );
}

async Task HandlaTextMessageAsync(ITelegramBotClient? botClient, Update update, CancellationToken cancellationToken)
{
    await botClient.SendTextMessageAsync(
        chatId: update.Id,
        text: update.Message.Text = "nima holavosan ukamjonim"
        ); 
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


async Task HandleUnknowUpdateType(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    throw new NotImplementedException();
}
async Task HandlaUnkowMessageAsync(ITelegramBotClient? botClient, Update update, CancellationToken cancellationToken)
{
    throw new NotImplementedException();
}

