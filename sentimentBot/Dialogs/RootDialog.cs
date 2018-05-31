using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using System.Collections.Generic;
using Microsoft.Rest;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace sentimentBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // Calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            analysisText a=new analysisText();
            double score = a.textAnalyse((string)activity.Text);

            // Return our reply to the user
            await context.PostAsync($"You sent {activity.Text} which was has a sentiment of {score} !");

            context.Wait(MessageReceivedAsync);
        }
    }

    public class analysisText
    {
        class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("Ocp-Apim-Subscription-Key", "c834291a236a4f33bb1963fa8053833d");
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }

        public double textAnalyse(string s)
        {
            double ans=0;

            ITextAnalyticsAPI client = new TextAnalyticsAPI(new ApiKeyServiceClientCredentials());
            client.AzureRegion = AzureRegions.Westcentralus;


            SentimentBatchResult result3 = client.SentimentAsync(
                   new MultiLanguageBatchInput(
                       new List<MultiLanguageInput>()
                       {
                          new MultiLanguageInput("en", "0", s),
                       })).Result;


            foreach (var document in result3.Documents)
            {
                ans += (double)document.Score;   
            }
            return ans;
        }
    }
}