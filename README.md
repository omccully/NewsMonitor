# Extensions
This project is designed to be extensible without needing to recompile the main project. Developers can implement their own INewsSearcherExtension, INewsFilterExtension, or INewsSharerExtension. For the program to recognize your extension, place the dll file containing your extension in the same folder as the NewsMonitor executable, and modify App.config to refer to your extension. Refer to the provided extensions as examples.

All extension interfaces inherit from the ISettingsGroupExtension interface, which allows you to provide a name and a SettingsPage implementation for your extension. Your settings page is dynamically added to the settings window in the app. A KeyValueStorage object is injected into the settings page to provide a common way to store data for your extension. 

## INewsSearcherExtension

News searchers are search engines for news. Extensions for Google and Bing are provided. Implementations of INewSearcherExtension.CreateNewsSearcher(KeyValueStorage settings) return a INewsSearcher based on the extension's settings. For example, the BingNewsSearcherExtension uses an API key from the settings to create a BingNewsSearcher. 

## INewsFilterExtension

You may want to block certain news articles. Extensions are provided for filtering out news articles with titles matching a set of regular expressions, filtering news older than a certain number of days, and filtering news published by certain news organizations. The INewsFilterExtension implementation can provide a quick-filter window. This allows the user to quickly and easily add a certain article to the filter without having to go into the extension's settings. 


## INewsSharerExtension

The user is able to share news articles using any of the implementations of INewsSharerExtension. For example, the provided RedditNewsSharerExtension allows the user to share the article on multiple subreddits at a time. 

To implement a INewsSharerExtension, it is good practice to also implement a serializable IShareJob to encapsulate your share operation. This way, if the program closes before your operations were finished, they can be restored. The IShareJob objects are executed one at a time. This is a particularly important feature if the website you're sharing the article to has flood prevention. If there is an exception thrown by an IShareJob in the queue, the user is prompted and is able to skip the job. 
