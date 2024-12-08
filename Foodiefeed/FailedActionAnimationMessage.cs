

using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Foodiefeed
{
    public class FailedActionAnimationMessage : ValueChangedMessage<string>
    {
        public FailedActionAnimationMessage(string action) :base (action)
        {
            
        }
    }
}
