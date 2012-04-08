# Swipper+

---
## Features
*  Shows users feeds from Facebook, Twitter, and LinkedIn
*  Allows users to switch to a different feed by swipping
*  Support images, text, and links. (Refer to People's app)
*  Data is cached for offline viewing.
*  Basical interactions such as comment, like, and share/retweet is possible
*  Support image gallery style

## Layout
-  Hidden tray for managing accounts and link information
-  Hidden tray also allows users to get to settings (if any)
-  Each stream is a Parnorama View. If a source is not linked, there should be no view for it (if possible)
-  Each feed has same width but different height.
-  Although data is different across different social feeds, will try best to align feed layout within app

## Classes Required (SW for prefix)
-  SWMain (The main class that will display all feeds)

-  SWFeed (A abstract class for all feeds)
  - SWImageFeed
  - SWTextFeed
  - SWConversationFeed
  - SWLinkFeed

-  SWLinksView (A class that allow users to manage social links)

-  SWSettingsView (A class for managing settings)

-  SWStreamView (A class that takes in a sociallink and display in on stream)

-  SWImageView (A view displaying a large image)

-  SWDataManager (Class for handling caching and fetching info)

-  SWSocialLink (Abstract class for each social link)
  -  SWFacebookManager (Class for getting Facebook info)
  -  SWTwitterManager (Class for getting Twitter info)
  -  SWLinkedInManager (Class for getting LinkedIn info)

## Notes
-  A data analytics service will be attached to the project to collect user behaviour.