using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Our.Umbraco.FullTextSearch.NotificationHandlers
{
    public class AddFullTextSearchToServerVariables : INotificationHandler<ServerVariablesParsingNotification>
    {
        public void Handle(ServerVariablesParsingNotification notification)
        {
            notification.ServerVariables.Add("FullTextSearch", new Dictionary<string, string>
            {
                { "Version", Assembly.GetExecutingAssembly().GetName().Version.ToString() }
            });
        }
    }
}
