using BalanBot.Models;
using AdaptiveCards.Templating;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.TeamsFx.Conversation;
using Newtonsoft.Json;

namespace BalanBot.Commands
{
    /// <summary>
    /// The <see cref="BalanCommandHandler"/> registers a pattern with the <see cref="ITeamsCommandHandler"/> and 
    /// responds with an Adaptive Card if the user types the <see cref="TriggerPatterns"/>.
    /// </summary>
    public class BalanCommandHandler : ITeamsCommandHandler
    {
        private readonly ILogger<BalanCommandHandler> _logger;
        private readonly string _adaptiveCardFilePath = Path.Combine(".", "Resources", "BalanCard.json");

        public IEnumerable<ITriggerPattern> TriggerPatterns => new List<ITriggerPattern>
        {
            // Used to trigger the command handler if the command text contains 'balanbot'
            new RegExpTrigger("/balanbot")
        };

        public BalanCommandHandler(ILogger<BalanCommandHandler> logger)
        {
            _logger = logger;
        }

        public async Task<ICommandResponse> HandleCommandAsync(ITurnContext turnContext, CommandMessage message, CancellationToken cancellationToken = default)
        {
            _logger?.LogInformation($"Bot received message: {message.Text}");

            // Fetch GIF
            var BalanUrls = new[]
            {
                "https://d1ophd2rlqbanb.cloudfront.net/2021/speakers/balansubramanian79.jpeg",
                "https://m.media-amazon.com/images/S/dmp-catalog-images-prod/images/a89d3476-8ca5-4701-b961-07571d920174/79e0df8d-d294-44fe-b436-d0d02bb3f158--1929413970._SX576_SY576_BL0_QL100__UXNaN_FMjpg_QL85_.jpg"
            };

            var random = new Random().Next(0, BalanUrls.Length - 1);

            // Load and populate adaptive card with data
            var cardTemplate = await File.ReadAllTextAsync(_adaptiveCardFilePath, cancellationToken);
            var cardContent = new AdaptiveCardTemplate(cardTemplate).Expand
            (
                new BalanModel
                {
                    BalanUrl = BalanUrls[random]
                }
            );
            var activity = MessageFactory.Attachment
            (
                new Attachment
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = JsonConvert.DeserializeObject(cardContent),
                }
            );

            // Post message to channel with adaptive card
            return new ActivityCommandResponse(activity);
        }
    }
}
