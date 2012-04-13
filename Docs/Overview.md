# Swipper+

---
## Features
*  Shows users feeds from Facebook, Twitter, and LinkedIn
*  Allows users to switch to a different feed by swipping
*  Support images, text, and links. (Refer to People's app)
*  Data is cached for offline viewing.
*  Basical interactions such as comment, like, and share/retweet is possible
*  Support image gallery style

---

## Layout
-  Hidden tray for managing accounts and link information
-  Hidden tray also allows users to get to settings (if any)
-  Each stream is a Parnorama View. If a source is not linked, there should be no view for it (if possible)
-  Each feed has same width but different height.
-  Although data is different across different social feeds, will try best to align feed layout within app

---

## Project Structure

#### MainPage
The MainPage displays the social lists and create the ViewModels to populate the lists.

#### Models
-  SWFeed (An abstract class for all feeds)
  -  SWImageFeed
  -  SWStatusFeed
  -  SWActionFeed
  -  SWConversationFeed
  -  SWLinkFeed

#### ViewModels
ViewsModels are responsible for getting the data and binding them to the view. 
ViewModels should all have a ObservableCollection so that the view updates.
In our implementation, ViewModels are called Managers.

-  SWSocialLinkManager (Abstract class for each Social Manager)
  -  SWFacebookManager (Contains a list of facebook feeds)
  -  SWTwitterManager (Contains a list of twitter feeds)
  -  SWLinkedInManager (Contains a list of LinkedIn feeds)

#### Views
For each feed style, we have a view for it.

-  SWImageFeed
-  SWStatusFeed
-  SWActionFeed
-  SWLinkFeed

-  SWChannelsView

#### Utils

---

## Notes
-  A data analytics service will be attached to the project to collect user behaviour.