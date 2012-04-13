#Feed

All data related to feeds is all handled through the ```Feed``` class under the Models folder.
Since we are converting JSON into Feed objects, I have opt not to create a number of classes that inherits from the Feed model since that will complicate code alot. Instead all information is under the main ```Feed``` class and if a feed does not have a certain information, that field is set to ```null``` or ```0``` depending on the type of the field.
    
    SWFeed

    // Can be Facebook, LinkedIn, Twiteer, etc.
    LinkType (Enum)

    // Can be Text, Attachment, Conversation, etc
    // This will determine how the UI handles the feed
    FeedType (Enum)

    // ID of the feed, directly from the source
    ID (string)

    Date (DateTime)

    // Id of the person who posted it
    SourcePerson (UInt64)

    // ID of the person who the feed is for. Used for conversation feeds
    TargetPerson (UInt64)

    Message (string)

    // This corresponds to an action
    Description (string)

    // Handles likes & comments.
    // Things that only Facebook have
    FacebookProperties (FacebookItem)

    // Contains information about an attachment
    AttachmentProperties (Attachment)

---
**Supporting our main ```Feed``` class is the ```Person```, ```Comment```, ```Attachment```, and ```FacebookItem``` class. These classes encapsulate related information into a single class for our ease of use.**

### Comment
The ```Comment``` class contain information related to a Facebook comment. This includes the message, userID, date, # of likes, and a bool that states whether the user has liked this comment.
The ```Comment``` class is used by the ```FacebookItem``` class to record the comments for a feed.

### Attachment
Since most Facebook feeds are either a pictures, a link, or a video this class is almost included in every ```Feed``` class. The ```Attachment``` class contains information regarding the attachment for us to display to the user. 
Currently the class contains information regarding the href of the attachment, the type, the name, and the icon of the attachment. There are quiet a few information that we did not collect from Facebook regarding the attachment. These information are ommitted now since we are not planning to use them for the first version.

### FacebookItem
This class contains information regarding the likes and the comments. For likes, we also record the IDs of your friends who have liked this item. For comments we record few preview comments since Facebook did not provide the whole comment list for each feed. For the whole comment list, a separate request must be issued.

### Person
The ```Person``` class contain some sparse information regarding a Person. Right now we record the ID, the name, the type (Facebook, Twitter, etc), and the icon of the person.
Since the ```Person``` class is used a lot by the ```Feeds``` class we store the core Person object in a People Dictionary identified by the Person's ID. The static People Dictionary is thus persisted and is not related to the Feeds.
A distinction is made between People from different SocialLinks. Thus we have a ```People``` class that contains a static Dictionary for each of the People in these different SocialLinks. To find a person, all we need is the type (Facebook, Twitter, etc) and the ID of the individual.