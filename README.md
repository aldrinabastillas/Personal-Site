# [aldrinabastillas.com](http://www.aldrinabastillas.com)

## Background
This is my personal site with a link to my Billboard Hot 100 Predictor project, links to past projects and contact information.  


## Files
Notable files include

#### Server Side
* [WebAppPortfolio/Controllers/HomeController.cs](https://github.com/aldrinabastillas/Personal-Site/blob/master/WebAppPortfolio/Controllers/HomeController.cs):
  Returns the home page.
* [WebAppPortfolio/Controllers/SpotifyController.cs](https://github.com/aldrinabastillas/Personal-Site/blob/master/WebAppPortfolio/Controllers/SpotifyController.cs):
  Returns the predictor page, calls the machine learning web service, and queries the database with Entity.
* [WebAppPortfolio/Classes/SpotifyAPIs.cs](https://github.com/aldrinabastillas/Personal-Site/blob/master/WebAppPortfolio/Classes/SpotifyAPIs.cs):
  Wrappers around the Spotify Web APIs, that uses asynchronous tasks to call them and Json.NET to deserialize the response.

#### Markup
* [WebAppPortfolio/Views/Shared/_Layout.cshtml](https://github.com/aldrinabastillas/Personal-Site/blob/master/WebAppPortfolio/Views/Shared/_Layout.cshtml):
  Master HTML layout page
* [WebAppPortfolio/Views/Home/Index.cshtml](https://github.com/aldrinabastillas/Personal-Site/blob/master/WebAppPortfolio/Views/Home/Index.cshtml):
  The home page's HTML
* [WebAppPortfolio/Views/Spotify/Index.cshtml](https://github.com/aldrinabastillas/Personal-Site/blob/master/WebAppPortfolio/Views/Spotify/Index.cshtml):
  The Billboard Hot 100 Predictor App's HTML

#### Client Behavior
* [WebAppPortfolio/Scripts/Spotify/Spotify.js](https://github.com/aldrinabastillas/Personal-Site/blob/master/WebAppPortfolio/Scripts/Spotify/Spotify.js):
  Predictor app's client behavior code.  Includes code to free-text search the Spotify library, 
  display results from the machine learning web service, and display results from querying SQL with Entity.

## Things I Learned
List of new things I had to learn, issues I encountered during this project, or concepts I realized in general.

#### Data Science
* Higher accuracy doesn't necessarily lead to a better model.  In this case, accuracy was increasing
due to a higher rate of false positives, but other metrics of performance decreased, like precision, the ratio
of true positives.  
* Common metrics to evaluate performance of a binary classification model. For more info, see 
[link1](https://docs.microsoft.com/en-us/azure/machine-learning/machine-learning-evaluate-model-performance#evaluating-a-binary-classification-model)
and [link2](https://blogs.msdn.microsoft.com/andreasderuiter/2015/02/09/performance-measures-in-azure-ml-accuracy-precision-recall-and-f1-score/).
* How to extrapolate quanitfiable data to describe subjective qualities, like using the Billboard Hot 100 chart as a proxy for popularity.
* Difficulties with obtaining a data set with exactly the features you want and how to change your hypotheses based on what is available.
* A more in depth discussion can be found in the accordion panels at the bottom of the [app](http://aldrinabastillas.com/Spotify/Index)

#### Web apps
* How to use asynchronous tasks to consume web services
* How to work with HTML headers
* How to customize SQL connection strings for local debugging and release on the Azure cloud
* How to create a client controller with AngularJS
* How to resolve virtual directory paths when bundled CSS files link to other files in the solution
* How to write to the Windows Event Log and to a trace file

## Technologies

* Web-app framework: [ASP.NET MVC](https://www.asp.net/mvc)
* Front-end frameworks: [Bootstrap](http://getbootstrap.com/), [AngularJS](https://angularjs.org/),
					   [Semantic UI](http://semantic-ui.com/)
* Database: [Microsoft SQL in Azure](https://azure.microsoft.com/en-us/services/sql-database/), 
            [Entity Framework](https://www.asp.net/entity-framework)
* Home page layout: [Start Bootstrap](https://startbootstrap.com/template-overviews/grayscale/)
* Web Services: [Microsoft Azure Machine Learning Studio](https://studio.azureml.net/), 
                [Spotify Web API](https://developer.spotify.com/web-api/)
* JavaScript Libraries: [jQuery](http://jquery.com/), [Json.NET](http://www.newtonsoft.com/json)


## Future Enhancements
* Customizing the error page for unhandled exceptions on the server.