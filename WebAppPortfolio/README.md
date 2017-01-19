# [aldrinabastillas.com](http://www.aldrinabastillas.com)

## Background
This is my personal site with a link to my Billboard Hot 100 Predictor project, audio visualizer project, links to past projects, and contact information.  


## Files
Notable files include:

#### Server Side
* [WebAppPortfolio/Controllers/HomeController.cs](https://github.com/aldrinabastillas/Personal-Site/blob/master/WebAppPortfolio/Controllers/HomeController.cs):
  Returns the home page.
* [WebAppPortfolio/Controllers/SpotifyController.cs](https://github.com/aldrinabastillas/Personal-Site/blob/master/WebAppPortfolio/Controllers/SpotifyController.cs):
  Returns the predictor app's page, calls the machine learning web service, and queries the database with Entity.
* [WebAppPortfolio/Classes/SpotifyAPIs.cs](https://github.com/aldrinabastillas/Personal-Site/blob/master/WebAppPortfolio/Classes/SpotifyAPIs.cs):
  Wrappers around the Spotify Web APIs, that use asynchronous tasks to call them and Json.NET to deserialize the response.
* [WebAppPortfolio/Classes/RedisSongList.cs](https://github.com/aldrinabastillas/Personal-Site/blob/master/WebAppPortfolio/Classes/RedisSongList.cs):
  Implments the [ISongList interface](https://github.com/aldrinabastillas/Personal-Site/blob/master/WebAppPortfolio/Interfaces/ISongList.cs) querying
  a Redis cache for a song list on a given year. On cache misses, queries the DB using the
  [SQLSongList](https://github.com/aldrinabastillas/Personal-Site/blob/master/WebAppPortfolio/Classes/SQLSongList.cs) implementation of the interface.

#### Markup
* [WebAppPortfolio/Views/Shared/_Layout.cshtml](https://github.com/aldrinabastillas/Personal-Site/blob/master/WebAppPortfolio/Views/Shared/_Layout.cshtml):
  Master HTML layout page
* [WebAppPortfolio/Views/Home/Index.cshtml](https://github.com/aldrinabastillas/Personal-Site/blob/master/WebAppPortfolio/Views/Home/Index.cshtml):
  The home page's HTML
* [WebAppPortfolio/Views/Spotify/Index.cshtml](https://github.com/aldrinabastillas/Personal-Site/blob/master/WebAppPortfolio/Views/Spotify/Index.cshtml):
  The Billboard Hot 100 Predictor App's HTML
* [WebAppPortfolio/Views/Home/_Visualizer.cshtml](https://github.com/aldrinabastillas/Personal-Site/blob/master/WebAppPortfolio/Views/Home/_Visualizer.cshtml):
  The modal pop-up for the audio visualizer.

#### Client Behavior
* [WebAppPortfolio/Scripts/Spotify/Spotify.js](https://github.com/aldrinabastillas/Personal-Site/blob/master/WebAppPortfolio/Scripts/Spotify/Spotify.js):
  Predictor app's client behavior code to search the Spotify library, display results from the machine learning web service, 
  and display results from querying the SQL database with Entity.

## Things I Learned
List of new things I had to learn, issues I encountered during this project, or concepts realized in general.

#### Data Science
* Higher accuracy doesn't necessarily lead to a better model.  In this case, accuracy was increasing
due to a higher rate of false positives, but other metrics of performance decreased, like precision, the ratio
of true positives.  
* Common metrics to evaluate performance of a binary classification model. For more info, see 
[link1](https://docs.microsoft.com/en-us/azure/machine-learning/machine-learning-evaluate-model-performance#evaluating-a-binary-classification-model)
and [link2](https://blogs.msdn.microsoft.com/andreasderuiter/2015/02/09/performance-measures-in-azure-ml-accuracy-precision-recall-and-f1-score/).
* How to extrapolate quanitfiable data to describe subjective qualities, like using the Billboard Hot 100 chart as a proxy for popularity.
* Difficulties with obtaining a data set with exactly the features you want and how to change your hypotheses based on what data is available.
* A more in depth discussion can be found in the panels at the bottom of the [app](http://aldrinabastillas.com/Spotify/Index)

#### Web apps
* How to use asynchronous tasks to consume web services
* How to work with HTML headers
* How to customize SQL connection strings for local debugging and release on the Azure cloud
* How to create a client controller with AngularJS
* How to resolve virtual directory paths when bundled CSS files link to other files in the solution
* How to write to the Windows Event Log and to a trace file
* How to write unit tests and use dependency injection
* How to use a Redis cache to save from querying the database 

#### Game Development 
* How to use different programming paradigms like working with Unity's 
  [event functions](https://docs.unity3d.com/Manual/ExecutionOrder.html) rather than using traditional OOP
  concepts, like constructors for example.
* How and when to implement creational design patterns, like the Factory Method pattern.
* Basic vector algebra

## Technologies
* Web-app framework: [ASP.NET MVC](https://www.asp.net/mvc)
* Front-end frameworks: [Bootstrap](http://getbootstrap.com/), [AngularJS](https://angularjs.org/),
					   [Semantic UI](http://semantic-ui.com/)
* Game engine: [Unity3D](https://unity3d.com/)
* Database: [Microsoft SQL in Azure](https://azure.microsoft.com/en-us/services/sql-database/), 
            [Entity Framework](https://www.asp.net/entity-framework)
* Home page layout: [Start Bootstrap](https://startbootstrap.com/template-overviews/grayscale/)
* Web Services: [Microsoft Azure Machine Learning Studio](https://studio.azureml.net/), 
                [Spotify Web API](https://developer.spotify.com/web-api/)
* JavaScript Libraries: [jQuery](http://jquery.com/), [Json.NET](http://www.newtonsoft.com/json)
* Unit-testing framework: [NUnit](https://www.nunit.org/)
* Caching framework: [Azure Redis Cache](https://azure.microsoft.com/en-us/services/cache/)


## Future Enhancements
* Customizing the error page for unhandled exceptions on the server.