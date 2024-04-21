Journal
=======
This is a quiz project. One could propably deduce as much from the repository name. So this project will be a lot like one I previously made (it's simply called 'Quiz'). This one will be basically identical, but with some different functions and whatnot. Primarily I want this one to be more smooth, and not as cluttered with old and outdated stuff. And I won't have three separate user interfaces this time. The React app will do for now. I'll be using an SQL database that I'll communicate with using an ASP.NET web API and EF Core.

2024-04-20
-----------
Who needs spare time anyway? I need more REST. So I'll start by preparing the database creation and migrations and such using EF Core.

Several hours later, I've now successfully set up the SQL database. It took me quite a while, with a lot of struggles along the way, but the database has now been created using only EF Core. No SQL queries needed! Seeing that SQL database set up was mighty fine, indeed. I'll continue work on it tomorrow.

2024-04-21
----------
Time to start setting up the controller, the service, and Mediator in the backend. I'll be using all of this to interact with the SQL database manually through Postman. I think I'll create a POST endpoint for creating questions that won't initially be available from the frontend. I'll need some questions for testing the quiz, after all.

I've now been at this for a few hours, and I'll be leaving for a bit. I've successfully added an endpoint to my controller class, and it should be fully funcional. I've been working on the related MediatR command and the quiz class, but the endpoint is not fully ready yet. I also created a 'FloatingIds' table in the SQL database. It always worries me to create a new migration and update the database, and I was real worried for a second when I updated it and didn't see the new table. Took me about five seconds to realize that I'd forgotten to map the new model class to a table in my DbContext class. So I fixed that, created a new migration, and updated the database. And just like that, new table! It's really fun seeing EF Core interact so directly with my SQL database like this. By the way, the 'FloatingIds' table will contain the IDs of questions that have been deleted. Should any IDs exist here at the time of a question being created, that ID will be used. Otherwise, the new question's ID will be the number of questions in the table + 1.
