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
        chatId:update.Message.Chat.Id,
        voice: InputFile.FromUri("https://github.com/TelegramBots/book/raw/master/src/docs/audio-guitar.mp3"),
    cancellationToken: cancellationToken);
        
}

async Task HandleVideoMessageAync(ITelegramBotClient? botClient, Update update, CancellationToken cancellationToken)
{
    await botClient.SendPhotoAsync(
        chatId: update.Message.Chat.Id,
        caption: "Ishlashi kerek",
        photo:InputFile.FromUri("https://www.youtube.com/watch?v=8kyBdxEBiSk"),
        cancellationToken:cancellationToken
        );
}

async Task HandlaTextMessageAsync(ITelegramBotClient? botClient, Update update, CancellationToken cancellationToken)
{
    await botClient.SendTextMessageAsync(
        chatId: update.Message.Chat.Id,
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

