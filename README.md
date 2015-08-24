# pokerdemo
A web application that evaluates poker hands.

To run this web application, open Poker Demo.sln in Visual Studio, set the Web Front End project as the Startup project and run the solution.

All packages needed to build and run the application should be downloaded automatically through Nuget.

If the Azure Toolkit for Visual Studio is not installed where the solution is opened, an error may appear during the loading of the solution indicating that a project could not be opened and the Azure deployment project will appear as unloaded. This can be ignored as it will not affect the running of the project locally.

Tests are separated between two test projects, Playing Cards Tests and Web Front End Tests. All logic in the Playing Cards project is covered but no attempt was made to cover login in framework-generated (ASP.Net MVC, ASP.Net, etc.) files. Logic in custom-built MVC controllers *is* covered.

Code coverage rates were determined using OpenCover (https://www.nuget.org/packages/opencover) but nothing there needs to be installed to run and evaluate the project itself if these metrics are not of interest.