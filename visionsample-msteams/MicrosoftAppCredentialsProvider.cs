using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;

namespace VisonSample
{
    public interface IMicrosoftAppCredentialsProvider
    {
        MicrosoftAppCredentials GetCredentials();
    }

    public class MicrosoftAppCredentialsProvider : IMicrosoftAppCredentialsProvider
    {
        public MicrosoftAppCredentials GetCredentials()
        {
            return new MicrosoftAppCredentials();
        }
    }
}